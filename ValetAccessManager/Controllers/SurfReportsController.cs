using System.Collections.Generic;
using System.Web.Http;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using ValetAccessManager.Interfaces;
using System.Threading.Tasks;

namespace ValetAccessManager.Controllers
{
    internal sealed class SurfReportsSASController : StatelessService, ISurfReportsSASController
    {
        private readonly CloudStorageAccount account;
        private readonly string blobContainer;
        private Guid serviceId = Guid.NewGuid();

        //initialize blob configuration in the constructor. 
        public SurfReportsSASController(StatelessServiceContext context)
            : base(context)
        {        
            //Cloud settings as per App.config. The storage account comes from the Azure portal (I created a blob account specifically
            //for this app) and you get the connection info from the "Access Keys" pane under "settings"
            this.account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BLOB"));

            //The name of the actual blob container. A single storage account application can have multiple containers. The 
            //surf report container has information on the gnar to be shredded. 
            this.blobContainer = "surf-reports";
        }

        // GET api/SurfReports 
        /// <summary>
        /// Retrieves a Shared Access Signature for the surf-reports blob storage for either upload or download, depending on the
        /// query param passed in
        /// </summary>
        /// <param name="SAStype">
        /// shared access signature for upload or download. Must be "upload" or "download" lolz
        /// </param>
        /// <returns></returns>
        public async Task<StorageEntitySas> Get(string SAStype)
        {
            ServiceEventSource.Current.Message("SAS key requested from valet service of type {0}: {1}", SAStype, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            try
            {
                var blobName = Guid.NewGuid();
                StorageEntitySas blobSas = new StorageEntitySas();
                // Retrieve a shared access signature of the location we should upload/download this file to/from
                //Task<StorageEntitySas>[] getSas = new Task<StorageEntitySas>[1];
                Task<StorageEntitySas> getSas;
                //StorageEntitySas sas = new StorageEntitySas();
                if (SAStype.ToLowerInvariant() == "upload")
                {
                    //getSas[0] = Task.Factory.StartNew(() => sas = this.GetSharedAccessReferenceForUpload(blobName.ToString()));
                    getSas = new Task<StorageEntitySas>(() => this.GetSharedAccessReferenceForUpload(blobName.ToString()));
                }
                else if (SAStype.ToLowerInvariant() == "download")
                {
                    //getSas[0] = Task.Factory.StartNew(() => sas = this.GetSharedAccessReferenceForDownload(blobName.ToString()));
                    getSas = new Task<StorageEntitySas>(() => this.GetSharedAccessReferenceForDownload(blobName.ToString()));
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent("You must specify upload or download in the query request"),
                            ReasonPhrase = "Invalid request format"
                        });
                }

                Trace.WriteLine(string.Format("Blob Uri: {0} - Shared Access Signature: {1}", blobSas.BlobUri, blobSas.Credentials));

                //var retrieved =  Task.Factory.ContinueWhenAll<StorageEntitySas>(getSas, completedTask => { return sas; });
                //var x = await Task.Run(() => getSas);

                var retrieved = await getSas;
                //return await retrieved;
                return retrieved;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("An error has ocurred"),
                        ReasonPhrase = "Critical Exception"
                    });
            }
        }

        private StorageEntitySas GetSharedAccessReferenceForDownload(string blobName)
        {
            var blobClient = this.account.CreateCloudBlobClient();
            // here we need to enable CORS on the blob client. So, lets set up the Cors props! If you don't know what CORS is, 
            //you'll need to google that
            ServiceProperties blobServiceProperties = blobClient.GetServiceProperties();
 
            //Create a new CORS properties configuration
            blobServiceProperties.Cors = new CorsProperties();
 
            blobServiceProperties.Cors.CorsRules.Add(new CorsRule()
            {
                AllowedHeaders = new List<string>() { "*" },
                AllowedMethods = CorsHttpMethods.Put | CorsHttpMethods.Get | CorsHttpMethods.Head | CorsHttpMethods.Post,
                AllowedOrigins = new List<string>() { "*" },
                ExposedHeaders = new List<string>() { "*" },
                MaxAgeInSeconds = 1800 // 30 minutes
            });
 
            //Set the properties on the client
            blobClient.SetServiceProperties(blobServiceProperties);
            var container = blobClient.GetContainerReference(this.blobContainer);

            var blob = container.GetBlockBlobReference(blobName);
            
            if (blob.Exists())
            {
                throw new Exception("Blob does not exist");
            }

            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,

                // Create a signature for 5 min earlier to leave room for clock skew
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),

                // Create the signature for as long as necessary -  we can 
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5)
            };
            
            var sas = blob.GetSharedAccessSignature(policy);

            return new StorageEntitySas
            {
                BlobUri = blob.Uri,
                Credentials = sas
            };
        }

        /// <summary>
        /// We return a limited access key that allows the caller to upload a file to this specific destination for defined period of time
        /// </summary>
        private StorageEntitySas GetSharedAccessReferenceForUpload(string blobName)
        {
            var blobClient = this.account.CreateCloudBlobClient();
 
            // here we need to enable CORS on the blob client. So, lets set up the Cors props! If you don't know what CORS is, 
            //you'll need to google that
            ServiceProperties blobServiceProperties = blobClient.GetServiceProperties();
 
            //Create a new CORS properties configuration
            blobServiceProperties.Cors = new CorsProperties();
 
            blobServiceProperties.Cors.CorsRules.Add(new CorsRule()
            {
                AllowedHeaders = new List<string>() { "*" },
                AllowedMethods = CorsHttpMethods.Put | CorsHttpMethods.Get | CorsHttpMethods.Head | CorsHttpMethods.Post,
                AllowedOrigins = new List<string>() { "*" },
                ExposedHeaders = new List<string>() { "*" },
                MaxAgeInSeconds = 1800 // 30 minutes
            });
 
            //Set the properties on the client
            blobClient.SetServiceProperties(blobServiceProperties);
            var container = blobClient.GetContainerReference(this.blobContainer);

            var blob = container.GetBlockBlobReference(blobName);

            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Write,

                // Create a signature for 5 min earlier to leave room for clock skew
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),

                // Create the signature for as long as necessary -  we can 
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5)
            };

            var sas = blob.GetSharedAccessSignature(policy);

            return new StorageEntitySas
            {
                BlobUri = blob.Uri,
                Credentials = sas,
                Name = blobName
            };
        }

    }
}

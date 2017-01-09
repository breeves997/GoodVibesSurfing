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

namespace ValetAccessManager.Controllers
{
    public class SurfReportsController : ApiController
    {
        private readonly CloudStorageAccount account;
        private readonly string blobContainer;

        //initialize blob configuration in the constructor. 
        public SurfReportsController()
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
        public StorageEntitySas Get(string SAStype)
        {
            try
            {
                var blobName = Guid.NewGuid();
                StorageEntitySas blobSas = new StorageEntitySas();
                // Retrieve a shared access signature of the location we should upload/download this file to/from
                if (SAStype.ToLowerInvariant() == "upload")
                {
                    blobSas = this.GetSharedAccessReferenceForUpload(blobName.ToString());
                }
                else if (SAStype.ToLowerInvariant() == "download")
                {
                    blobSas = this.GetSharedAccessReferenceForDownload(blobName.ToString());
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

                return blobSas;
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

        public struct StorageEntitySas
        {
            public string Credentials;
            public Uri BlobUri;
            public string Name;
        }
    }
}

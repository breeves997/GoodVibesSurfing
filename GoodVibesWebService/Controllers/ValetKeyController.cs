using System;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValetAccessManager.Interfaces;
using CloudUtilities;
using ValetAccessManager.Interfaces.Contracts;
using System.Web.Http;

namespace GoodVibesWebService.Controllers
{
    [Route("api/[controller]")]
    public class ValetKeyController : ApiController
    {
        // GET api/ValetKey/upload
        /// <summary>
        /// Gets a shared access signature key for blob uploads/downloads
        /// So you're supposed to be able to do something like Get([FromUri] SharedAccessSignatureRequest request)
        /// and have web api build out the request object for you implicky. But microsoft being the paradigm of half assed, shittily documented
        /// and inflexible frameworks that they are, I can't get it to work for the life of me. And I simply refust to build my own type converter
        /// or model binder. So let's throw a nice clean contract to the wind and just make our method signature match what we need.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<StorageEntitySas> Get(string blobName, string requestType)
        {
            if (String.IsNullOrWhiteSpace(requestType)) throw new InvalidOperationException("you need to give a request type");
            if (String.IsNullOrWhiteSpace(blobName)) throw new InvalidOperationException("you need to give a blob name");
            ServiceEventSource.Current.Message("SAS key requested through API of type {0}: {1}", requestType, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            var valetKeyUri = new ServiceUriBuilder("ValetAccessManager");
            var surfSASclient = ServiceProxy.Create<ISurfReportsSASController>(valetKeyUri.ToUri());
            StorageEntitySas sasKey = await surfSASclient.Get(Utilities.ParseEnum<SASAllowedRequests>(requestType));
            //string ben = await "sdfjadslk";

            return sasKey;
        }

    }
}

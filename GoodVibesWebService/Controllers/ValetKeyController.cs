using System;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValetAccessManager.Interfaces;
using CloudUtilities;
using ValetAccessManager.Interfaces.Contracts;

namespace GoodVibesWebService.Controllers
{
    [Route("api/[controller]")]
    public class ValetKeyController : Controller
    {
        private readonly ISASKeyProvider Svc;
        public ValetKeyController(ISASKeyProvider svc)
        {
            Svc = svc;

        }
        // GET api/ValetKey/upload
        /// <summary>
        /// Gets a shared access signature key for blob uploads/downloads
        /// So you're supposed to be able to do something like Get([FromUri] SharedAccessSignatureRequest request)
        /// and have web api build out the request object for you implicky. But microsoft being the paradigm of "build a shit ton of features which only work 80%"
        /// that they are, I can't get it to work for the life of me. And I simply refust to build my own type converter
        /// or model binder. So let's just make our method signature match what we need.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<StorageEntitySas> Get(string blobName, string requestType)
        {
            //TODO add validator for this guy and inject the validation service into this class
            if (String.IsNullOrWhiteSpace(requestType)) throw new InvalidOperationException("you need to give a request type");
            if (String.IsNullOrWhiteSpace(blobName)) throw new InvalidOperationException("you need to give a blob name");
            ServiceEventSource.Current.Message("SAS key requested through API of type {0}: {1}", requestType, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //call the service SurfReportsSASController in $/ValetAccessManager/Controllers/SurfReportController.cs and ask it to give us a gosh darned SAS key
            StorageEntitySas sasKey = await Svc.Get(Utilities.ParseEnum<SASAllowedRequests>(requestType));
            //string ben = await "sdfjadslk";

            return sasKey;
        }

    }
}

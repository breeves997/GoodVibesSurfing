using System;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValetAccessManager.Interfaces;

namespace GoodVibesWebService.Controllers
{
    [Route("api/[controller]")]
    public class ValetKeyController : Controller
    {
        // GET api/ValetKey/upload
        [HttpGet("{type}")]
        public async Task<StorageEntitySas> Get(string type)
        {
            ServiceEventSource.Current.Message("SAS key requested through API of type {0}: {1}", type, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            var surfSASclient = ServiceProxy.Create<ISurfReportsSASController>(new Uri("fabric:/GoodVibesSurfing/ValetAccessManager"));
            StorageEntitySas sasKey = await surfSASclient.Get(type);
            //string ben = await "sdfjadslk";

            return sasKey;
        }

    }
}

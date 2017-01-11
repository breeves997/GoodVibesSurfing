using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using GetSnowConditions.Interfaces;
using Microsoft.ServiceFabric.Services.Client;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GoodVibesWebService.Controllers
{
    [Route("api/[controller]")]
    public class SnowServiceController : Controller
    {

        // GET: api/values
        [HttpGet]
        public async Task<string> Get()
        {
            ISnowServiceProvider snow =
                ServiceProxy.Create<ISnowServiceProvider>(new Uri("fabric:/GoodVibesSurfing/GetSnowConditions"),
                new ServicePartitionKey(0));
            var status = await snow.GetSnowConditionServiceStatus();
            return status;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        public class TurnServiceOffRequest
        {
            public bool TurnServiceOn { get; set; }
        }
        // POST api/snowservice
        //Pass in true to start service, false to kill service
        [HttpPost]
        public async Task<bool> Post([FromBody]TurnServiceOffRequest value)
        {
            //We need to provide two sets of info to the service proxy. The name of the service as registered
            //wit SF and the partition key, which is set to 0 currently because we only have one partition.
            //partitioning info found here https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-concepts-partitioning
            ISnowServiceProvider snow =
                ServiceProxy.Create<ISnowServiceProvider>(new Uri("fabric:/GoodVibesSurfing/GetSnowConditions"),
                new ServicePartitionKey(0));
            bool updatedSuccesfully = await snow.KillGetSnowConditionsService(value.TurnServiceOn);
            return updatedSuccesfully;

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

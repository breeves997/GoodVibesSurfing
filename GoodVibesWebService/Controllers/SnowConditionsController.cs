using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using GetSnowConditions.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;
using CloudUtilities.FaultTolerance;

namespace GoodVibesWebService.Controllers
{
    [Route("api/[controller]")]
    public class SnowConditionsController : Controller
    {

        private readonly IRetryStrategy RetryPattern;
        public SnowConditionsController(IRetryStrategy retryPattern)
        {
            this.RetryPattern = retryPattern;

        }
        // GET api/snowconditions
        [HttpGet]
        public async Task<string> Get()
        {
            //We need to provide two sets of info to the service proxy. The name of the service as registered
            //wit SF and the partition key, which is set to 0 currently because we only have one partition.
            //partitioning info found here https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-concepts-partitioning
            ISnowProvider snow =
                ServiceProxy.Create<ISnowProvider>(new Uri("fabric:/GoodVibesSurfing/GetSnowConditions"),
                new ServicePartitionKey(0));

            try
            {
                var taskResult = await RetryPattern.DoTaskWithRetry(snow.GetSnowConditionsAsync);

                //due to some nested task garbaggio we're going to wait for this again. The reason is that our delegate is running a task
                //which runs a task... so the await for some reason isn't sufficient
                taskResult.Wait();

                var conditions = taskResult.Result;
                //Dictionary<string, long> conditions = await snow.GetSnowConditionsAsync();
                return JsonConvert.SerializeObject(conditions);
            }
            catch (AggregateException ex)
            {
                if(ex.InnerExceptions.Any(x=> x is TimeoutException) || ex.GetBaseException() is TimeoutException)
                {
                    //need to tell the user that we timed out after retry
                }
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException || ex.InnerException is TimeoutException)
                {
                    string x = "fsdsf";
                    //implement retry pattern
                }

            }

            throw new ApplicationException();

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
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

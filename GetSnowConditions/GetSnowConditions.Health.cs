using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Fabric.Testability.Scenario;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using GetSnowConditions.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using System.Fabric.Health;
using System.Diagnostics;

namespace GetSnowConditions
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed partial class GetSnowConditions : ISnowServiceProvider
    {

        /// <summary>
        /// Kills the get snow conditions service for a random period of time. Turns it back on if a bool of true is passed in
        /// </summary>
        /// <param name="turnOn"></param>
        /// <returns></returns>
        public async Task<bool> KillGetSnowConditionsService(bool turnOn)
        {
            if (turnOn)
            {
                GlobalContext.ServiceIsTurnedOff = false;
                return !turnOn;
            }
            else
            {
                GlobalContext.ServiceIsTurnedOff = !turnOn;
                //This warning is valid since we don't need to, or want to, await the execution of this
                TurnServiceBackOnAfterARandomInterval().
                    ContinueWith(t => Trace.TraceWarning(t.Exception.ToString()), TaskContinuationOptions.OnlyOnFaulted);
                return !turnOn;
            }

        }

        private async Task TurnServiceBackOnAfterARandomInterval()
        {
            Random rnd = new Random();
            //generate a time between 0 and 15 seconds to sleep for
            int sleepTime = rnd.Next(1, 15000);
            await Task.Delay(sleepTime);
            GlobalContext.ServiceIsTurnedOff = false;
        }

        public async Task<bool> RestartGetSnowConditionsService()
        { 
            var replicaSelector = ReplicaSelector.PrimaryOf(PartitionSelector.RandomOf(GlobalContext.ServiceName));
            //PartitionSelector namedPartitionSelector = PartitionSelector.PartitionKeyOf(new Uri(GlobalContext.appName), "Partition1");

            //await fabricClient.ClusterManager.(new ServicePar); ; //statefull
            return true;
        }

        /// <summary>
        /// Simply returns if the service has been turned off remotely or not
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetSnowConditionServiceStatus()
        {
            if (GlobalContext.ServiceIsTurnedOff)
            {
                return "Switched Off";
            }
            else
            {
                var gotHealth = await GetServiceHealth();
                return "Switched On";
            }
        }
        public async Task<bool> GetServiceHealth()
        {
            ApplicationHealth applicationHealth;

            // Get the application health.
            try
            {
                applicationHealth = await fabricClient.HealthManager.GetApplicationHealthAsync(GlobalContext.AppName);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());

                if (e.InnerException != null)
                    Console.WriteLine("  Inner Exception: " + e.InnerException.Message);

                return false;
            }
            return true;

        }
    }
}

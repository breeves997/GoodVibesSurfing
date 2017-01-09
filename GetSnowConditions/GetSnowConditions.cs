using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using GetSnowConditions.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace GetSnowConditions
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed partial class GetSnowConditions : StatefulService, ISnowProvider
    {
        private FabricClient fabricClient;
        public GetSnowConditions(StatefulServiceContext context)
            : base(context)
        {
            fabricClient = new FabricClient(GlobalContext.ClusterConnection);
        }

        private void ThrowIfServiceTurnedOff()
        {
            if (GlobalContext.ServiceIsTurnedOff)
            {
                throw new TimeoutException();
            }
        }

        public async Task<long> GetNewSnowAsync()
        {
            ThrowIfServiceTurnedOff();
            var myDictionary =
              await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("Snowfall");

            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await myDictionary.TryGetValueAsync(tx, "NewSnow");
                return result.HasValue ? result.Value : 0;
            }
        }
        public async Task<long> GetSnowpackAsync()
        {
            ThrowIfServiceTurnedOff();
            var myDictionary =
              await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("Snowfall");

            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await myDictionary.TryGetValueAsync(tx, "Snowpack");
                return result.HasValue ? result.Value : 0;
            }
        }

        public async Task<Dictionary<string, long>> GetSnowConditionsAsync()
        {
            ThrowIfServiceTurnedOff();
            //this should be an object so we can keep a consistent contract across the client and service
            long[] results = await Task.WhenAll(GetNewSnowAsync(), GetSnowpackAsync());
            return new Dictionary<string, long>()
            {
                ["NewSnow"] = results[0],
                ["Snowpack"] = results[1]
            };

        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new List<ServiceReplicaListener>() {
                new ServiceReplicaListener((context) =>
               this.CreateServiceRemotingListener(context))
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("Snowfall");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var rando = new Random();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var newSnow = await myDictionary.TryGetValueAsync(tx, "NewSnow");
                    var snowpack = await myDictionary.TryGetValueAsync(tx, "Snowpack");

                    ServiceEventSource.Current.ServiceMessage(this, "New Snow: {0}, Snowpack: {1}",
                        newSnow.HasValue ? newSnow.Value.ToString() : "0",
                        snowpack.HasValue ? snowpack.Value.ToString() : "0");


                    var snowfall = rando.Next(1, 10);
                    var melt = rando.Next(0, 5);

                    await myDictionary.AddOrUpdateAsync(tx, "NewSnow", 0, (key, value) => snowfall);
                    await myDictionary.AddOrUpdateAsync(tx, "Snowpack", 0, (key, value) => value + snowfall - melt);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
        }

    }
}

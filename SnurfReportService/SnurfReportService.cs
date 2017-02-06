using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SnurfReportService.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace SnurfReportService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class SnurfReportService : StatefulService, ISnowReportsService, ISurfReportsService
    {
        public SnurfReportService(StatefulServiceContext context)
            : base(context)
        { }

        //---------------reliable collections in this service--------------------------------
        //reports filed day-by-day where the value is the hash code of the actual report location in the reliable collection. The hashed key is the Poster+PostTime
        //var dailySurfReports = await this.StateManager.GetOrAddAsync<IReliableDictionary<DateTime, List<long>>>("DailySurfReports");
        //var dailySnowReports = await this.StateManager.GetOrAddAsync<IReliableDictionary<DateTime, List<long>>>("DailySnowReports");

        //var surfReports = await this.StateManager.GetOrAddAsync<IReliableDictionary<long, SurfReport>>("SurfReports");
        //var snowReports = await this.StateManager.GetOrAddAsync<IReliableDictionary<long, SnowReport>>("SnowReports");
        //---------------reliable collections in this service--------------------------------
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var snow = await this.StateManager.GetOrAddAsync<IReliableDictionary<long, SnowReport>>(CollectionNames.SnowReports);
            var surf = await this.StateManager.GetOrAddAsync<IReliableDictionary<long, SurfReport>>(CollectionNames.SurfReports);
            using (var tx = this.StateManager.CreateTransaction())
            {
                if (await surf.GetCountAsync(tx) == 0)
                {
                    SampleInitialization.surf.ForEach(async x => await SaveReport(x));
                }
                //unneccessary but w/w
                await tx.CommitAsync();
            }
            using (var tx = this.StateManager.CreateTransaction())
            {
                if (await snow.GetCountAsync(tx) == 0)
                {
                    SampleInitialization.snow.ForEach(async x => await SaveReport(x));
                }
                //unneccessary but w/w
                await tx.CommitAsync();
            }

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

        public async Task<IEnumerable<T>> GetDailyReports<T>(DateTime day) where T : ReportBase
        {
            day = day.Date;
            List<T> rtn = new List<T>();
            var dailyreports = await this.StateManager.GetOrAddAsync<IReliableDictionary<DateTime, List<long>>>(CollectionNames.GetDailyReportDictionaryName<T>());
            using (var tx = this.StateManager.CreateTransaction())
            {
                //get a list of the report keys for the day
                var reportIdsConditional = await dailyreports.TryGetValueAsync(tx, day);
                if (!reportIdsConditional.HasValue) return rtn;
                var reportIds = reportIdsConditional.Value;

                var reports = await this.StateManager.GetOrAddAsync<IReliableDictionary<long, T>>(CollectionNames.GetReportDictionaryName<T>());
                using (var txx = this.StateManager.CreateTransaction())
                {
                    foreach (var id in reportIds)
                    {
                        var report = await reports.TryGetValueAsync(txx, id);
                        if (report.HasValue) rtn.Add(report.Value);
                    }

                }

            }
            return rtn;
        }

        public async Task<T> GetReport<T>(long id) where T : ReportBase
        {
            var reports = await this.StateManager.GetOrAddAsync<IReliableDictionary<long, T>>(CollectionNames.GetReportDictionaryName<T>());
            using (var tx = this.StateManager.CreateTransaction())
            {
                var report = await reports.TryGetValueAsync(tx, id);
                if (report.HasValue) return report.Value;
            }
            return null;

        }
        public async Task<T> SaveReport<T>(T report) where T : ReportBase
        {
            var reports = await this.StateManager.GetOrAddAsync<IReliableDictionary<long, T>>(CollectionNames.GetReportDictionaryName<T>());
            var dailies = await this.StateManager.GetOrAddAsync<IReliableDictionary<DateTime, List<long>>>(CollectionNames.GetDailyReportDictionaryName<T>());
            var key = BuildReportKey(report);
            using (var tx = this.StateManager.CreateTransaction())
            {
                await dailies.AddOrUpdateAsync(tx, report.Date, new List<long>() { BuildReportKey(report) },
                    (k, v) =>
                    {
                        v.Add(BuildReportKey(report));
                        return v;
                    });
                var success = await reports.AddOrUpdateAsync(tx, key, report, (k, v) => report);
                await tx.CommitAsync();
                return success;
            }
        }

        /// <summary>
        /// Creates a key from the hash of the poster and post date. This puts the restriction that a person may only post once a day, which isn't realistic but
        /// we can say that because this isn't a real effin app
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        private static long BuildReportKey(ReportBase report)
        {
            return new Tuple<DateTime, string>(report.Date, report.Poster).GetHashCode();

        }
        private static long BuildReportKey(DateTime date, string poster)
        {
            return new Tuple<DateTime, string>(date, poster).GetHashCode();

        }

        #region workarounds for RPC not supporting generics

        public Task<IEnumerable<SnowReport>> GetDailySnowReports(DateTime day)
        {
            return this.GetDailyReports<SnowReport>(day);
        }

        public Task<SnowReport> GetSnowReport(long id)
        {
            return this.GetReport<SnowReport>(id);
        }

        public Task<SnowReport> SaveSnowReport(SnowReport report)
        {
            return this.SaveReport<SnowReport>(report);
        }

        public Task<IEnumerable<SurfReport>> GetDailySurfReports(DateTime day)
        {
            return this.GetDailyReports<SurfReport>(day);
        }

        public Task<SurfReport> GetSurfReport(long id)
        {
            return this.GetReport<SurfReport>(id);
        }

        public Task<SurfReport> SaveSurfReport(SurfReport report)
        {
            return this.SaveReport<SurfReport>(report);
        }

        public async Task<IEnumerable<SurfReport>> GetDailySurfReportByKey(DateTime day, string poster)
        {
            var report = await this.GetReport<SurfReport>(BuildReportKey(day, poster));
            return new List<SurfReport>() { report };
        }

        public async Task<IEnumerable<SnowReport>> GetDailySnowReportByKey(DateTime day, string poster)
        {
            var report = await this.GetReport<SnowReport>(BuildReportKey(day, poster));
            return new List<SnowReport>() { report };
        }

        #endregion
    }
}

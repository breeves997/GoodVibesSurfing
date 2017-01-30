using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnurfReportService.Interfaces
{
    //this isn't supported because the RPC protocol can't do generics
    public interface ISnurfReportsService : IService
    {
        Task<IEnumerable<T>> GetDailyReports<T>(DateTime day) where T : ReportBase;
        Task<T> GetReport<T>(long id) where T : ReportBase;
        Task<T> SaveReport<T>(T report) where T : ReportBase;
    }

    public interface ISurfReportsService : IService
    {
        Task<IEnumerable<SurfReport>> GetDailySurfReports(DateTime day);
        Task<SurfReport> GetSurfReport(long id);
        Task<SurfReport> SaveSurfReport(SurfReport report);
    }
    public interface ISnowReportsService : IService
    {
        Task<IEnumerable<SnowReport>> GetDailySnowReports(DateTime day);
        Task<SnowReport> GetSnowReport(long id);
        Task<SnowReport> SaveSnowReport(SnowReport report);
    }
}

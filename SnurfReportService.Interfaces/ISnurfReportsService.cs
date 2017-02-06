using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnurfReportService.Interfaces
{
    //this isn't supported because the RPC protocol can't do generics
    //also RPC doesn't do overloads. WOULD BE NICE TO KNOW AT BUILD TIME
    public interface ISnurfReportsService : IService
    {
        Task<IEnumerable<T>> GetDailyReports<T>(DateTime day) where T : ReportBase;
        Task<IEnumerable<T>> GetDailyReports<T>(DateTime day, string poster) where T : ReportBase;
        Task<T> GetReport<T>(long id) where T : ReportBase;
        Task<T> SaveReport<T>(T report) where T : ReportBase;
    }

    public interface ISurfReportsService : IService
    {
        Task<IEnumerable<SurfReport>> GetDailySurfReports(DateTime day);
        Task<IEnumerable<SurfReport>> GetDailySurfReportByKey(DateTime day, string poster);
        Task<SurfReport> GetSurfReport(long id);
        Task<SurfReport> SaveSurfReport(SurfReport report);
    }
    public interface ISnowReportsService : IService
    {
        Task<IEnumerable<SnowReport>> GetDailySnowReports(DateTime day);
        Task<IEnumerable<SnowReport>> GetDailySnowReportByKey(DateTime day, string poster);
        Task<SnowReport> GetSnowReport(long id);
        Task<SnowReport> SaveSnowReport(SnowReport report);
    }
}

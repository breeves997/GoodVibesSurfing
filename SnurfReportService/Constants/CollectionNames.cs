using SnurfReportService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnurfReportService
{
    public static class CollectionNames
    {
        public static string DailySurfReports = "DailySurfReports";
        public static string DailySnowReports = "DailySnowReports";
        public static string SurfReports = "SurfReports";
        public static string SnowReports = "SnowReports";
        public static string GetDailyReportDictionaryName<T>() where T : ReportBase
        {
            Type t = typeof(T);
            if (t == typeof(SurfReport))
            {
                return DailySurfReports;
            }
            else if (t == typeof(SnowReport))
            {
                return DailySnowReports;
            }
            else throw new ApplicationException("Dictionary does not exist");


        } 
        public static string GetReportDictionaryName<T>() where T : ReportBase
        {
            Type t = typeof(T);
            if (t == typeof(SurfReport))
            {
                return SurfReports;
            }
            else if (t == typeof(SnowReport))
            {
                return SnowReports;
            }
            else throw new ApplicationException("Dictionary does not exist");


        } 
    }
}

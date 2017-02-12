using GoodVibesWebService.ClientContracts;
using SnurfReportService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodVibesWebService
{
    /// <summary>
    /// DONT PUT ANYTHING HERE OTHER THAN TRUE GENERIT UTILITY METHODS
    /// </summary>
    public static class Utilities
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }

    public static class ContractExtensions
    {
        public static SnowReport ToDomain(this SnowReportContract src)
        {
            return new SnowReport(src.Rating, src.Poster, src.Location, src.Date, src.Attachments, src.Temperature, src.Visibility);
        }
        public static SurfReport ToDomain(this SurfReportContract src)
        {
            return new SurfReport(src.Rating, src.Poster, src.Location, src.Date, src.Attachments, src.WaveSize, src.Period );
        }
    }
}

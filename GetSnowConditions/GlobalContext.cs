using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetSnowConditions
{
    public static class GlobalContext
    {
        public static string ClusterConnection { get; } = @"http://localhost/";
        public static Uri AppName { get; } = new Uri(@"fabric:/GoodVibesSurfing");
        public static Uri ServiceName { get; } = new Uri(@"fabric:/GoodVibesSurfing/GetSnowConditions");
        /// <summary>
        /// Indicates whether or not an external request has told the service to go down and throw timeout exceptions
        /// Used for demonstration and testing purposes
        /// </summary>
        public static bool ServiceIsTurnedOff { get; set; } = false;
    }
}

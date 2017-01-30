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
}

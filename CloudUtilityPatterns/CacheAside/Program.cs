using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CacheAside
{
    class Program
    {
        static void Main(string[] args)
        {
            var mgr = new UserInterface();
            mgr.Start();
        }
    }
}

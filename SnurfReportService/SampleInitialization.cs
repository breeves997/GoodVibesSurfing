using SnurfReportService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnurfReportService
{
    public class SampleInitialization
    {
            public static List<SurfReport> surf = new List<SurfReport>()
            {
                new SurfReport(Ratings.Mediocre, "Spencer", "Tofino", DateTime.Today, new List<Uri>(), 3.5, 15),
                new SurfReport(Ratings.Mediocre, "Ben", "Tofino", DateTime.Today, new List<Uri>(), 3.5, 15),
                new SurfReport(Ratings.Good, "Stephen", "Santa Cruz", DateTime.Today, new List<Uri>(), 4.5, 20),
                new SurfReport(Ratings.Good, "Alex", "Santa Cruz", DateTime.Today, new List<Uri>(), 4.5, 20),

            };

            public static List<SnowReport> snow = new List<SnowReport>()
            {
                new SnowReport(Ratings.Great, "Ben", "Revelstoke", DateTime.Today, new List<Uri>(), -10, "Sunny"),
                new SnowReport(Ratings.Great, "Rhys", "Revelstoke", DateTime.Today, new List<Uri>(), -10, "Sunny"),
                new SnowReport(Ratings.Good, "Becky", "Fernie", DateTime.Today, new List<Uri>(), -2, "Whiteout"),
            };

    }
}

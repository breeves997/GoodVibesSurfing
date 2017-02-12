using SnurfReportService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodVibesWebService.ClientContracts
{
    public class SurfReportContract : BaseReportContract
    {
        public  double WaveSize { get; set; }
        public  int Period {get; set;}

    }
    public class SnowReportContract : BaseReportContract
    {
        public int Temperature { get; set; }
        public string Visibility { get; set; }

    }
    public abstract class BaseReportContract
    {
        public Ratings Rating { get; set; }
        public string Poster { get; set; }
        public IEnumerable<Uri> Attachments  { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }

    }
}

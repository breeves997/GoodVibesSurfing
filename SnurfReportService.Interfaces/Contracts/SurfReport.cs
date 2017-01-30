using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SnurfReportService.Interfaces
{
    [DataContract]
    public sealed class SurfReport : ReportBase
    {
        public SurfReport(Ratings rating, string poster, string location, DateTime date, IEnumerable<Uri> attachments, double waveSize, int period) 
            : base(rating, poster, location, date, attachments)
        {
            WaveSize = waveSize;
            Period = period;
        }

        [DataMember]
        public readonly double WaveSize;
        [DataMember]
        public readonly int Period;

        public override ReportBase AddAttachment(Uri attachmentLocation)
        {
            return new SurfReport(Rating, Poster, Location, Date, ((ImmutableList<Uri>)Attachments).Add(attachmentLocation), WaveSize, Period);
        }
    }
}

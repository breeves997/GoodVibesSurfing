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
    public sealed class SnowReport : ReportBase
    {
        public SnowReport(Ratings rating, string poster, string location, DateTime date, IEnumerable<Uri> attachments, int temp, string visibility) 
            : base(rating, poster, location, date, attachments)
        { Temperature = temp;
            Visibility = visibility;
        }

        [DataMember]
        public readonly int Temperature;
        [DataMember]
        public readonly string Visibility;

        public override ReportBase AddAttachment(Uri attachmentLocation)
        {
            return new SnowReport(Rating, Poster, Location, Date, ((ImmutableList<Uri>)Attachments).Add(attachmentLocation), Temperature, Visibility);
        }
    }
}

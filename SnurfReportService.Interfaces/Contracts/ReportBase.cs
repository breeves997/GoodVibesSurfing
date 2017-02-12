using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SnurfReportService.Interfaces
{
    /// <summary>
    /// Uses IExtensibleDataObject for forward compatible data contracts so that as changes to the contract occur it will not break the existing collection
    /// as per this post https://msdn.microsoft.com/library/ms731083.aspx
    /// </summary>
    [DataContract]
    public abstract class ReportBase : IExtensibleDataObject
    {
        public ReportBase(Ratings rating, string poster, string location, DateTime date, IEnumerable<Uri> attachments)
        {
            Rating = rating;
            Poster = poster;
            Location = location;
            Attachments = attachments?.ToImmutableList() ?? new List<Uri>().ToImmutableList();
            Date = date.Date;
        }
       [OnDeserialized]
       private void OnDeserialized(StreamingContext context) {
          // Convert the deserialized collection to an immutable collection
          Attachments = Attachments.ToImmutableList();
       }
        public abstract ReportBase AddAttachment(Uri attachmentLocation);
        [DataMember]
        public readonly Ratings Rating;
        [DataMember]
        public readonly string Poster;
        [DataMember]
        public IEnumerable<Uri> Attachments { get; private set; }
        [DataMember]
        public readonly string Location;
        [DataMember]
        public readonly DateTime Date;
        private ExtensionDataObject theData;

        public virtual ExtensionDataObject ExtensionData
        {
            get { return theData; } 
            set { theData = value; }
        }
    }

    public enum Ratings
    {
        None,
        Awful,
        Poor,
        Mediocre,
        Good,
        Great
    }
}

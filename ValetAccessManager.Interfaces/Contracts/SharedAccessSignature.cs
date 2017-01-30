using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValetAccessManager.Interfaces.Contracts
{
    public struct StorageEntitySas
    {
        public string Credentials;
        public Uri BlobUri;
        public string Name;
    }

    public class SharedAccessSignatureRequest
    {
        public string BlobName { get; set; }
        public string Request { get; set; }
    }

    public enum SASAllowedRequests
    {
        Upload = 0,
        Download = 1
    }
}

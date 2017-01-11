
namespace ValetAccessManager.Interfaces
{
    using Microsoft.ServiceFabric.Services.Remoting;
    using System;
    using System.Threading.Tasks;

    public struct StorageEntitySas
    {
        public string Credentials;
        public Uri BlobUri;
        public string Name;
    }
    public interface ISurfReportsSASController : IService
    {
        Task<StorageEntitySas> Get(string SAStype);
    }
}

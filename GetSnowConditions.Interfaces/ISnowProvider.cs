
namespace GetSnowConditions.Interfaces
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting;
    using System.Collections.Generic;

    public interface ISnowProvider: IService
     {
         Task<Dictionary<string, long>> GetSnowConditionsAsync();
         Task<long> GetNewSnowAsync();
         Task<long> GetSnowpackAsync();
     }
}

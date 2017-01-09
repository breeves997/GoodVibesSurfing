
namespace GetSnowConditions.Interfaces
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting;
    using System.Collections.Generic;

    public interface ISnowServiceProvider: IService
     {
         Task<bool> KillGetSnowConditionsService(bool turnOn);
         Task<string> GetSnowConditionServiceStatus();
     }
}

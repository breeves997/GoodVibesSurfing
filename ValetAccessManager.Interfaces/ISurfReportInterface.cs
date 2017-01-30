﻿
namespace ValetAccessManager.Interfaces
{
    using Contracts;
    using Microsoft.ServiceFabric.Services.Remoting;
    using System;
    using System.Threading.Tasks;

    public interface ISurfReportsSASController : IService
    {
        Task<StorageEntitySas> Get(SASAllowedRequests request);
    }
}

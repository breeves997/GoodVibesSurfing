using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric.Description;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using ValetAccessManager.Controllers;

namespace ValetAccessManager
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class ValetAccessManager : StatelessService, IService
    {
        public ValetAccessManager(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Creates service listener configured with Owin from the OwinCommunicationListener.cs class. Check out this guide for the source
        /// and descriptions https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-communication-webapi
        /// UPDATE: The port configuration was failing every time I tried to do it the way the guide says (yay Microsoft... just jokes eff you)
        /// so I had to reconfigure based off this answer on SO and manually pass in my concrete services to the remoting listener.
        /// TODO: Find out how to fix the situation where I need to manually pass concrete service instances to the instance listener.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            //For the time being you need to create new listener instances for each IService implementation you have
            return new[] { new ServiceInstanceListener(context => new FabricTransportServiceRemotingListener(context, new SurfReportsSASController(context))) };

            ////This is the way the guide told me to do it which doesn't work due to some sort of port configuration issue. I always get a CLR exception
            ////that the port OWIN is listening on is invalid
            //var endpoints = Context.CodePackageActivationContext.GetEndpoints()
            //                       .Where(endpoint => endpoint.Protocol == EndpointProtocol.Http || endpoint.Protocol == EndpointProtocol.Https)
            //                       .Select(endpoint => endpoint.Name);

            //return endpoints.Select(endpoint => new ServiceInstanceListener(
            //    serviceContext => new OwinCommunicationListener(Startup.ConfigureApp, serviceContext, ServiceEventSource.Current, endpoint), endpoint));
        }

    }
}

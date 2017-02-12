using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CloudUtilities.FaultTolerance;
using SnurfReportService.Interfaces;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using CloudUtilities;
using ValetAccessManager.Interfaces;
using ValidationService.Interfaces;

namespace GoodVibesWebService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddTransient<IRetryStrategy, BasicRetryPattern>();
            services.AddTransient<ISurfReportsService>(ctx =>
            {
                var snurfUri = new ServiceUriBuilder("SnurfReportService");
                return ServiceProxy.Create<ISurfReportsService>(snurfUri.ToUri(), new ServicePartitionKey(0));
            });
            services.AddTransient<ISnowReportsService>(ctx =>
            {
                return ServiceProxy.Create<ISnowReportsService>(new Uri("fabric:/GoodVibesSurfing/SnurfReportService"), new ServicePartitionKey(0));
            });
            services.AddTransient<ISnurfReportValidationService>(ctx =>
            {
                return ServiceProxy.Create<ISnurfReportValidationService>(new Uri("fabric:/GoodVibesSurfing/ValidationService"), new ServicePartitionKey(0));
            });
            services.AddTransient<ISASKeyProvider>(ctx =>
            {
                var valetKeyUri = new ServiceUriBuilder("ValetAccessManager");
                return ServiceProxy.Create<ISASKeyProvider>(valetKeyUri.ToUri());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}

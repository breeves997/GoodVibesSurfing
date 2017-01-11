using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Cors;
using System.Web.Cors;

[assembly: OwinStartup(typeof(ValetAccessManager.Startup))]

namespace ValetAccessManager
{
    public static class Startup
    {
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "ValetAccessManager",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
            //lets set up CORS on this bad bitch so we can call it from the client
            // we're just going to allow everything cause I don't give a flying hootenany
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}

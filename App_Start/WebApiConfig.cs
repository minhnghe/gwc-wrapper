using System.Configuration;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Routing;

namespace GreateRewardsService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*")
            {
                SupportsCredentials = true,
            };
            string[] allOrigins = ConfigurationManager.AppSettings[Constants.AppSettingKeys.Origin].Split(';');
            foreach (string key in allOrigins)
            {
                cors.Origins.Add(key);
            }
            config.EnableCors(cors);
            // Web API routes
            var constraints = new { httpMethod = new HttpMethodConstraint(HttpMethod.Options) };
            config.Routes.IgnoreRoute("OPTIONS", "*", constraints);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

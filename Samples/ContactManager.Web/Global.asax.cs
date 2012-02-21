using System.Web.Http;
using System.Web.Http.Controllers;
using Autofac;
using ContactManager.Formatters;
using ContactManager.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Ninject;
using Thinktecture.Web.Http.DI;
using Thinktecture.Web.Http.Formatters;
using Thinktecture.Web.Http.Handlers;
using Thinktecture.Web.Http.Selectors;

namespace ContactManager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterApis(HttpConfiguration config)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            config.Formatters[0] = new JsonNetFormatter(serializerSettings);
            config.Formatters.Add(new ProtoBufFormatter()); 
            config.Formatters.Add(new ContactPngFormatter());
            config.Formatters.Add(new VCardFormatter());
            config.Formatters.Add(new ContactCalendarFormatter());
            
            config.MessageHandlers.Add(new UriFormatExtensionHandler(new UriExtensionMappings()));
            
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<InMemoryContactRepository>().As<IContactRepository>();
            containerBuilder.RegisterType<CorsActionSelector>().As<IHttpActionSelector>();
            var container = containerBuilder.Build();

            config.ServiceResolver.SetResolver(new AutoFacResolver(container));
                
            config.Routes.MapHttpRoute(
                "Default",
                "{controller}/{id}/{ext}",
                new { id = RouteParameter.Optional, ext = RouteParameter.Optional }
            );
            //config.Routes.MapHttpRoute(
            //    "Contacts", // Route name
            //    "{controller}/{action}/{id}/{ext}", // URL with parameters
            //    new { controller = "Contacts", action = "Get", id = RouteParameter.Optional, ext = RouteParameter.Optional } // Parameter defaults
            //);
        }

        protected void Application_Start()
        {
            RegisterApis(GlobalConfiguration.Configuration);
        }
    }
}
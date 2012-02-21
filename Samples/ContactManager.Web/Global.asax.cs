using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using ContactManager.Models;
using ContactManager.Web.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Ninject;
using Thinktecture.Web.Http.DI;
using Thinktecture.Web.Http.Formatters;
using Thinktecture.Web.Http.Handlers;
using Thinktecture.Web.Http.Selectors;

namespace ContactManager.Web
{
    public class MvcApplication : HttpApplication
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
            
            ConfigureResolver(config);

            config.Routes.MapHttpRoute(
                "Default",
                "{controller}/{id}/{ext}",
                new { id = RouteParameter.Optional, ext = RouteParameter.Optional });
        }

        private static void ConfigureResolver(HttpConfiguration config)
        {
            var kernel = new StandardKernel();
            kernel.Bind<IContactRepository>().ToConstant(new InMemoryContactRepository());
            kernel.Bind<IHttpActionSelector>().ToConstant(new CorsActionSelector());

            config.ServiceResolver.SetResolver(new NinjectResolver(kernel));
        }

        protected void Application_Start()
        {
            RegisterApis(GlobalConfiguration.Configuration);
        }
    }
}
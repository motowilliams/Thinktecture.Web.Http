using System.Web.Http;
using ContactManager.Formatters;
using ContactManager.Models;
using Ninject;
using Thinktecture.Web.Http.Formatters;
using Thinktecture.Web.Http.Handlers;

namespace ContactManager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterApis(HttpConfiguration config)
        {
            config.Formatters.Add(new ContactPngFormatter());
            config.Formatters.Add(new VCardFormatter());
            config.Formatters.Add(new CalendarFormatter());
            config.Formatters.Add(new ProtoBufFormatter());

            config.MessageHandlers.Add(new UriFormatExtensionHandler(new UriExtensionMappings()));

            var kernel = new StandardKernel();
            kernel.Bind<IContactRepository>().ToConstant(new ContactRepository());
            config.ServiceResolver.SetResolver(
                t => kernel.TryGet(t),
                t => kernel.GetAll(t));

            config.Routes.MapHttpRoute(
                "Default", // Route name
                "{controller}/{id}/{ext}", // URL with parameters
                new { id = RouteParameter.Optional, ext = RouteParameter.Optional } // Parameter defaults
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
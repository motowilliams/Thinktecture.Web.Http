using System;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.SelfHost;
using ContactManager.Models;
using Ninject;

namespace ContactManager.ConsoleSelfHost
{
    class Program
    {
        private const string webApiUrl = "http://localhost:7777/services/cm";

        static void Main(string[] args)
        {
            var a = Assembly.Load("ContactManager.APIs");
            
            var host = SetupWebApiServer(webApiUrl);
            host.OpenAsync().Wait();

            Console.WriteLine("Web API host running on {0}", webApiUrl);
            Console.WriteLine();
            Console.ReadKey();

            host.CloseAsync().Wait();
        }

        private static HttpSelfHostServer SetupWebApiServer(string webApiUrl)
        {
            var ninjectKernel = new StandardKernel();
            ninjectKernel.Bind<IContactRepository>().To<InMemoryContactRepository>();

            var configuration = new HttpSelfHostConfiguration(webApiUrl);
            configuration.ServiceResolver.SetResolver(
                t => ninjectKernel.TryGet(t),
                t => ninjectKernel.GetAll(t));
            configuration.Routes.MapHttpRoute("default", "{controller}/{id}", new { id = RouteParameter.Optional });

            var host = new HttpSelfHostServer(configuration);

            return host;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Thinktecture.Web.Http.Filters;

namespace Thinktecture.Web.Http.Selectors
{
    public class CorsActionSelector : ApiControllerActionSelector
    {
        private const string Origin = "Origin";
        private const string AccessControlRequestMethod = "Access-Control-Request-Method";
        private const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        private const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        private const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            var originalRequest = controllerContext.Request;
            var isCorsRequest = originalRequest.Headers.Contains(Origin);

            if (originalRequest.Method == HttpMethod.Options && isCorsRequest)
            {
                var accessControlRequestMethod = originalRequest.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();

                if (!string.IsNullOrEmpty(accessControlRequestMethod))
                {
                    var modifiedRequest = new HttpRequestMessage(
                        new HttpMethod(accessControlRequestMethod),
                        originalRequest.RequestUri);
                    controllerContext.Request = modifiedRequest;
                    var actualDescriptor = base.SelectAction(controllerContext);
                    controllerContext.Request = originalRequest;

                    if (actualDescriptor != null)
                    {
                        if (actualDescriptor.GetFilters().OfType<EnableCorsAttribute>().Any())
                        {
                            return new PreflightActionDescriptor(actualDescriptor, accessControlRequestMethod);
                        }
                    }
                }
            }

            return base.SelectAction(controllerContext);
        }

        class PreflightActionDescriptor : HttpActionDescriptor
        {
            readonly HttpActionDescriptor originalAction;
            readonly string accessControlRequestMethod;

            public PreflightActionDescriptor(HttpActionDescriptor originalAction, string accessControlRequestMethod)
            {
                this.originalAction = originalAction;
                this.accessControlRequestMethod = accessControlRequestMethod;
            }

            public override string ActionName
            {
                get { return this.originalAction.ActionName; }
            }

            public override object Execute(HttpControllerContext controllerContext, IDictionary<string, object> arguments)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add(AccessControlAllowMethods, this.accessControlRequestMethod);

                var requestedHeaders = string.Join(
                    ", ",
                    controllerContext.Request.Headers.GetValues(AccessControlRequestHeaders));

                if (!string.IsNullOrEmpty(requestedHeaders))
                {
                    response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
                }

                return response;
            }

            public override ReadOnlyCollection<HttpParameterDescriptor> GetParameters()
            {
                return originalAction.GetParameters();
            }

            public override Type ReturnType
            {
                get { return typeof(HttpResponseMessage); }
            }

            public override ReadOnlyCollection<Filter> GetFilterPipeline()
            {
                return originalAction.GetFilterPipeline();
            }

            public override IEnumerable<IFilter> GetFilters()
            {
                return originalAction.GetFilters();
            }

            public override ReadOnlyCollection<T> GetCustomAttributes<T>()
            {
                return originalAction.GetCustomAttributes<T>();
            }
        }
    }
}
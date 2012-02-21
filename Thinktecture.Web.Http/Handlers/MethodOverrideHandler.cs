﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Thinktecture.Web.Http.Handlers
{
    public class MethodOverrideHandler : DelegatingHandler
    {
        private readonly string[] methods = { "DELETE", "HEAD", "PUT" };
        private const string Header = "X-HTTP-Method-Override";

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post && request.Headers.Contains(Header))
            {
                var method = request.Headers.GetValues(Header).FirstOrDefault();

                if (methods.Contains(method, StringComparer.InvariantCultureIgnoreCase))
                {
                    request.Method = new HttpMethod(method);
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}

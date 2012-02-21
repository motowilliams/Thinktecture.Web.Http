﻿using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Thinktecture.Web.Http.Handlers
{
    // Code based on: http://code.msdn.microsoft.com/Implementing-CORS-support-a677ab5d
    public class SimpleCorsHandler : DelegatingHandler
    {
        private const string Origin = "Origin";
        private const string AccessControlRequestMethod = "Access-Control-Request-Method";
        private const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        private const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        private const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            var isCorsRequest = request.Headers.Contains(Origin);
            var isPreflightRequest = request.Method == HttpMethod.Options;

            if (isCorsRequest)
            {
                if (isPreflightRequest)
                {
                    return Task.Factory.StartNew(() =>
                            {
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                response.Headers.Add(AccessControlAllowOrigin,
                                                    request.Headers.GetValues(Origin).First());

                                var accessControlRequestMethod =
                                    request.Headers.GetValues(AccessControlRequestMethod).
                                        FirstOrDefault();

                                if (accessControlRequestMethod != null)
                                {
                                    response.Headers.Add(AccessControlAllowMethods,
                                                        accessControlRequestMethod);
                                }

                                var requestedHeaders = string.Join(", ", request.Headers.GetValues(AccessControlRequestHeaders));

                                if (!string.IsNullOrEmpty(requestedHeaders))
                                {
                                    response.Headers.Add(AccessControlAllowHeaders,
                                                        requestedHeaders);
                                }

                                return response;
                            }, cancellationToken);
                }
                else
                {
                    return base.SendAsync(request, cancellationToken).ContinueWith(t =>
                            {
                                var resp = t.Result;
                                resp.Headers.Add(
                                    AccessControlAllowOrigin,
                                    request.Headers.GetValues(Origin).First());
                                
                                return resp;
                            });
                }
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
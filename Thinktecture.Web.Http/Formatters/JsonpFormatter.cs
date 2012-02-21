using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Thinktecture.Web.Http.Formatters
{
    public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
    {
        private string callbackQueryParameter;

        public JsonpMediaTypeFormatter()
        {
            var jsonpMediaType = new MediaTypeHeaderValue("application/javascript");

            SupportedMediaTypes.Add(jsonpMediaType);
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

            MediaTypeMappings.Add(new UriPathExtensionMapping("jsonp", jsonpMediaType));
        }

        public string CallbackQueryParameter
        {
            get { return callbackQueryParameter ?? "callback"; }
            set { callbackQueryParameter = value; }
        }

        protected override Task<object> OnReadFromStreamAsync(Type type, Stream stream,
                                                              HttpContentHeaders contentHeaders,
                                                              FormatterContext formatterContext)
        {
            return base.OnReadFromStreamAsync(type, stream, contentHeaders, formatterContext);
        }


        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream,
                                                     HttpContentHeaders contentHeaders,
                                                     FormatterContext formatterContext,
                                                     TransportContext transportContext)
        {
            string callback;

            if (IsJsonpRequest(formatterContext.Response.RequestMessage, out callback))
            {
                return Task.Factory.StartNew(() =>
                        {
                            var writer = new StreamWriter(stream);
                            writer.Write(callback + "(");
                            writer.Flush();
                            base.OnWriteToStreamAsync(type, value, stream, contentHeaders,
                                                    formatterContext, transportContext).Wait();
                            writer.Write(")");
                            writer.Flush();
                        });
            }
            else
            {
                return base.OnWriteToStreamAsync(type, value, stream, contentHeaders, formatterContext, transportContext);
            }
        }

        private bool IsJsonpRequest(HttpRequestMessage request, out string callback)
        {
            callback = null;

            if (request.Method != HttpMethod.Get)
            {
                return false;
            }

            NameValueCollection query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            callback = query[CallbackQueryParameter];

            return !string.IsNullOrEmpty(callback);
        }
    }
}
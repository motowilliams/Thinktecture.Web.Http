using System.Linq;
using System.Web.Http.Filters;

namespace Thinktecture.Web.Http.Filters
{
    public class EnableCorsAttribute : ActionFilterAttribute
    {
        private const string Origin = "Origin";
        private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Request.Headers.Contains(Origin))
            {
                var originHeader = actionExecutedContext.Request.Headers.GetValues(Origin).FirstOrDefault();
                
                if (!string.IsNullOrEmpty(originHeader))
                {
                    actionExecutedContext.Result.Headers.Add(AccessControlAllowOrigin, originHeader);
                }
            }
        }
    }
}
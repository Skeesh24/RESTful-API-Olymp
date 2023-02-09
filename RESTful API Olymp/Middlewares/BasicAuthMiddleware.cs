using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace RESTful_API_Olymp.Middlewares
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            //string authHeader = httpContext.Request.Headers["Authorization"];
            //if (authHeader != null && authHeader.StartsWith("Basic"))
            //{
            //    var userAndPassword = authHeader.Substring("Basic ".Length).Trim();
            //    Encoding encoding = Encoding.UTF8;
            //    var index = authHeader.IndexOf(":");
            //    var username = userAndPassword.Substring(0, index);
            //    var password = userAndPassword.Substring(index + 1);
            //    var password 

            //    if(encoding.GetString(Convert.FromBase64String(password)) == "")
            //    {

            //    }
            //    return _next(httpContext);
            //}
            //else
            //{
            //    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //    return;
            //}
            
            return;
        }

        //private async Task<string> GetFromDb(string name)
        //{
            
        //}
    }

    public static class BasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthMiddleware>();
        }
    }
}

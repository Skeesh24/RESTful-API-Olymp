using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace RESTful_API_Olymp.Domain.CodeResults
{
    public class AuthorizedRequestResult : StatusCodeResult
    {
        private const int statusCode = StatusCodes.Status403Forbidden;
        public AuthorizedRequestResult() : base(statusCode)
        {
        }
    }
}

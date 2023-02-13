using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace RESTful_API_Olymp.Domain.CodeResults
{
    public class ExistedEmailResult : StatusCodeResult
    {
        private const int _code = StatusCodes.Status409Conflict;
        public ExistedEmailResult() : base(_code)
        {
        }
    }
}

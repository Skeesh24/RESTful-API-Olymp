using Microsoft.AspNetCore.Mvc;

namespace RESTful_API_Olymp.Controllers
{
    public class RegistrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

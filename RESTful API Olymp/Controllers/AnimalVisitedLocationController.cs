using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;

namespace RESTful_API_Olymp.Controllers
{
    public class AnimalVisitedLocationController : Controller
    {
        private readonly DataContext? Db;

        public AnimalVisitedLocationController(DataContext? context)
        {
            Db = context;
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}

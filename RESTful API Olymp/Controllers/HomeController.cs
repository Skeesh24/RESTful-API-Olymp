using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;

namespace RESTful_API_Olymp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext? Db;

        public HomeController(DataContext? context)
        {
            Db = context;
        }


        public IActionResult Index()
        {
            ViewBag.Title = "Главная";
            return View();
        }
    }
}

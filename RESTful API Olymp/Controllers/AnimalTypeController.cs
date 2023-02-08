using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Models;

namespace RESTful_API_Olymp.Controllers
{
    public class AnimalTypeController : Controller
    {
        private readonly DataContext? Db;

        public AnimalTypeController(DataContext? context)
        {
            Db = context;
        }


        [HttpGet("animals/types")]
        public IActionResult Types(long typeId)
        {
            ViewBag.Title = "Типы";
            var vm = new TypeViewModel { Types = Db.Types.Where(x => x.Id == typeId).ToList() };
            return View(vm);
        }
    }
}

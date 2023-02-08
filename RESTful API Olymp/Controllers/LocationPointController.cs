using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Models;

namespace RESTful_API_Olymp.Controllers
{
    public class LocationPointController : Controller
    {
        private readonly DataContext? Db;

        public LocationPointController(DataContext? context)
        {
            Db = context;
        }


        [HttpGet/*("locations/")*/]
        public IActionResult Locations(long pointId)
        {
            ViewBag.Title = "Места";
            var vm = new PointViewModel { Points = Db.Points.Where(x => x.Id == pointId).ToList() };
            return View(vm);
        }
    }
}

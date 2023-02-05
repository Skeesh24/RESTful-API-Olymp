using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Models;

namespace RESTful_API_Olymp.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly DataContext? Db;

        public AnimalsController(DataContext? context)
        {
            Db = context;
        }

        [HttpGet]
        public IActionResult Search(DateTime startDateTime, DateTime endDateTime, int chipperId = -1, long chippingLocationId = -1, string lifeStatus = "", string gender = "", int from = -1, int size = -1)
        {
            ViewBag.Title = "Поиск";

            var vm = new AnimalViewModel
            {
                Animals = Db.Animals.
                Where(x => x.ChippingDateTime.CompareTo(startDateTime) >= 0)
                .ToList()
            };

            if (chipperId != -1)
            {
                vm.Animals = vm.Animals.Where(x => x.ChipperId == chipperId).ToList();
            }

            if (chippingLocationId != -1)
            {
                vm.Animals = vm.Animals.Where(x => x.ChippingLocationId == chippingLocationId).ToList();
            }

            if (lifeStatus != "")
            {
                vm.Animals = vm.Animals.Where(x => x.LifeStatus == lifeStatus).ToList();
            }

            if (gender != "")
            {
                vm.Animals = vm.Animals.Where(x => x.Gender == gender).ToList();
            }

            if (from != -1)
            {
                vm.Animals = vm.Animals.Skip(from).ToList();
            }

            if (size != -1)
            {
                vm.Animals = vm.Animals.Take(size).ToList();
            }

            vm.Animals = vm.Animals.OrderBy(x => x.Id).ToList();

            return View(vm);
        }

    }
}

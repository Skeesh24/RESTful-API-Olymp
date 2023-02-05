using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;
using System.Diagnostics;
using System.Security.Policy;

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
        public IActionResult Animals(long animalid)
        {
            var vm = new AnimalViewModel { Animals = Db.Animals.Where(x => x.Id == animalid).ToList() };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Search(DateTime startDateTime, DateTime endDateTime, int chipperId = -1, long chippingLocationId = -1, string lifeStatus = "", string gender = "", int from = -1, int size = -1)
        {
            var vm = new AnimalViewModel {
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

        //[HttpPost]
        //public IActionResult Animals()
        //{
        //    return RedirectToAction("Animals");
        //}

        static HttpClient client = new HttpClient();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
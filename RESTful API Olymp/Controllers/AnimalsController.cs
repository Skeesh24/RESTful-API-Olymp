using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
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

            //var animals = Db.Animals
            //    .Where(x => x.ChippingDateTime.CompareTo(startDateTime) >= 0)
            //    .Where(x => x.ChippingDateTime.CompareTo(endDateTime) < 0)
            //    .Where(x => x.ChipperId == chipperId)
            //    .Where(x => x.ChippingLocationId == chippingLocationId)
            //    .Where(x => x.LifeStatus == lifeStatus)
            //    .Where(x => x.Gender == gender)
            //    .Skip(from)
            //    .Take(size)
            //    .OrderBy(x => x.Id)
            //    .ToList();


            var vm = new AnimalViewModel();

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


        public AcceptedResult AnimalsPut([FromBody] AnimalPostViewModel newAnimal)
        {
            Db.Animals.Remove(Db?.Animals?.Where(x => x.Id == 1)?.FirstOrDefault());
            Db.Animals.Add(new AnimalEntity
            {
                Id = 1,
                Weight = newAnimal.Weight,
                Height = newAnimal.Height,
                Length = newAnimal.Length,
                Gender = newAnimal.Gender,
                ChipperId = newAnimal.ChipperId,
                ChippingLocationId = newAnimal.ChippingLocationId,
            });
            Db.SaveChanges();

            return Accepted();
        }


        [HttpPost]
        public IActionResult AnimalsPost([FromBody] AnimalPostViewModel animal)
        {
            var newid = Db.Animals.ToList().Count + 1;

            Db.Animals.Add(new AnimalEntity
            {
                Id = newid,
                AnimalTypes = animal.AnimalTypes,
                Weight = animal.Weight,
                Length = animal.Length,
                Height = animal.Height,
                Gender = animal.Gender,
                ChipperId = animal.ChipperId,
                ChippingLocationId = animal.ChippingLocationId
            });
            Db.SaveChanges();

            return RedirectToAction($"animals?animalId={newid}");
        }

    }
}

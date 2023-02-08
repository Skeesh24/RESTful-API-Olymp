using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;

namespace RESTful_API_Olymp.Controllers
{
    public class AnimalController : Controller
    {
        private readonly DataContext? Db;

        public AnimalController(DataContext? context)
        {
            Db = context;
        }

        [HttpGet("animal/animals/search")]
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




        [HttpGet/*("animals/")*/]
        public IActionResult Animals(long animalid)
        {
            ViewBag.Title = "Питомец";
            var vm = new AnimalViewModel { Animals = Db.Animals.Where(x => x.Id == animalid).ToList() };
            return View(vm);
        }




        [HttpPost]
        public IActionResult AnimalPost([FromBody] AnimalPostViewModel animal)
        {
            long newid = Db.Animals.ToList().Count + 1;

            if (animal == null)
            {
                return BadRequest();
            }

            Db.Animals.Add(new AnimalEntity
            {
                AnimalTypes = animal.AnimalTypes,
                Weight = animal.Weight,
                Length = animal.Length,
                Height = animal.Height,
                Gender = animal.Gender,
                ChipperId = animal.chipperId,
                ChippingLocationId = animal.chippingLocationId,
                Id = newid,
            });

            Db.SaveChanges();
            return RedirectToAction($"locationpoint/locations?locationId={newid}");
        }



        [HttpPut]
        public IActionResult AnimalPut(long animalId, [FromBody] AnimalPostViewModel animal)
        {
            var putAnimal = Db?.Animals.Where(x => x.Id == animalId).FirstOrDefault();
            if (putAnimal == null)
            {
                return NotFound();
            }

            if(animal == null)
            {
                return BadRequest();
            }

            Db?.Animals.Remove(putAnimal);
            Db?.Animals.Add(new AnimalEntity
            {
                AnimalTypes = animal.AnimalTypes,
                Weight = animal.Weight,
                Length = animal.Length,
                Height = animal.Height,
                Gender = animal.Gender,
                ChipperId = animal.chipperId,
                ChippingLocationId = animal.chippingLocationId,
                Id = animalId,
            });


            Db?.SaveChanges();
            return AcceptedAtAction("");
        }



        [HttpDelete/*("locations/")*/]
        public NoContentResult AnimalDelete(long animalId)
        {
            var deleteAnimal = Db?.Animals?.Where(x => x.Id == animalId)?.FirstOrDefault();

            if (deleteAnimal == null)
            {
                return NoContent();
            }
            Db?.Animals.Remove(deleteAnimal);


            Db?.SaveChanges();
            return NoContent();
        }
    }
}

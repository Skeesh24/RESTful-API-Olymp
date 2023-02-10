using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;
using RESTful_API_Olymp.Static_Helper;
using System.Text;
using System.Text.Json;

namespace RESTful_API_Olymp.Controllers
{
    public class AnimalController : Controller
    {
        private readonly DataContext? Db;

        public AnimalController(DataContext? context)
        {
            Db = context;
        }



        [HttpGet]
        public IActionResult Animals(long? animalid)
        {
            if (animalid == 0 || animalid == null)
                return BadRequest();

            if (!Helper.Authenticate(Request, Db, out int code) && code != 1)
                return Unauthorized();

            var getAnimal = Db.Animals.Where(x => x.Id == animalid).FirstOrDefault();

            if(getAnimal == null)
                return NotFound();

            // возвращаем новое животное в джейсн
            //Response.Body = SerializeAnimal(getAnimal);
            Response.StatusCode = 200;
            return Json(getAnimal);
        }



        [HttpGet("animal/animals/search")]
        public IActionResult Search(DateTime? startDateTime, DateTime? endDateTime, int chipperId, long chippingLocationId, string lifeStatus, string gender, int from, int size)
        {
            if (!Helper.Authenticate(Request, Db, out int code) && code != 1)
                return Unauthorized();

            if (from < 0 || size <= 0) return BadRequest();

            // чек на нул, т.к. парсер асп даст нул из квери лайна, если не удалось скастить к типу DateTime, здесь все ок
            if(startDateTime == null || endDateTime == null) return BadRequest();

            if(chipperId <= 0 || chippingLocationId <= 0) return BadRequest();

            if(!"ALIVEDEAD".Contains(lifeStatus) || !"MALEFEMALEOTHER".Contains(gender)) return BadRequest();

            var foundedArr =
                Db.Animals
                .Where(x => x.ChippingDateTime.CompareTo(startDateTime.Value) >= 0)
                .Where(x => x.ChippingDateTime.CompareTo(endDateTime.Value) < 0)
                .Where(x => x.ChipperId == chipperId)
                .Where(x => x.ChippingLocationId == chippingLocationId)
                .Where(x => x.LifeStatus == lifeStatus)
                .Where(x => x.Gender == gender)
                .Skip(from)
                .Take(size)
                .OrderBy(x => x.Id)
                .ToArray();


            Response.StatusCode = 200;
            return Json(foundedArr);
        }



        [HttpPost]
        public IActionResult AnimalPost()
        {
            if(!Helper.Authenticate(Request, Db, out int code) && code == 0)
                return Unauthorized();         

            var animal = Helper.DeserializeAnimal(Request);
            if (animal == null)
            {
                return BadRequest();
            }

            if(animal.AnimalTypes == null || animal.AnimalTypes.Length <= 0) return BadRequest();
            if (animal.AnimalTypes.Any(x => x == null || x <= 0)) return BadRequest();
            if (animal.Weight == null || animal.Weight == 0) return BadRequest();
            if (animal.Length == null || animal.Length == 0) return BadRequest();
            if (animal.Height == null || animal.Height == 0) return BadRequest();
            if (animal.Gender == null || !"MALEFEMALEOTHER".Contains(animal.Gender)) return BadRequest();
            if (animal.ChipperId == null || animal.ChipperId <= 0) return BadRequest();
            if (animal.ChippingLocationId == null || animal.ChippingLocationId <= 0) return BadRequest();

            // чек на то, что в таблице типов есть все те типы, что указаны в реквесте
            foreach(var type in Db.Types.ToArray())
            {
                if(!animal.AnimalTypes.All(x => x == type.Id))
                    return NotFound();
            }

            // чек на то, что чиппер айди присутствует в таблице акков
            if(!Db.Accounts.Select(x => x.Id).Contains(animal.ChipperId))
                return NotFound();

            if (!Db.Locations.Any(x => x.locationPointId == animal.ChippingLocationId))
                return NotFound();
    
            // чек массива типов на дубликаты
            foreach(var type in animal.AnimalTypes)
            {
                if (animal.AnimalTypes.Where(x => x == type).Count() > 1)
                    return Conflict();
            }

            animal.LifeStatus = "ALIVE";

            long newid = Db.Animals.ToList().Count + 1;
            var newAnimal = new AnimalEntity
            {
                Id = newid,
                AnimalTypes = animal.AnimalTypes.Select(x => x).ToArray(),
                Weight = animal.Weight,
                Length = animal.Length,
                Height = animal.Height,
                LifeStatus = animal.LifeStatus,
                Gender = animal.Gender,
                ChipperId = animal.ChipperId,
                ChippingLocationId = animal.ChippingLocationId,
                ChippingDateTime = DateTime.UtcNow,
            };
            newAnimal.VisitingLocations = new long[] { animal.ChippingLocationId };

            Db.Animals.Add(newAnimal);


            Response.StatusCode = 201;
            Db.SaveChanges();
            return Json(newAnimal);
        }



        [HttpPut]
        public IActionResult AnimalPut(long? animalId)
        {
            if (animalId <= 0 || animalId == null)
                return BadRequest();

            if (!Helper.Authenticate(Request,Db,out int code) && code == 0)
                return Unauthorized();

            var animal = Helper.DeserializeAnimal(Request);

            var putAnimal = Db?.Animals.Where(x => x.Id == animalId).FirstOrDefault();
            if (putAnimal == null)
            {
                return NotFound();
            }

            if (animal == null)
                return BadRequest();

            if (animal.Weight == null || animal.Weight == 0) 
                return BadRequest();

            if (animal.Length == null || animal.Length == 0) 
                return BadRequest();

            if (animal.Height == null || animal.Height == 0) 
                return BadRequest();

            if (animal.Gender == null || !"MALEFEMALEOTHER".Contains(animal.Gender)) 
                return BadRequest();

            if (animal.ChipperId == null || animal.ChipperId <= 0) 
                return BadRequest();

            if (animal.ChippingLocationId == null || animal.ChippingLocationId <= 0)
                return BadRequest();

            // новая точка чипирования совпадает с первой посещенной точкой локации
            if (animal.ChippingLocationId == putAnimal.VisitingLocations.FirstOrDefault())
                return BadRequest();

            if (!"ALIVEDEAD".Contains(animal.LifeStatus))
                return BadRequest();

            // попытка воскресения :O
            if (putAnimal.LifeStatus == "DEAD" && animal.LifeStatus == "ALIVE")
                return BadRequest();

            // чек на то, что чиппер айди присутствует в таблице акков
            if(!Db.Accounts.Select(x => x.Id).Contains(animal.ChipperId))
                return NotFound();

            if (!Db.Locations.Any(x => x.locationPointId == animal.ChippingLocationId))
                return NotFound();


            if (animal.LifeStatus == "DEAD" && animal.DeathDateTime != null)
                animal.DeathDateTime = DateTime.UtcNow;

            Db?.Animals.Remove(putAnimal);
            var newAnimal = new AnimalEntity
            {
                Id = animalId.Value,
                AnimalTypes = animal.AnimalTypes.Select(x => x).ToArray(),
                LifeStatus = putAnimal.LifeStatus,
                VisitingLocations = putAnimal.VisitingLocations.Append(animal.ChippingLocationId).ToArray(),
                Weight = animal.Weight,
                Length = animal.Length,
                Height = animal.Height,
                Gender = animal.Gender,
                ChipperId = animal.ChipperId,
                ChippingLocationId = animal.ChippingLocationId,
            };

            if(animal.LifeStatus != null)
                newAnimal.LifeStatus = animal.LifeStatus;
            if(animal.DeathDateTime != null)
                newAnimal.DeathDateTime = animal.DeathDateTime;

            Db?.Animals.Add(newAnimal);


            Db?.SaveChanges();
            Response.StatusCode = 200;
            // TODO: уточнить, почему код ответа 200, а не 202
            return Json(newAnimal) ;
        }



        [HttpDelete]
        public IActionResult AnimalDelete(long? animalId)
        {
            if (animalId == 0 || animalId == null)
                return BadRequest();

            if (!Helper.Authenticate(Request, Db, out int code) && code == 0)
                return Unauthorized();


            var deleteAnimal = Db?.Animals?.Where(x => x.Id == animalId)?.FirstOrDefault();

            if (deleteAnimal == null)
                return NotFound();

            // животное покинуло локацию чипирования, при этом есть другие посещенные точки
            if (deleteAnimal.ChippingLocationId != deleteAnimal.VisitingLocations.First() && deleteAnimal.VisitingLocations.Length > 1)
                return BadRequest();

            Db?.Animals.Remove(deleteAnimal);
         

            Db?.SaveChanges();      
            // TODO: уточнить, почему по тз статус код 200, а не 204
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;
using RESTful_API_Olymp.Static_Helper;
using System.Reflection.Metadata.Ecma335;

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

            var animal = Helper.DeserializeJson<AnimalEntity>(Request);
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

            var animal = Helper.DeserializeJson<AnimalEntity>(Request);

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



            var newTypeList = Db?.Animals?.ToList();
            newTypeList?.Remove(deleteAnimal);


            // повторная индексация ключей
            for (var elem = 1; elem <= newTypeList.Count; elem++)
            {
                var updateElem = new AnimalEntity
                {
                    Id = elem,
                    AnimalTypes = newTypeList[elem - 1].AnimalTypes,
                    ChipperId = newTypeList[elem - 1].ChipperId,
                    ChippingDateTime = newTypeList[elem - 1].ChippingDateTime,
                    ChippingLocationId = newTypeList[elem - 1].ChippingLocationId,
                    DeathDateTime = newTypeList[elem - 1].DeathDateTime,
                    Gender = newTypeList[elem - 1].Gender,
                    Height = newTypeList[elem - 1].Height,
                    Length = newTypeList[elem - 1].Length,
                    LifeStatus = newTypeList[elem - 1].LifeStatus,
                    VisitingLocations = newTypeList[elem - 1].VisitingLocations,
                    Weight = newTypeList[elem - 1].Weight
                };
                newTypeList[elem - 1] = updateElem;
            }

            Db?.Animals.RemoveRange(Db?.Animals);
            Db?.Animals.AddRangeAsync(newTypeList);


            Db?.SaveChanges();      
            // TODO: уточнить, почему по тз статус код 200, а не 204
            return Ok();
        }

        // Добавление типа животного к животному
        [Route("animal/{animalId}/types/{typeId}")]
        public IActionResult Animal(long? animalId, long? typeId)
        {
            if (!Helper.Authenticate(Request, Db, out int code) && code != 0)
                return Unauthorized();

            if (animalId == null || animalId <= 0)
                return BadRequest();

            if (typeId == null || typeId <= 0)
                return BadRequest();

            if (Db?.Animals.Where(x => x.Id == animalId).Count() == 0)
                return NotFound();

            if (Db?.Types.Where(x => x.Id == typeId).Count() == 0)
                return NotFound();

            var animal = Db?.Animals.Where(x => x.Id == animalId).FirstOrDefault();
            animal.AnimalTypes = animal.AnimalTypes.Append(typeId.Value).ToArray();


            Db?.SaveChanges();
            Response.StatusCode = 201;
            return Json(animal);
        }


        // Редактирования типа животного у животного
        [Route("animals/{animalId}/types")]
        public IActionResult EditAnimalType(long? animalId)
        {
            if (animalId <= 0 || animalId == null)
                return BadRequest();

            if (!Helper.Authenticate(Request, Db, out int code) && code == 0)
                return Unauthorized();

            var typeVM = Helper.DeserializeJson<OldAndNewTypeViewModel>(Request);

            if (typeVM == null || typeVM.OldTypeId == null || typeVM.NewTypeId == null || typeVM.NewTypeId <= 0 || typeVM.OldTypeId <= 0)
                return BadRequest();

            var animal = Db.Animals.Where(x => x.Id == animalId).FirstOrDefault();

            if (animal == null)
                return NotFound();

            if (Db?.Types.Where(x => x.Id == typeVM.OldTypeId || x.Id == typeVM.NewTypeId).Count() == 0)
                return NotFound();

            if (!animal.AnimalTypes.ToList().Contains(typeVM.OldTypeId.Value))
                return NotFound();

            if (animal.AnimalTypes.ToList().Contains(typeVM.NewTypeId.Value))
                return Conflict();

            if (animal.AnimalTypes.ToList().Contains(typeVM.NewTypeId.Value) && animal.AnimalTypes.ToList().Contains(typeVM.OldTypeId.Value))
                return Conflict();

            // импровизированный срез
            animal.AnimalTypes = animal.AnimalTypes.Where(x => !(x == typeVM.OldTypeId)).ToArray();
            animal.AnimalTypes = animal.AnimalTypes.ToList().Append(typeVM.NewTypeId.Value)/*.OrderBy(x => x)*/.ToArray();


            Db?.SaveChanges();
            Response.StatusCode = 200;
            return Json(animal);
        }


        // Удаление типа животнолго у животного
        [Route("animals/{animalId:long}/types/{typeId:long}")]
        public IActionResult DeleteAnimalType(long? animalId, long? typeId)
        {
            if (!Helper.Authenticate(Request, Db, out int code) && code != 0)
                return Unauthorized();

            if (animalId == null || animalId <= 0)
                return BadRequest();

            if (typeId == null || typeId <= 0)
                return BadRequest();

            var animal = Db?.Animals.Where(x => x.Id == animalId).FirstOrDefault();

            if (animal == null)
                return NotFound();

            // у животного с animalId нет типа с typeId 
            if (!animal.AnimalTypes.Contains(typeId.Value))
                return NotFound();

            // тип животного с typeId не найден
            if (Db?.Types.Where(x => x.Id == typeId).Count() == 0)
                return NotFound();
                
            // У животного только один тип и это тип с typeId
            if (animal.AnimalTypes.Contains(typeId.Value) && animal.AnimalTypes.Length == 1)
                return BadRequest();

            animal.AnimalTypes = animal.AnimalTypes.Where(x => x != typeId).ToArray();


            Db?.SaveChanges();
            Response.StatusCode = 200;
            return Json(animal);
        }
    }
}

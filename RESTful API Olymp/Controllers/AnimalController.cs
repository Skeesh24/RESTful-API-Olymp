using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;
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



        [HttpGet("animal/animals/search")]
        public IActionResult Search(DateTime? startDateTime, DateTime? endDateTime, int chipperId, long chippingLocationId, string lifeStatus, string gender, int from, int size)
        {
            if (!Authenticate(out int code) && code != 1)
                return Unauthorized();

            if (from < 0 || size <= 0) return BadRequest();

            if(startDateTime == null || endDateTime == null) return BadRequest();

            if(chipperId <= 0 || chippingLocationId <= 0) return BadRequest();

            if(!"ALIVEDEAD".Contains(lifeStatus) || !"MALEFEMALEOTHER".Contains(gender)) return BadRequest();
                 
            var vm = new AnimalViewModel
            {
                Animals = Db.Animals
                .Where(x => x.ChippingDateTime.CompareTo(startDateTime) >= 0)
                .Where(x => x.ChippingDateTime.CompareTo(endDateTime) > 0)
                .Where(x => x.ChipperId == chipperId)
                .Where(x => x.ChippingLocationId == chippingLocationId)
                .Where(x => x.LifeStatus == lifeStatus)
                .Where(x => x.Gender == gender)
                .Skip(from)
                .Take(size)
                .OrderBy(x => x.Id)
                .ToList()
            };

            // возвращать джейсн а не хтмл
            return View(vm);
        }



        [HttpGet]
        public IActionResult Animals(long? animalid)
        {
            if (animalid == 0 || animalid == null)
                return BadRequest();

            if (!Authenticate(out int code) || code == 1)
                return Unauthorized();

            var listForShow = Db.Animals.Where(x => x.Id == animalid).ToList();
            
            if(listForShow.Count == 0)
                return NotFound();

            // возвращаем джейсн а не хтмл
            ViewBag.Title = "Питомец";
            var vm = new AnimalViewModel { Animals = listForShow };
            return View(vm);
        }



        [HttpPost]
        public IActionResult AnimalPost()
        {
            if(!Authenticate(out int code) && code == 0)
                return Unauthorized();         

            var animal = getAnimalFromRequestBody(Request);
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

            // протестить. здесь чекаем все типы в бд, на то что пришедший реквест содержит в себе все типы, которые есть в этой таблице
            if(!Db.Types.All(type => animal.AnimalTypes.All(curType => curType == type.Id)))
                return NotFound();

            // акааунт с чиппер айди не найден - это что

            if (!Db.Locations.Any(x => x.locationPointId == animal.ChippingLocationId))
                return NotFound();
    
            foreach(var type in animal.AnimalTypes)
            {
                if (animal.AnimalTypes.Where(x => x == type).Count() > 1)
                    return Conflict();
            }


            long newid = Db.Animals.ToList().Count + 1;
            Db.Animals.Add(new AnimalEntity
            {
                AnimalTypes = animal.AnimalTypes.Select(x => x.Value).ToArray(),
                Weight = animal.Weight.Value,
                Length = animal.Length.Value,
                Height = animal.Height.Value,
                Gender = animal.Gender,
                ChipperId = animal.ChipperId.Value,
                ChippingLocationId = animal.ChippingLocationId.Value,
                Id = newid,
            });


            Db.SaveChanges();
            return StatusCode(201);
        }



        [HttpPut]
        public IActionResult AnimalPut(long animalId)
        {
            if (!Authenticate(out int code) && code == 0)
                return Unauthorized();

            // во вью модели у меня нет поля лайф статус, а по тз я должен его иметь и чекать на валидность
            var animal = getAnimalFromRequestBody(Request);

            var putAnimal = Db?.Animals.Where(x => x.Id == animalId).FirstOrDefault();
            if (putAnimal == null)
            {
                return NotFound();
            }

            if(animal == null)
            {
                return BadRequest();
            }


            if (animal.Weight == null || animal.Weight == 0) return BadRequest();
            if (animal.Length == null || animal.Length == 0) return BadRequest();
            if (animal.Height == null || animal.Height == 0) return BadRequest();
            if (animal.Gender == null || !"MALEFEMALEOTHER".Contains(animal.Gender)) return BadRequest();
            if (animal.ChipperId == null || animal.ChipperId <= 0) return BadRequest();
            if (animal.ChippingLocationId == null || animal.ChippingLocationId <= 0) return BadRequest();
            if (animal.ChippingLocationId == Db.Locations?.FirstOrDefault()?.locationPointId) return BadRequest();


            // аккаунт с чиппер айди не найден - это что

            if (!Db.Locations.Any(x => x.locationPointId == animal.ChippingLocationId))
                return NotFound();

            Db?.Animals.Remove(putAnimal);
            Db?.Animals.Add(new AnimalEntity
            {
                AnimalTypes = animal.AnimalTypes.Select(x => x.Value).ToArray(),
                Name = putAnimal.Name,
                LifeStatus = putAnimal.LifeStatus,
                VisitingLocations = putAnimal.VisitingLocations,
                Weight = animal.Weight.Value,
                Length = animal.Length.Value,
                Height = animal.Height.Value,
                Gender = animal.Gender,
                ChipperId = animal.ChipperId.Value,
                ChippingLocationId = animal.ChippingLocationId.Value,
                Id = animalId,
            });


            Db?.SaveChanges();
            return Accepted();
        }



        [HttpDelete]
        public IActionResult AnimalDelete(long? animalId)
        {
            if (animalId == 0 || animalId == null)
                return BadRequest();

            if (!Authenticate(out int code) && code == 0)
                return Unauthorized();

            // животное покинуло локацию чипирования, при этом есть другие посещенные точки

            var deleteAnimal = Db?.Animals?.Where(x => x.Id == animalId)?.FirstOrDefault();

            if (deleteAnimal == null)
            {
                return NotFound();
            }
            Db?.Animals.Remove(deleteAnimal);


            Db?.SaveChanges();
            return NoContent();
        }



        [NonAction]
        public AnimalPostViewModel getAnimalFromRequestBody(HttpRequest request)
        { 
            return JsonSerializer.Deserialize<AnimalPostViewModel>(GetJSONRequestBody(request.Body));
        }

        [NonAction]
        public bool Authenticate(out int exitCode)
        {
            // exit codes:
            // 0 - success auth, yeah
            // 1 - error
            // 2 - incorrect pass
            // 3 - incorrect email

            string input = Request.Headers["Authorization"];

            if (input != null && input.StartsWith("Basic"))
            {
                var encoder = Encoding.UTF8;

                var userAndPassword = encoder.GetString(Convert.FromBase64String(input.Substring("Basic ".Length).Trim()));

                var index = userAndPassword.IndexOf(":");
                var email = userAndPassword.Substring(0, index);
                var password = userAndPassword[(index + 1)..];
                
                if(!Db.Accounts.Any(x => x.Email == email))
                {
                    exitCode = 3;
                    return false;
                } 

                var accoundPass = Db.Accounts.Where(x => x.Email == email).FirstOrDefault().Password;

                if (password.Equals(accoundPass))
                {
                    exitCode = 0;
                    return true;
                }
                else
                {
                    exitCode = 2;
                    return false;
                }
            }
            else
            {
                exitCode = 1;
                return false;
            }
        }

        [NonAction]
        public static string GetJSONRequestBody(Stream stream)
        {
            var bodyStream = new StreamReader(stream);
            var bodyText = bodyStream.ReadToEndAsync();
            return bodyText.Result;
        }
    }
}

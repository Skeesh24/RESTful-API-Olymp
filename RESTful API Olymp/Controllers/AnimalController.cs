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



        [HttpGet]
        public IActionResult Animals(long animalid)
        {
            if (animalid == 0)
                return BadRequest();

            if (!Authenticate(out int code) || code == 2)
                return Unauthorized();

            var listForShow = Db.Animals.Where(x => x.Id == animalid).ToList();
            
            if(listForShow.Count == 0)
                return NotFound();

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
                ChipperId = animal.ChipperId,
                ChippingLocationId = animal.ChippingLocationId,
                Id = newid,
            });

            Db.SaveChanges();
            return RedirectToAction($"locationpoint/locations?locationId={newid}");
        }



        [HttpPut]
        public IActionResult AnimalPut(long animalId)
        {
            if (!Authenticate(out int code) && code == 0)
                return Unauthorized();

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

            Db?.Animals.Remove(putAnimal);
            Db?.Animals.Add(new AnimalEntity
            {
                AnimalTypes = animal.AnimalTypes,
                Name = putAnimal.Name,
                LifeStatus = putAnimal.LifeStatus,
                VisitingLocations = putAnimal.VisitingLocations,
                Weight = animal.Weight,
                Length = animal.Length,
                Height = animal.Height,
                Gender = animal.Gender,
                ChipperId = animal.ChipperId,
                ChippingLocationId = animal.ChippingLocationId,
                Id = animalId,
            });


            Db?.SaveChanges();
            return AcceptedAtAction("");
        }



        [HttpDelete]
        public IActionResult AnimalDelete(long animalId)
        {
            if (!Authenticate(out int code) && code == 0)
                return Unauthorized();

            var deleteAnimal = Db?.Animals?.Where(x => x.Id == animalId)?.FirstOrDefault();

            if (deleteAnimal == null)
            {
                return NoContent();
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

            string input = Request.Headers["Authorization"];

            if (input != null && input.StartsWith("Basic"))
            {
                var encoder = Encoding.UTF8;

                var userAndPassword = encoder.GetString(Convert.FromBase64String(input.Substring("Basic ".Length).Trim()));

                var index = userAndPassword.IndexOf(":");
                var email = userAndPassword.Substring(0, index);
                var password = userAndPassword[(index + 1)..];

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

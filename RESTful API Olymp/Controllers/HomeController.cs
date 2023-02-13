using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;

namespace RESTful_API_Olymp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext? Db;

        public HomeController(DataContext? context)
        {
            Db = context;
        }


        public IActionResult Index()
        {
            ViewBag.Title = "Главная";
            return View();
        }
<<<<<<< HEAD



        [HttpGet("accounts/{accountId?}")]
        public IActionResult Accounts(long accountId)
        {
            ViewBag.Title = "Профиль";
            var vm = new AccountViewModel { Accounts = Db.Accounts.Where(x => x.Id == accountId).ToList() };
            return View(vm);
        }



        [HttpGet("animals/{animalId?}")]
        public IActionResult Animals(long animalid)
        {
            ViewBag.Title = "Питомец";
            var vm = new AnimalViewModel { Animals = Db.Animals.Where(x => x.Id == animalid).ToList() };
            return View(vm);
        }



        [HttpGet("animals/types")]
        public IActionResult Types(long typeId)
        {
            ViewBag.Title = "Типы";
            var vm = new TypeViewModel { Types = Db.Types.Where(x => x.Id == typeId).ToList() };
            return View(vm);
        }



        [HttpGet]
        public IActionResult Locations(DateTime startDateTime, DateTime endDateTime, int from /*= -1*/, int size/* = -1*/)
        {
            ViewBag.Title = "Места";
            var vm = new LocationsViewModel { 
                Locations = Db.Locations
                .Where(x => x.DateTimeOfVisitLocationPoint.CompareTo(startDateTime) >= 0)
                .Where(x => x.DateTimeOfVisitLocationPoint.CompareTo(endDateTime) < 0)
                .Skip(from)
                .Take(size)
                .OrderBy(x => x.Id)
                .ToList() 
            };

            return View(vm);
        }



        [HttpGet("animals/{animalId:long}/locations")]
        public IActionResult AnimalLocations(long animalId)
        {
            var visLocs = Db?.Animals.Where(x => x.Id == animalId)?.FirstOrDefault()?.VisitingLocations;
            var locations = new LocationEntity[visLocs.Length];

            for(var i = 0; i < visLocs.Length-1; i++)
            {
                locations[i] = Db?.Locations?.Where(x => x.Id == visLocs[i])?.FirstOrDefault();
            }

            return View(locations);
        }



        [HttpGet("locations/{pointId?}")]
        public IActionResult Points(long pointId)
        {
            var vm = new PointViewModel { Points = Db.Points.Where(x => x.Id == pointId).ToList() };
            return View(vm);
        }




        [HttpGet("registration/")]
        public IActionResult Registration()
        {
            ViewBag.Title = "Профиль";
            return View();
        }

        [HttpPost]
        public IActionResult Registration(string firstName, string secondName, string email, string password)
        {
            var newid = Db.Accounts.ToList().Count + 1;
            Db.Accounts.Add(new AccountEntity
            {
                FirstName = firstName,
                LastName = secondName,
                Email = email,
                Password = password,
                Id = newid,
            });
            Db.SaveChanges();

            return Redirect($"accounts?accountid={newid}");
        }

=======
>>>>>>> auth
    }
}

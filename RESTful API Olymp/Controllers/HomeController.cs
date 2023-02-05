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



        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "Главная";
            return View();
        }



        [HttpGet("accounts/")]
        public IActionResult Accounts(long accountId)
        {
            ViewBag.Title = "Профиль";
            var vm = new AccountViewModel { Accounts = Db.Accounts.Where(x => x.Id == accountId).ToList() };
            return View(vm);
        }



        [HttpGet("animals/")]
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



        [HttpGet("locations/")]
        public IActionResult Locations(long pointId)
        {
            ViewBag.Title = "Места";
            var vm = new PointViewModel { Points = Db.Points.Where(x => x.Id == pointId).ToList() };
            return View(vm);
        }



        // //

        // GET /animals/{animalId}/locations

        // //



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

            return Redirect($"account/accounts?accountid={newid}");
        }

    }
}

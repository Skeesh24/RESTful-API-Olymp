using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;
using System.ComponentModel.DataAnnotations;

namespace RESTful_API_Olymp.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext? Db;

        public AccountController(DataContext? context)
        {
            Db = context;
        }

        [HttpGet]
        public IActionResult Accounts(int accountId)
        {
            var vm = new AccountViewModel { Accounts = Db.Accounts.Where(x => x.Id == accountId).ToList() };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(string firstName, string secondName, string email, string password)
        {
            var newid = Db.Accounts.ToList().Count+1;
            Db.Accounts.Add(new AccountEntity
            {
                FirstName = firstName,
                LastName = secondName,
                Email = email,
                Password = password,
                Id = newid,
            }) ;
            Db.SaveChanges();

            return Redirect($"account/accounts?accountid={newid}");
        }


        //[HttpGet]
        //public IActionResult Search(DateTime startDateTime, DateTime endDateTime, int chipperId = -1, long chippingLocationId = -1, string lifeStatus = "", string gender = "", int from = -1, int size = -1)
        //{
        //    var vm = new AnimalViewModel
        //    {
        //        Animals = Db.Animals.
        //        Where(x => x.ChippingDateTime.CompareTo(startDateTime) >= 0)
        //        .ToList()
        //    };

        //    if (chipperId != -1)
        //    {
        //        vm.Animals = vm.Animals.Where(x => x.ChipperId == chipperId).ToList();
        //    }

        //    if (chippingLocationId != -1)
        //    {
        //        vm.Animals = vm.Animals.Where(x => x.ChippingLocationId == chippingLocationId).ToList();
        //    }

        //    if (lifeStatus != "")
        //    {
        //        vm.Animals = vm.Animals.Where(x => x.LifeStatus == lifeStatus).ToList();
        //    }

        //    if (gender != "")
        //    {
        //        vm.Animals = vm.Animals.Where(x => x.Gender == gender).ToList();
        //    }

        //    if (from != -1)
        //    {
        //        vm.Animals = vm.Animals.Skip(from).ToList();
        //    }

        //    if (size != -1)
        //    {
        //        vm.Animals = vm.Animals.Take(size).ToList();
        //    }

        //    vm.Animals = vm.Animals.OrderBy(x => x.Id).ToList();

        //    return View(vm);
        //}


    }
}

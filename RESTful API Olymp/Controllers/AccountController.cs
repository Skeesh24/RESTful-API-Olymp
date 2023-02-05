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


        [HttpGet]
        public IActionResult Search(string firstName = "", string lastName = "", string email = "", int from = -1, int size = -1)
        {
            var vm = new AccountViewModel
            {
                Accounts = Db.Accounts.
                Where(x => x.FirstName == firstName)
                .ToList()
            };

            if (lastName != "")
            {
                vm.Accounts = vm.Accounts.Where(x => x.LastName == lastName).ToList();
            }

            if (email != "")
            {
                vm.Accounts = vm.Accounts.Where(x => x.Email == email).ToList();
            }

            if (from != -1)
            {
                vm.Accounts = vm.Accounts.Skip(from).ToList();
            }

            if (size != -1)
            {
                vm.Accounts = vm.Accounts.Take(size).ToList();
            }

            vm.Accounts = vm.Accounts.OrderBy(x => x.Id).ToList();

            return View(vm);
        }


    }
}

using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;

namespace RESTful_API_Olymp.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext? Db;

        public AccountController(DataContext? context)
        {
            Db = context;
        }


        [HttpGet("account/accounts/search")]
        public IActionResult Search(string firstName = "", string lastName = "", string email = "", int from = -1, int size = -1)
        {
            ViewBag.Title = "Поиск";
            var vm = new AccountViewModel { Accounts = Db?.Accounts.ToList() };

            if (firstName != "" && vm != null)
            {
                vm.Accounts = vm?.Accounts?.Where(x => x.FirstName == firstName).ToList();
            }

            if (lastName != "" && vm != null)
            {
                vm.Accounts = vm?.Accounts?.Where(x => x.LastName == lastName).ToList();
            }

            if (email != "" && vm != null)
            {
                vm.Accounts = vm?.Accounts?.Where(x => x.Email == email).ToList();
            }

            if (from != -1 && vm != null)
            {
                vm.Accounts = vm?.Accounts?.Skip(from).ToList();
            }

            if (size != -1 && vm != null)
            {
                vm.Accounts = vm?.Accounts?.Take(size).ToList();
            }

            if (vm != null)
                vm.Accounts = vm?.Accounts?.OrderBy(x => x.Id).ToList();

            return View(vm);
        }



        [HttpGet/*("accounts/")*/]
        public IActionResult Accounts(long accountId)
        {
            ViewBag.Title = "Профиль";
            var vm = new AccountViewModel { Accounts = Db?.Accounts.Where(x => x.Id == accountId).ToList() };
            return View(vm);
        }


        
    }
}

using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Models;

namespace RESTful_API_Olymp.Controllers
{
    public class AccountsController : Controller
    {
        private readonly DataContext? Db;

        public AccountsController(DataContext? context)
        {
            Db = context;
        }


        [HttpGet]
        public IActionResult Search(string firstName = "", string lastName = "", string email = "", int from = -1, int size = -1)
        {
            var vm = new AccountViewModel { Accounts = Db.Accounts.ToList() };

            if (firstName != "")
            {
                vm.Accounts = vm.Accounts.Where(x => x.FirstName == firstName).ToList();
            }

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

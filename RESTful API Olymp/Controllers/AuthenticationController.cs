using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;

namespace RESTful_API_Olymp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly DataContext? Db;

        public AuthenticationController(DataContext? context)
        {
            Db = context;
        }

        [HttpGet]
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

            return RedirectToAction($"animals/animals?accountid={newid}");
        }

    }
}

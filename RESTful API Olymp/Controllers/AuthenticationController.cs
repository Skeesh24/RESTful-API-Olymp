using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.CodeResults;
using RESTful_API_Olymp.Domain.Entities;
using System.Text;

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
            if (string.IsNullOrEmpty(firstName) || firstName.Split().Length > 0)
                return BadRequest();

            if (string.IsNullOrEmpty(secondName) || secondName.Split().Length > 0)
                return BadRequest();

            if (string.IsNullOrEmpty(email) || secondName.Split().Length > 0)
                return BadRequest();

            if (string.IsNullOrEmpty(password) || password.Split().Length > 0)
                return BadRequest();

            if (Db.Accounts.Any(x => x.Email == email))
                return new ExistedEmailResult();


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
            Response.StatusCode = 201;
            return Redirect($"home/index");
        }



        
    }
}

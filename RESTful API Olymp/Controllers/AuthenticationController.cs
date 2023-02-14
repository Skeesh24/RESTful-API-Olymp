using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.CodeResults;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Static_Helper;
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


        [HttpPost]
        public IActionResult Registration()
        {
            var newAccount = Helper.DeserializeJson<AccountEntity>(Request);

            // TODO: считается ли за отсутствие автооризации ввод неверного пароля или имейла для отработки ифа?
            if (Helper.Authenticate(Request, Db, out int code))
                return new AuthorizedRequestResult();

            if (string.IsNullOrEmpty(newAccount.FirstName) || newAccount.FirstName.Split().Length > 1)
                return BadRequest();

            if (string.IsNullOrEmpty(newAccount.LastName) || newAccount.LastName.Split().Length > 1)
                return BadRequest();

            if (string.IsNullOrEmpty(newAccount.Email) || newAccount.Email.Split().Length > 1)
                return BadRequest();

            if (string.IsNullOrEmpty(newAccount.Password) || newAccount.Password.Split().Length > 1)
                return BadRequest();

            if (Db.Accounts.Any(x => x.Email == newAccount.Email))
                return new ExistedEmailResult();

            var newid = Db.Accounts.ToList().Count + 1;
            Db.Accounts.Add(newAccount);
            newAccount.Id = newid;


            Db.SaveChanges();
            Response.StatusCode = 201;
            return Json(newAccount);
        }
    }
}

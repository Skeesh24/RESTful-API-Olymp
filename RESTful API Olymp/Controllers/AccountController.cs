using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;
using RESTful_API_Olymp.Static_Helper;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace RESTful_API_Olymp.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext? Db;

        public AccountController(DataContext? context)
        {
            Db = context;
        }


        [HttpGet/*("accounts/")*/]
        public IActionResult Accounts(int? accountId)
        {
            if (accountId == null || accountId <= 0)
                return BadRequest();

            // доступно без авторизации, но ругается если try неудачен
            if (!Helper.Authenticate(Request, Db, out int code) && code != 1)
                return Unauthorized();

            var accountGet = Db?.Types.Where(x => x.Id == accountId).FirstOrDefault();
            if (accountGet == null)
                return NotFound();


            Response.StatusCode = 200;
            return Json(accountGet);
        }



        [HttpGet("account/accounts/search")]
        public IActionResult Search(string? firstName, string? lastName, string? email, int from = 0, int size = 10)
        {
            if (!Helper.Authenticate(Request, Db, out int code) && code != 1)
                return Unauthorized();

            var foundAccount = Db?.Accounts.ToArray();
            if (firstName != null) 
                foundAccount = foundAccount?.Where(x => x.FirstName.Contains(firstName.ToLower())).ToArray();

            if (lastName != null)
                foundAccount = foundAccount?.Where(x => x.LastName.Contains(lastName.ToLower())).ToArray();

            if (email != null)
                foundAccount = foundAccount?.Where(x => x.Email.Contains(email.ToLower())).ToArray();

            foundAccount = 
                foundAccount?
                .Skip(from)
                .Take(size)
                .OrderBy(x => x.Id)
                .ToArray();


            Response.StatusCode = 200;
            return Json(foundAccount);
        }



        [HttpPut]
        public IActionResult AccountsPut(int? accountId)
        {
            if (accountId == null || accountId <= 0)
                return BadRequest();

            if (!Helper.Authenticate(Request, Db, out int code) && code == 0)
                return Unauthorized();
                 
            var account = Helper.DeserializeJson<AccountEntity>(Request);
            
            //// проверка, что действия со своим аккаунтом
            //string input = Request.Headers["Authorization"];
            //if (input != null && input.StartsWith("Basic"))
            //{
            //    var emailAndpass = Helper.GetStringFromBase64(input);
            //    // TODO: check
            //    if (emailAndpass.Substring(0, emailAndpass.Split(":")[0].Length) == account.Email)
            //        return StatusCode(403);

            //}

            var putAccount = Db?.Accounts.Where(x => x.Id == accountId).FirstOrDefault();
            if (putAccount == null)
            {
                return NotFound();
            }

            // действие со своим аккаунтом TODO: перепроверить
            if (!(putAccount.Password == account.Password && putAccount.Id == accountId))
                return StatusCode(403);

            if (string.IsNullOrEmpty(account.FirstName) || string.IsNullOrWhiteSpace(account.FirstName))
                return BadRequest();

            if (string.IsNullOrEmpty(account.LastName) || string.IsNullOrWhiteSpace(account.LastName))
                return BadRequest();

            if (string.IsNullOrEmpty(account.Email) || string.IsNullOrWhiteSpace(account.Email)
                || !Regex.IsMatch(account.Email, "(\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*)"))
                return BadRequest();


            // аккаунт с таким имейлом уже существует
            if (Db?.Accounts.Where(x => x.Email == account.Email).Count() > 1)
                return StatusCode(409);
            

            Db?.Accounts.Remove(putAccount);
            var newAccount = new AccountEntity
            {
                Id = accountId.Value,
                Email = account.Email,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Password = account.Password
            };


            Db?.Accounts.Add(newAccount);


            Response.StatusCode = 200;
            Db?.SaveChanges();
            return Json(newAccount);
        }



        [HttpDelete]
        public IActionResult AccountDelete(int? accountId)
        {
            if (accountId == 0 || accountId == null)
                return BadRequest();

            if (!Helper.Authenticate(Request, Db, out int code) && code != 0)
                return Unauthorized();


            var deleteAccount = Db?.Accounts?.Where(x => x.Id == accountId)?.FirstOrDefault();

            // не ошибка, по тз именно такой статус код 
            if (deleteAccount == null)
                return StatusCode(403);

            // проверка на свой акк
            string input = Request.Headers["Authorization"];
            if (input != null && input.StartsWith("Basic"))
            {
                var emailAndpass = Helper.GetStringFromBase64(input);
                // TODO: check = OK
                if (emailAndpass.Substring(0, emailAndpass.Split(":")[0].Length) != deleteAccount.Email)
                    return StatusCode(403);
            }

            // аккаунт связан с животным - TODO: уточнить корректность
            if (Db.Animals.Any(x => x.ChipperId == accountId))
                return BadRequest();


            var newTypeList = Db?.Accounts?.ToList();
            newTypeList?.Remove(deleteAccount);


            // повторная индексация ключей
            for (var elem = 1; elem <= newTypeList.Count; elem++)
            {
                var updateElem = new AccountEntity
                {
                    Id = elem,
                    Email = newTypeList[elem - 1].Email,
                    FirstName = newTypeList[elem - 1].FirstName,
                    LastName = newTypeList[elem - 1].LastName,
                    Password = newTypeList[elem - 1].Password,
                };
                newTypeList[elem - 1] = updateElem;
            }

            Db?.Accounts.RemoveRange(Db?.Accounts);
            Db?.Accounts.AddRangeAsync(newTypeList);


            Db?.SaveChanges();
            // TODO: уточнить, почему по тз статус код 200, а не 204
            return Ok();
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;
using System.Text.Json;

namespace RESTful_API_Olymp.Controllers
{
    public class AnimalTypeController : Controller
    {
        private readonly DataContext? Db;

        public AnimalTypeController(DataContext? context)
        {
            Db = context;
        }


        [HttpGet("animals/types")]
        public IActionResult Types(long typeId)
        {
            ViewBag.Title = "Типы";
            var vm = new TypeViewModel { Types = Db?.Types.Where(x => x.Id == typeId).ToList() };
            return View(vm);
        }



        public TypePostViewModel getTypeFromRequestBody(HttpRequest request)
        {
            return JsonSerializer.Deserialize<TypePostViewModel>(GetJSONRequestBody(request.Body));
        }

        public static string GetJSONRequestBody(Stream stream)
        {
            var bodyStream = new StreamReader(stream);
            var bodyText = bodyStream.ReadToEndAsync();
            return bodyText.Result;
        }



        [HttpPost]
        public IActionResult TypePost()
        {
            var type = getTypeFromRequestBody(Request);

            long newid = Db.Types.ToList().Count + 1;

            if (type == null)
            {
                return BadRequest();
            }

            Db.Types.Add(new TypeEntity
            {
                Type = type.Type,
                Id = newid,
            });

            Db.SaveChanges();
            return RedirectToAction($"locationpoint/locations?locationId={newid}");
        }



        [HttpPut]
        public IActionResult TypePut(long typeId)
        {
            var type = getTypeFromRequestBody(Request);

            var putType = Db?.Types.Where(x => x.Id == typeId).FirstOrDefault();
            if (putType == null)
            {
                return NotFound();
            }

            if (type == null)
            {
                return BadRequest();
            }

            Db?.Types.Remove(putType);
            Db?.Types.Add(new TypeEntity
            {
                Type = putType.Type,
                Id = typeId,
            });


            Db?.SaveChanges();
            return AcceptedAtAction("");
        }



        [HttpDelete]
        public NoContentResult TypeDelete(long typeId)
        {
            var deleteType = Db?.Types?.Where(x => x.Id == typeId)?.FirstOrDefault();

            if (deleteType == null)
            {
                return NoContent();
            }
            Db?.Types.Remove(deleteType);


            Db?.SaveChanges();
            return NoContent();
        }
    }
}

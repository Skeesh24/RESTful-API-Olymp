using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;

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




        [HttpPost]
        public IActionResult TypePost([FromBody] TypePostViewModel type)
        {
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
        public IActionResult TypePut(long typeId, [FromBody] TypePostViewModel type)
        {
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



        [HttpDelete/*("locations/")*/]
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

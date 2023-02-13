using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;
using RESTful_API_Olymp.Static_Helper;
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
        public IActionResult Types(long? typeId)
        {
            if (typeId == null || typeId <= 0)
                return BadRequest();

            if (!Helper.Authenticate(Request, Db, out int code) && code != 1)
                return Unauthorized();

            var type = Db.Types.Where(x => x.Id == typeId).FirstOrDefault();

            if (type == null)
                return NotFound();


            Response.StatusCode = 200;
            return Json(type);
        }



        [HttpPost]
        public IActionResult TypePost()
        {
            if (!Helper.Authenticate(Request, Db, out int code) && code == 0)
                return Unauthorized();

            var type = Helper.DeserializeJson<TypeEntity>(Request);

            if (type == null)
                return NotFound();

            // тип = пуст, пустая строка или пробелы
            if (type.Type == "" || type.Type == null || type.Type.Split()?.FirstOrDefault()?.Count() == 0)
                return BadRequest();

            // тип существует
            if (Db?.Types.Where(x => x.Type == type.Type).Count() > 0)
                return BadRequest();


            long newid = Db.Types.ToList().Count + 1;

            var newType = new TypeEntity
            {
                Type = type.Type,
                Id = newid,
            };
            Db.Types.Add(newType);


            Db.SaveChanges();
            Response.StatusCode = 201;  
            return Json(newType);
        }



        [HttpPut]
        public IActionResult TypePut(long? typeId)
        {
            if (typeId == null || typeId <= 0)
                return BadRequest();

            if (!Helper.Authenticate(Request, Db, out int code) && code == 0)
                return Unauthorized();

            var type = Helper.DeserializeJson<TypeEntity>(Request);
            if (type == null)
                return BadRequest();

            if (type.Type == null || type.Type == "")
                return BadRequest();


            var oldType = Db.Types.Where(x => x.Id == typeId).FirstOrDefault();

            if (oldType == null)
                return NotFound();

            Db?.Types.Remove(oldType);
            var newType = new TypeEntity
            {
                Id = typeId.Value,
                Type = type.Type,
            };

            Db?.Types.Add(newType);


            Db?.SaveChanges();
            Response.StatusCode = 200;
            return Json(newType);
        }



        [HttpDelete]
        public IActionResult TypeDelete(long? typeId)
        {
            if (typeId == null || typeId <= 0)
                return BadRequest();

            if (!Helper.Authenticate(Request, Db, out int code) && code == 0)
                return Unauthorized();

            // существует животное с типом типАйди
            if (Db.Animals.Any(x => x.AnimalTypes.ToList().Contains(typeId.Value)))
                return BadRequest();

            var deleteType = Db?.Types?.Where(x => x.Id == typeId)?.FirstOrDefault();

            if (deleteType == null)
                return NotFound();

            var newTypeList = Db?.Types?.ToList();
            newTypeList?.Remove(deleteType);


            // повторная индексация ключей
            for(var elem = 1; elem <= newTypeList.Count; elem++)
            {
                var updateElem = new TypeEntity { 
                    Id = elem, 
                    Type = newTypeList[elem-1].Type, 
                };
                newTypeList[elem-1] = updateElem;
            }

            Db?.Types.RemoveRange(Db?.Types);
            Db?.Types.AddRangeAsync(newTypeList);
            
            
            Db?.SaveChanges();
            return Ok();
        }
    }
}

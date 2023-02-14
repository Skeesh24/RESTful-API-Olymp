using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;
using RESTful_API_Olymp.Static_Helper;
using System.Text.Json;

namespace RESTful_API_Olymp.Controllers
{
    public class LocationPointController : Controller
    {
        private readonly DataContext? Db;

        public LocationPointController(DataContext? context)
        {
            Db = context;
        }



        [HttpGet]
        public IActionResult Locations(long? pointId)
        {
            if (pointId == null || pointId <= 0)
                return BadRequest();

            if(!Helper.Authenticate(Request, Db, out int code) && code != 1)
                return Unauthorized();

            var pointGet = Db?.Points.Where(x => x.Id == pointId).FirstOrDefault();

            if(pointGet == null)
            {
                return NotFound();
            }


            Response.StatusCode = 200;
            return Json(pointGet);
        }


            
        [HttpPost]
        public IActionResult LocationsPost()
        {
            if (!Helper.Authenticate(Request, Db, out int code) && code != 0)
                return Unauthorized();

            var point = Helper.DeserializeJson<PointEntity>(Request);

            if (point == null)
                return BadRequest();

            if (point.Latitude == null || point.Latitude < -90 || point.Latitude > 90)
                return BadRequest();

            if (point.Longitude == null || point.Longitude < -180 || point.Longitude > 180)
                return BadRequest();

            // уже есть такая точка локации
            if (Db.Points.Where(x => x.Latitude == point.Latitude && x.Longitude == point.Longitude).Count() > 0)
                return Conflict();

            long newid = Db.Points.ToList().Count + 1;
            var newPoint = new PointEntity
            {
                Id = newid,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
            };
            Db.Points.Add(newPoint);


            Db.SaveChanges();
            Response.StatusCode = 200;
            return Json(newPoint);
        }
        


        [HttpPut]
        public IActionResult LocationsPut(long? pointId)
        {
            if(pointId == null || pointId <= 0)
                return BadRequest();

            if (!Helper.Authenticate(Request, Db, out int code) && code != 0)
                return Unauthorized();

            var point = Helper.DeserializeJson<PointEntity>(Request);

            var putPoint = Db?.Points.Where(x => x.Id == pointId).FirstOrDefault();
            if(putPoint == null)
            {
                return NotFound();
            }

            if (point == null)
            {
                return BadRequest();
            }

            if (point.Latitude == null || point.Latitude < -90 || point.Latitude > 90)
                return BadRequest();

            if (point.Longitude == null || point.Longitude < -180 || point.Longitude > 180)
                return BadRequest();

            // уже есть такая точка локации
            if (Db.Points.Where(x => x.Latitude == point.Latitude && x.Longitude == point.Longitude).Count() > 0)
                return Conflict();


            Db?.Points.Remove(putPoint);
            var newPoint = new PointEntity
            {
                Id = pointId.Value,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
            };
            Db?.Points.Add(newPoint);


            Db?.SaveChanges();
            Response.StatusCode = 200;
            return Json(newPoint);
        }
        
        
        
        [HttpDelete]
        public IActionResult LocationsDelete(long? pointId)
        {
            if (pointId == null || pointId <= 0)
                return BadRequest();

            if(!Helper.Authenticate(Request, Db, out int code) && code != 0)
                return Unauthorized();

            var deletePoint = Db?.Points?.Where(x => x.Id == pointId)?.FirstOrDefault();

            // точка локации связана с животным
            if (Db.Animals.Any(x => x.VisitingLocations.Contains(pointId.Value)))
                return BadRequest();

            if(deletePoint == null)
            { 
                return NotFound();
            }
            
            var newPointsList = Db?.Points?.ToList();
            newPointsList?.Remove(deletePoint);

            // повторная индексация ключей
            for (var elem = 1; elem <= newPointsList.Count; elem++)
            {
                var updateElem = new PointEntity
                {
                    Id = elem,
                    Latitude = newPointsList[elem - 1].Latitude,
                    Longitude = newPointsList[elem - 1].Longitude,
                };
                newPointsList[elem - 1] = updateElem;
            }

            Db?.Points.RemoveRange(Db?.Points);
            Db?.Points.AddRangeAsync(newPointsList);


            Db?.SaveChanges();
            return Ok();

        }
    }
}

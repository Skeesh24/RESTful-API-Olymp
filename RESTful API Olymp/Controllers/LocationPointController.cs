using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;

namespace RESTful_API_Olymp.Controllers
{
    public class LocationPointController : Controller
    {
        private readonly DataContext? Db;

        public LocationPointController(DataContext? context)
        {
            Db = context;
        }



        [HttpGet/*("locations/")*/]
        public IActionResult Locations(long pointId)
        {
            ViewBag.Title = "Места";

            var allPoints = Db?.Points.Where(x => x.Id == pointId).ToList();

            if(allPoints == null)
            {
                return NoContent();
            }

            var vm = new PointViewModel { Points = allPoints };
            return View(vm);
        }



        [HttpPost]
        public IActionResult LocationsPost([FromBody] PointPostViewModel point)
        {
            long newid = Db.Points.ToList().Count + 1;
            Db.Points.Add(new PointEntity
            {
                latitude = point.Latitude,
                longitude = point.Longitude,
                Id = newid,
            });

            Db.SaveChanges();
            return RedirectToAction($"locationpoint/locations?locationId={newid}");
        }
        


        [HttpPut]
        public IActionResult LocationsPut(long pointId, [FromBody] PointPostViewModel point)
        {
            var putPoint = Db?.Points.Where(x => x.Id == pointId).FirstOrDefault();
            if(putPoint == null)
            {
                return NotFound();
            }

            if(point == null)
            {
                return BadRequest();
            }

            Db?.Points.Remove(putPoint);
            Db?.Points.Add(new PointEntity
            {
                latitude = point.Latitude,
                longitude = point.Longitude,
                Id = pointId,
            }) ;


            Db?.SaveChanges();
            return AcceptedAtAction("");
        }
        
        
        
        [HttpDelete/*("locations/")*/]
        public NoContentResult LocationsDelete(long pointId)
        {
            var deletePoint = Db?.Points?.Where(x => x.Id == pointId)?.FirstOrDefault();

            if(deletePoint == null)
            { 
                return NoContent();
            }
            Db?.Points.Remove(deletePoint);


            Db?.SaveChanges();
            return NoContent();
        }
    }
}

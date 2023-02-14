using Microsoft.AspNetCore.Mvc;
using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using RESTful_API_Olymp.Models;
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



        public void getPointFromRequestBody(HttpRequest request)
        {
            //return JsonSerializer.Deserialize<PointPostViewModel>(GetStringRequestBody(request.Body));
        }

        public static string GetStringRequestBody(Stream stream)
        {
            var bodyStream = new StreamReader(stream);
            var bodyText = bodyStream.ReadToEndAsync();
            return bodyText.Result;
        }



        [HttpPost]
        public IActionResult LocationsPost()
        {
            //var point = getPointFromRequestBody(Request);

            long newid = Db.Points.ToList().Count + 1;
            //Db.Points.Add(new PointEntity
            //{
            //    latitude = point.Latitude,
            //    longitude = point.Longitude,
            //    Id = newid,
            //});

            Db.SaveChanges();
            return RedirectToAction($"locationpoint/locations?locationId={newid}");
        }
        


        [HttpPut]
        public IActionResult LocationsPut(long pointId)
        {
            //var point = getPointFromRequestBody(Request);

            var putPoint = Db?.Points.Where(x => x.Id == pointId).FirstOrDefault();
            if(putPoint == null)
            {
                return NotFound();
            }

            //if(point == null)
            //{
            //    return BadRequest();
            //}

            Db?.Points.Remove(putPoint);
            //Db?.Points.Add(new PointEntity
            //{
            //    latitude = point.Latitude,
            //    longitude = point.Longitude,
            //    Id = pointId,
            //}) ;


            Db?.SaveChanges();
            return AcceptedAtAction("");
        }
        
        
        
        [HttpDelete]
        public IActionResult LocationsDelete(long pointId)
        {
            var deletePoint = Db?.Points?.Where(x => x.Id == pointId)?.FirstOrDefault();

            if(deletePoint == null)
            { 
                return NotFound();
            }
            Db?.Points.Remove(deletePoint);


            Db?.SaveChanges();
            return NoContent();
        }
    }
}

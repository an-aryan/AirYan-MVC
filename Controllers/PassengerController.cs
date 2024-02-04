using Microsoft.AspNetCore.Mvc;
using FlightBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
namespace FlightBooking.Controllers
{
    public class PassengerController : Controller
    {
        private readonly Ace52024Context db;
        public PassengerController(Ace52024Context _db)
        {
            db = _db;
        }
        public IActionResult ShowPassengers()
        {
            ViewBag.Username = HttpContext.Session.GetString("uname");
            if (ViewBag.Username != "AdminUser")
            {
                return RedirectToAction("Index", "Home");
            }
            List<AryanPassenger> passengers = new List<AryanPassenger>();
            passengers = db.AryanPassengers.ToList();
            return View(passengers);
        }
        public async Task<IActionResult> ShowPassengersApi()
        {
            string passengersApiUrl = $"http://localhost:5190/api/Passenger";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage passengerResponse = await httpClient.GetAsync(passengersApiUrl);
                if (passengerResponse.IsSuccessStatusCode)
                {
                    var passengers = await passengerResponse.Content.ReadFromJsonAsync<IEnumerable<AryanPassenger>>();
                    var pass_list = passengers.ToList();

                    return View(pass_list);
                }
                else
                {
                    return View("error");
                }
            }
        }
        public IActionResult AddPassengers()
        {
            ViewBag.Username = HttpContext.Session.GetString("uname");
            if (ViewBag.Username == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public IActionResult AddPassengers(AryanPassenger passenger)
        {
            db.AryanPassengers.Add(passenger);
            db.SaveChanges();
            HttpContext.Session.SetInt32("pid", passenger.PassengerId);

            return RedirectToAction("FlightSearch", "Booking");
        }
        public async Task<IActionResult> AddPassengersApi()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddPassengersApi(AryanPassenger passenger)
        {
            if (passenger == null)
            {
                return RedirectToAction("LegoApi", "Home");
            }

            string passengersApiUrl = $"http://localhost:5190/api/Passenger";
            using (HttpClient httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(passenger),
              Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(passengersApiUrl, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var Accobj = JsonConvert.DeserializeObject<AryanPassenger>(apiResponse);
                    HttpContext.Session.SetInt32("pidapi", Accobj.PassengerId);
                }
                return RedirectToAction("FlightSearchApi", "Booking");
            }
        }

        



    }
}
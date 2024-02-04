using Microsoft.AspNetCore.Mvc;
using FlightBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Text;
namespace FlightBooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly Ace52024Context db;

        public BookingController(Ace52024Context _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("uname");
            if (ViewBag.Username != "AdminUser")
            {
                return RedirectToAction("Index", "Home");
            }
            var viewModel = new BookingViewModel
            {
                AvailableFlights = db.AryanFlights.Include(x => x.DepartureAirport).Include(x => x.ArrivalAirport).ToList(),
                BookingDetails = new AryanBooking() // Initialize a new booking instance
            };
            return View(viewModel);
        }
        public IActionResult Confirmation(int fid)
        {
            ViewBag.Username = HttpContext.Session.GetString("uname");
            if (ViewBag.Username == null)
            {
                return RedirectToAction("Index", "Home");
            }
            System.Console.WriteLine("THIS IS THE FID" + fid);
            var flight = db.AryanFlights
                    .Include(f => f.DepartureAirport)
                    .Include(f => f.ArrivalAirport)
                    .FirstOrDefault(f => f.FlightId == fid);
            if (flight != null)
            {
                var confirmationViewModel = new ConfirmationViewModel
                {
                    FlightDetails = flight
                };

                return View(confirmationViewModel);
            }
            return RedirectToAction("NoBooking");
        }
        public async Task<IActionResult> ConfirmationApi(int fid)
        {
            string apiBaseUrl = "http://localhost:5190/api";
            string bookingsApiUrl = "http://localhost:5190/api/Booking/";
            string flightsApiUrl = "http://localhost:5190/api/Flight";
            string passengersApiUrl = $"http://localhost:5190/api/Passenger";
            string airportsApiUrl = $"http://localhost:5190/api/Airport";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage FlightResponse = await httpClient.GetAsync($"{flightsApiUrl}/{fid}");
                if (FlightResponse.IsSuccessStatusCode)
                {
                    var flight = await FlightResponse.Content.ReadFromJsonAsync<AryanFlight>();
                    HttpResponseMessage depairResponse = await httpClient.GetAsync($"{airportsApiUrl}/{flight.DepartureAirportId}");
                    var depAirport = await depairResponse.Content.ReadFromJsonAsync<AryanAirport>();
                    HttpResponseMessage arrResponse = await httpClient.GetAsync($"{airportsApiUrl}/{flight.ArrivalAirportId}");
                    var arrAirport = await arrResponse.Content.ReadFromJsonAsync<AryanAirport>();

                    flight.ArrivalAirport = arrAirport;
                    flight.DepartureAirport = depAirport;
                    return View(flight);
                }
                else
                {
                    return View("error");
                }
            }
        }
        [HttpPost]
        public IActionResult Confirmation(ConfirmationViewModel confirmationViewModel)
        {
            AryanPassenger pass = db.AryanPassengers.Find(HttpContext.Session.GetInt32("pid"));
            if (confirmationViewModel == null || confirmationViewModel.FlightDetails == null)
            {
                return RedirectToAction("Index");
            }
            if (confirmationViewModel.FlightDetails.FlightId == 0)
            {
                return RedirectToAction("ShowPassengers", "Passenger");
            }
            AryanBooking booking = new AryanBooking
            {
                PassengerId = pass.PassengerId,
                FlightId = confirmationViewModel.FlightDetails.FlightId,
                BookingDate = DateTime.Now,
            };
            if (booking == null)
            {
                return RedirectToAction("Index", "Booking");
            }
            db.AryanBookings.Add(booking);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = booking.BookingId });
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmationApi(AryanFlight flight)
        {
            int? passid = HttpContext.Session.GetInt32("pidapi");
            if (passid == null || passid.Value == 0)
            {
                return RedirectToAction("NoBooking");
            }
            if (flight == null)
            {
                return RedirectToAction("Index");
            }
            if (flight.FlightId == 0)
            {
                return RedirectToAction("ShowPassengers", "Passenger");
            }
            string bookingsApiUrl = "http://localhost:5190/api/Booking/";
            string flightsApiUrl = "http://localhost:5190/api/Flight";
            string passengersApiUrl = $"http://localhost:5190/api/Passenger";
            string airportsApiUrl = $"http://localhost:5190/api/Airport";

            using (HttpClient httpClient = new HttpClient())
            {

                AryanBooking booking = new AryanBooking
                {
                    PassengerId = passid.Value,
                    FlightId = flight.FlightId,
                    BookingDate = DateTime.Now,
                };
                if (booking == null)
                {
                    return RedirectToAction("Lego", "Home");
                }
                StringContent content = new StringContent(JsonConvert.SerializeObject(booking),
                Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync($"{bookingsApiUrl}", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("THIS IS THE ERROR ------>->->->->->" + apiResponse);
                    var Accobj = JsonConvert.DeserializeObject<AryanBooking>(apiResponse);
                    return RedirectToAction("DetailsApi", new { id = Accobj.BookingId });
                }

            }


        }

        public IActionResult ShowBookings()
        {
            ViewBag.Username = HttpContext.Session.GetString("uname");
            ViewBag.UserId = HttpContext.Session.GetInt32("uid");
            if (ViewBag.Username == "AdminUser")
            {
                var result = db.AryanBookings.Include(x => x.Flight).Include(y => y.Flight.DepartureAirport).Include(z => z.Flight.ArrivalAirport);
                return View(result);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> ShowBookingsApi()
        {
            ViewBag.Username = HttpContext.Session.GetString("uname");
            ViewBag.UserId = HttpContext.Session.GetInt32("uid");

            if (ViewBag.Username == "AdminUser")
            {
                string apiBaseUrl = "http://localhost:5190/api";
                string bookingsApiUrl = "http://localhost:5190/api/Booking/";
                string flightsApiUrl = "http://localhost:5190/api/Flight";
                string passengersApiUrl = $"http://localhost:5190/api/Passenger";
                string airportsApiUrl = $"http://localhost:5190/api/Airport";

                using (HttpClient client = new HttpClient())
                {
                    // Fetch bookings data from the API
                    HttpResponseMessage bookingsResponse = await client.GetAsync(bookingsApiUrl);
                    if (bookingsResponse.IsSuccessStatusCode)
                    {
                        var bookings = await bookingsResponse.Content.ReadFromJsonAsync<IEnumerable<AryanBooking>>();

                        // Fetch flights data from the API
                        HttpResponseMessage flightsResponse = await client.GetAsync(flightsApiUrl);
                        var flights = await flightsResponse.Content.ReadFromJsonAsync<IEnumerable<AryanFlight>>();

                        // Fetch passengers data from the API
                        HttpResponseMessage passengersResponse = await client.GetAsync(passengersApiUrl);
                        var passengers = await passengersResponse.Content.ReadFromJsonAsync<IEnumerable<AryanPassenger>>();

                        HttpResponseMessage airportsResponse = await client.GetAsync(airportsApiUrl);
                        var airports = await airportsResponse.Content.ReadFromJsonAsync<IEnumerable<AryanAirport>>();

                        // Link related entities
                        foreach (var booking in bookings)
                        {
                            booking.Flight = flights.FirstOrDefault(f => f.FlightId == booking.FlightId);
                            booking.Passenger = passengers.FirstOrDefault(p => p.PassengerId == booking.PassengerId);
                            booking.Flight.DepartureAirport = airports.FirstOrDefault(p => p.AirportId == booking.Flight.DepartureAirportId);
                            booking.Flight.ArrivalAirport = airports.FirstOrDefault(p => p.AirportId == booking.Flight.ArrivalAirportId);
                        }

                        return View(bookings);
                    }
                    else
                    {
                        // Handle error
                        return View("Error");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Details(int id)
        {
            ViewBag.Username = HttpContext.Session.GetString("uname");
            if (ViewBag.Username == null)
            {
                return RedirectToAction("Index", "Home");
            }
            AryanBooking? booking = db.AryanBookings
        .Include(b => b.Flight)
            .ThenInclude(f => f.ArrivalAirport)
        .Include(b => b.Flight)
            .ThenInclude(f => f.DepartureAirport)
        .Include(b => b.Passenger)
        .FirstOrDefault(b => b.BookingId == id);
            if (booking != null)
            {
                var detailView = new DetailedViewModel
                {
                    DetailBooking = booking
                };
                return View(detailView);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DetailsApi(int id)
        {
            string bookingApiUrl = $"http://localhost:5190/api/Booking/{id}";
            string flightsApiUrl = "http://localhost:5190/api/Flight";
            string passengersApiUrl = $"http://localhost:5190/api/Passenger";
            string airportsApiUrl = $"http://localhost:5190/api/Airport";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage BookingResponse = await httpClient.GetAsync(bookingApiUrl);
                if (BookingResponse.IsSuccessStatusCode)
                {
                    var booking = await BookingResponse.Content.ReadFromJsonAsync<AryanBooking>();
                    HttpResponseMessage FlightResponse = await httpClient.GetAsync($"{flightsApiUrl}/{booking.FlightId}");
                    var flight = await FlightResponse.Content.ReadFromJsonAsync<AryanFlight>();
                    HttpResponseMessage PassengerResponse = await httpClient.GetAsync($"{passengersApiUrl}/{booking.PassengerId}");
                    var passenger = await PassengerResponse.Content.ReadFromJsonAsync<AryanPassenger>();
                    HttpResponseMessage DepFlightResponse = await httpClient.GetAsync($"{airportsApiUrl}/{flight.DepartureAirportId}");
                    var depAirport = await DepFlightResponse.Content.ReadFromJsonAsync<AryanAirport>();
                    HttpResponseMessage ArrFlightResponse = await httpClient.GetAsync($"{airportsApiUrl}/{flight.ArrivalAirportId}");
                    var arrAirport = await ArrFlightResponse.Content.ReadFromJsonAsync<AryanAirport>();

                    booking.Flight = flight;
                    booking.Passenger = passenger;
                    booking.Flight.DepartureAirport = depAirport;
                    booking.Flight.ArrivalAirport = arrAirport;

                    return View(booking);
                }
                else
                {
                    // Handle error
                    return View("Error");
                }
            }
        }
        [HttpGet]
        public IActionResult CustomerBookings()
        {
            ViewBag.Username = HttpContext.Session.GetString("uname");
            if (ViewBag.Username == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new CustomerBookingsViewModel());
        }
        public async Task<IActionResult> CustomerBookingsApi()
        {
            return View(new CustomerBookingsViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> CustomerBookingsApi(CustomerBookingsViewModel model)
        {
            string bookingApiUrl = $"http://localhost:5190/api/Booking";
            string flightsApiUrl = "http://localhost:5190/api/Flight";
            string passengersApiUrl = $"http://localhost:5190/api/Passenger";
            string airportsApiUrl = $"http://localhost:5190/api/Airport";
            IEnumerable<AryanPassenger> pass = new List<AryanPassenger>();
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage passResponse = await httpClient.GetAsync($"{passengersApiUrl}");
                if (passResponse.IsSuccessStatusCode)
                {
                    pass = await passResponse.Content.ReadFromJsonAsync<IEnumerable<AryanPassenger>>();
                    var req = pass.Where(p => p.Email == model.EmailAddress);
                    if (req.Any())
                    {
                        List<int> matchingPassengerIds = req
                    .Where(p => p.Email == model.EmailAddress)
                    .Select(p => p.PassengerId)
                    .ToList();

                        HttpResponseMessage bookingResponse = await httpClient.GetAsync($"{bookingApiUrl}");
                        var books = await bookingResponse.Content.ReadFromJsonAsync<List<AryanBooking>>();
                        List<AryanBooking> bookings = books.Where(b => matchingPassengerIds.Contains(b.PassengerId)).ToList();
                        HttpResponseMessage flightsResponse = await httpClient.GetAsync(flightsApiUrl);
                        var flights = await flightsResponse.Content.ReadFromJsonAsync<IEnumerable<AryanFlight>>();
                        HttpResponseMessage airportsResponse = await httpClient.GetAsync(airportsApiUrl);
                        var airports = await airportsResponse.Content.ReadFromJsonAsync<IEnumerable<AryanAirport>>();
                        HttpResponseMessage passengersResponse = await httpClient.GetAsync(passengersApiUrl);
                        var passengers = await passengersResponse.Content.ReadFromJsonAsync<IEnumerable<AryanPassenger>>();


                        if (!bookings.Any())
                        {
                            return RedirectToAction("NoBookingApi");
                        }

                        foreach (var booking in bookings)
                        {
                            booking.Flight = flights.FirstOrDefault(f => f.FlightId == booking.FlightId);
                            booking.Passenger = passengers.FirstOrDefault(p => p.PassengerId == booking.PassengerId);
                            booking.Flight.DepartureAirport = airports.FirstOrDefault(p => p.AirportId == booking.Flight.DepartureAirportId);
                            booking.Flight.ArrivalAirport = airports.FirstOrDefault(p => p.AirportId == booking.Flight.ArrivalAirportId);
                        }
                        CustomerBookingsViewModel bk = new CustomerBookingsViewModel
                        {
                            Bookings = bookings
                        };
                        return View(bk);
                    }
                    else
                    {
                        ModelState.AddModelError("EmailAddress", "No passengers found with the specified email address.");
                        return View(model);
                    }
                }
                else
                {
                    return View("error");
                }
            }

        }

        [HttpPost]
        public IActionResult CustomerBookings(CustomerBookingsViewModel model)
        {
            // if (ModelState.IsValid)
            // {
            List<AryanPassenger> passengers = db.AryanPassengers
                .Where(p => p.Email == model.EmailAddress)
                .ToList();

            if (passengers.Any())
            {
                List<int> matchingPassengerIds = passengers
                    .Where(p => p.Email == model.EmailAddress)
                    .Select(p => p.PassengerId)
                    .ToList();

                // Get bookings with matching passenger IDs
                List<AryanBooking> bookings = db.AryanBookings
                   .Where(b => matchingPassengerIds.Contains(b.PassengerId))
                   .Include(b => b.Flight)
                       .ThenInclude(f => f.ArrivalAirport)
                   .Include(b => b.Flight)
                       .ThenInclude(f => f.DepartureAirport)
                   .Include(b => b.Passenger)
                   .ToList();

                if (!bookings.Any())
                {
                    return RedirectToAction("NoBooking");
                }

                CustomerBookingsViewModel bk = new CustomerBookingsViewModel
                {
                    Bookings = bookings
                };
                // TempData["Bookings"] = bookings;

                // Redirect to a different action to display the bookings on a new page
                return View(bk);
            }
            else
            {
                ModelState.AddModelError("EmailAddress", "No passengers found with the specified email address.");
            }
            // }
            // If ModelState is not valid or there are no bookings, return to the view with errors
            return View(model);
        }
        public IActionResult DisplayBookings()
        {
            // Retrieve bookings from TempData
            var bookings = TempData["Bookings"] as List<AryanBooking>;

            if (bookings != null && bookings.Any())
            {
                return View(bookings);
            }

            // Handle case where TempData is empty or invalid
            return RedirectToAction("Index"); // Redirect to the home page or another appropriate action
        }
        public IActionResult CancelBooking(int id)
        {
            ViewBag.Username = HttpContext.Session.GetString("uname");
            if (ViewBag.Username == null)
            {
                return RedirectToAction("Index", "Home");
            }
            AryanBooking? booking = db.AryanBookings
                                    .Include(b => b.Flight)
                                        .ThenInclude(f => f.ArrivalAirport)
                                    .Include(b => b.Flight)
                                        .ThenInclude(f => f.DepartureAirport)
                                    .Include(b => b.Passenger)
                                    .FirstOrDefault(b => b.BookingId == id);
            if (booking != null)
            {
                var detailView = new DetailedViewModel
                {
                    DetailBooking = booking
                };
                return View(detailView);
            }
            return RedirectToAction("ShowBookings");
        }
        public async Task<IActionResult> CancelBookingApi(int id)
        {
            string bookingApiUrl = $"http://localhost:5190/api/Booking/{id}";
            string flightsApiUrl = "http://localhost:5190/api/Flight";
            string passengersApiUrl = $"http://localhost:5190/api/Passenger";
            string airportsApiUrl = $"http://localhost:5190/api/Airport";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage BookingResponse = await httpClient.GetAsync(bookingApiUrl);
                if (BookingResponse.IsSuccessStatusCode)
                {
                    var booking = await BookingResponse.Content.ReadFromJsonAsync<AryanBooking>();
                    HttpResponseMessage FlightResponse = await httpClient.GetAsync($"{flightsApiUrl}/{booking.FlightId}");
                    var flight = await FlightResponse.Content.ReadFromJsonAsync<AryanFlight>();
                    HttpResponseMessage PassengerResponse = await httpClient.GetAsync($"{passengersApiUrl}/{booking.PassengerId}");
                    var passenger = await PassengerResponse.Content.ReadFromJsonAsync<AryanPassenger>();
                    HttpResponseMessage DepFlightResponse = await httpClient.GetAsync($"{airportsApiUrl}/{flight.DepartureAirportId}");
                    var depAirport = await DepFlightResponse.Content.ReadFromJsonAsync<AryanAirport>();
                    HttpResponseMessage ArrFlightResponse = await httpClient.GetAsync($"{airportsApiUrl}/{flight.ArrivalAirportId}");
                    var arrAirport = await ArrFlightResponse.Content.ReadFromJsonAsync<AryanAirport>();

                    booking.Flight = flight;
                    booking.Passenger = passenger;
                    booking.Flight.DepartureAirport = depAirport;
                    booking.Flight.ArrivalAirport = arrAirport;

                    return View(booking);
                }
                else
                {
                    // Handle error
                    return View("Error");
                }
            }
        }
        [HttpPost]
        [ActionName("CancelBookingApi")]
        public async Task<IActionResult> CancelConfirmedApi(int id)
        {
            string bookingApiUrl = $"http://localhost:5190/api/Booking";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage bookingResponse = await httpClient.DeleteAsync($"{bookingApiUrl}/{id}");
                if(bookingResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("LegoApi", "Home");
                }
                else
                {
                    return RedirectToAction("NoBookingApi");
                }
            }
        }
        [HttpPost]
        [ActionName("CancelBooking")]
        public IActionResult CancelConfirmed(int id)
        {
            AryanBooking can = db.AryanBookings.Find(id);
            db.AryanBookings.Remove(can);
            db.SaveChanges();
            return RedirectToAction("CustomerBookings");
        }
        public IActionResult NoBooking()
        {
            return View();
        }
        public IActionResult NoBookingApi()
        {
            return View();
        }
        public IActionResult FlightSearch()
        {
            ViewBag.Username = HttpContext.Session.GetString("uname");
            if (ViewBag.Username == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Retrieve the list of available airports for dropdowns
            var airports = db.AryanAirports.ToList();
            ViewBag.Airports = new SelectList(airports, "AirportId", "City");
            var model = new FlightSearchViewModel();
            return View(model);
        }
        public async Task<IActionResult> FlightSearchApi()
        {
            string bookingApiUrl = $"http://localhost:5190/api/Booking";
            string flightsApiUrl = "http://localhost:5190/api/Flight";
            string passengersApiUrl = $"http://localhost:5190/api/Passenger";
            string airportsApiUrl = $"http://localhost:5190/api/Airport";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage airportResponse = await httpClient.GetAsync($"{airportsApiUrl}");
                if (airportResponse.IsSuccessStatusCode)
                {
                    var airport = await airportResponse.Content.ReadFromJsonAsync<IEnumerable<AryanAirport>>();
                    List<AryanAirport> airports = airport.ToList();
                    ViewBag.Aports = new SelectList(airports, "AirportId", "City");
                    var model = new FlightSearchViewModel();
                    return View(model);
                }
                else
                {
                    return View("error");
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> FlightSearchApi(FlightSearchViewModel model)
        {
            string flightsApiUrl = "http://localhost:5190/api/Flight";
            string airportsApiUrl = $"http://localhost:5190/api/Airport";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage flightsResponse = await httpClient.GetAsync($"{flightsApiUrl}");
                if (flightsResponse.IsSuccessStatusCode)
                {
                    var flight = await flightsResponse.Content.ReadFromJsonAsync<IEnumerable<AryanFlight>>();
                    List<AryanFlight> flights = flight.ToList();
                    var req = flights.Where(f => f.DepartureAirportId == model.SourceAirportId && f.ArrivalAirportId == model.DestinationAirportId).ToList();
                    HttpResponseMessage airportResponse = await httpClient.GetAsync($"{airportsApiUrl}");
                    var airports = await airportResponse.Content.ReadFromJsonAsync<IEnumerable<AryanAirport>>();
                    foreach (var item in req)
                    {
                        item.ArrivalAirport = airports.FirstOrDefault(f => f.AirportId == item.ArrivalAirportId);
                        item.DepartureAirport = airports.FirstOrDefault(f => f.AirportId == item.DepartureAirportId);
                    }
                    return View("SearchResultsApi", req);
                }
                else
                {
                    return View("error");
                }

            }
        }
        [HttpPost]
        public IActionResult FlightSearch(FlightSearchViewModel model)
        {
            // Perform the flight search based on source and destination airport IDs
            var flights = db.AryanFlights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Where(f => f.DepartureAirportId == model.SourceAirportId && f.ArrivalAirportId == model.DestinationAirportId)
                .ToList();

            // You can pass the search results to the view
            return View("SearchResults", flights);
        }
    }


}


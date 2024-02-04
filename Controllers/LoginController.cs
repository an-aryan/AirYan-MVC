using Microsoft.AspNetCore.Mvc;
using FlightBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
namespace FlightBooking.Controllers
{
    public class LoginController : Controller
    {
        private readonly ISession session;
        private readonly Ace52024Context db;
        public LoginController(Ace52024Context _db, IHttpContextAccessor httpContextAccessor)
        {
            db = _db;
            session = httpContextAccessor.HttpContext.Session;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(AryanUser u)
        {
            var result = (from i in db.AryanUsers
                          where i.Username == u.Username && i.Password == u.Password
                          select i).SingleOrDefault();
            if (result != null)
            {
                HttpContext.Session.SetString("uname", result.Username);
                return RedirectToAction("Lego", "Home");
            }
            else
            {
                return View();
            }
        }
        public async Task<IActionResult> LoginApi()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginApi(AryanUser u)
        {
            var apiResponse = await AuthenticateUserWithApi(u);

            if (apiResponse.Success)
            {
                HttpContext.Session.SetString("uname", u.Username);
                return RedirectToAction("LegoApi", "Home");
            }
            else
            {
                return View();
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> LoginApi(AryanUser u)
        // {
        //     string userurl = "http://localhost:5190/api/User";
        //     using (HttpClient httpClient = new HttpClient())
        //     {
        //         HttpResponseMessage userResponse = await httpClient.GetAsync(userurl);
        //         if (userResponse.IsSuccessStatusCode)
        //         {
        //             var users = await userResponse.Content.ReadFromJsonAsync<IEnumerable<AryanUser>>();
        //             var userlist = users.ToList();
        //             var result = (from i in userlist
        //                           where i.Username == u.Username && i.Password == u.Password
        //                           select i).SingleOrDefault();
        //             if (result != null)
        //             {
        //                 HttpContext.Session.SetString("uname", result.Username);
        //                 return RedirectToAction("Lego", "Home");
        //             }
        //             else
        //             {
        //                 return View();
        //             }
        //         }
        //         else
        //         {
        //             return View("error");
        //         }
        //     }
        // }

        private async Task<ApiResponse> AuthenticateUserWithApi(AryanUser user)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsJsonAsync("http://localhost:5190/api/User/authenticate", user);

                    // Clear existing ModelState errors
                    ModelState.Clear();

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                        if (!apiResponse.Success)
                        {
                            ModelState.AddModelError(string.Empty, "Invalid username or password");
                        }

                        return apiResponse;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to authenticate with API");
                    }

                    return new ApiResponse { Success = false };
                }
            }
            catch (Exception ex)
            {
               
                ModelState.AddModelError(string.Empty, "Error communicating with API");
                return new ApiResponse { Success = false };
            }
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(AryanUser acc)
        {
            try
            {
                db.AryanUsers.Add(acc);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException != null && (sqlException.Number == 2627 || sqlException.Number == 2601))
                {
    
                    ModelState.AddModelError("UserName", "The username is already taken. Please choose a different one.");
                }
                else
                {
                    // Handle other database update exceptions
                    ModelState.AddModelError("", "An error occurred while processing your request.");
                }

                return View(acc);
            }
        }
        public IActionResult ViewAllAccounts()
        {
            List<AryanUser> users = db.AryanUsers.ToList();
            return View(users);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FlightBooking.Models;

namespace FlightBooking.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Lego()
    {
        ViewBag.Username = HttpContext.Session.GetString("uname");
        if (ViewBag.Username == null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    public IActionResult LegoApi()
    {
        ViewBag.Username = HttpContext.Session.GetString("uname");
        if (ViewBag.Username == null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BiznesSpoter.Web.Models;
using BiznesSpoter.Web.Services;


namespace BiznesSpoter.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GooglePlacesService _placesService; // Wstrzykujemy serwis
    private readonly GusService _gusService;
    public HomeController(ILogger<HomeController> logger, GooglePlacesService placesService, GusService gusService)
    {
        _logger = logger;
        _placesService = placesService;
        _gusService = gusService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string location, string industry, double? radius)
    {
        if (string.IsNullOrEmpty(location) || string.IsNullOrEmpty(industry))
        {
            return RedirectToAction("Index");
        }

        var results = await _placesService.SearchPlacesAsync(location, industry);

        ViewData["GoogleMapsApiKey"] = "AIzaSyAZv4OqA2d1m5m9oQoUk5D83pRHosgrELA"; 

        return View("mapa", results);
    }
    [HttpGet]
    public async Task<IActionResult> GusStats(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return RedirectToAction("Index");

        var stats = await _gusService.GetCityStatsAsync(city);
        return View(stats);
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

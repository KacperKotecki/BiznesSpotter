using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BiznesSpoter.Web.Models;
using BiznesSpoter.Web.Services; 


namespace BiznesSpoter.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GooglePlacesService _placesService; // Wstrzykujemy serwis

    // Aktualizujemy konstruktor
    public HomeController(ILogger<HomeController> logger, GooglePlacesService placesService)
    {
        _logger = logger;
        _placesService = placesService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string location, string industry, double? radius)
    {
        if (string.IsNullOrEmpty(location) || string.IsNullOrEmpty(industry))
        {
            // Prosta walidacja - powrót na główną jeśli pusto
            return RedirectToAction("Index");
        }

        // Pobieramy dane z Google API
        // Uwaga: Na tym etapie 'radius' z suwaka jest ignorowany w zapytaniu TextSearch,
        // ponieważ TextSearch sam dopasowuje obszar do nazwy lokalizacji.
        // W przyszłości przy 'NearbySearch' radius będzie kluczowy.
        var results = await _placesService.SearchPlacesAsync(location, industry);

        return View(results);
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

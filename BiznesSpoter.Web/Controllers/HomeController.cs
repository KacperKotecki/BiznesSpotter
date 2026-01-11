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
    [HttpGet]
    public async Task<IActionResult> GusStats(string city, string unitId)
    {
        if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(unitId))
            return RedirectToAction("Index");

        // If unitId provided, fetch directly by id
        if (!string.IsNullOrWhiteSpace(unitId))
        {
            var statsById = await _gusService.GetCityStatsByUnitIdAsync(unitId);
            return View(statsById);
        }

        // Otherwise search for candidates
        var candidates = await _gusService.SearchUnitsAsync(city);
        if (candidates == null || candidates.Count == 0)
        {
            var stats = await _gusService.GetCityStatsAsync(city);
            return View(stats);
        }

        if (candidates.Count > 1)
        {
            return View("SelectGusUnit", candidates);
        }

        // single candidate
        var single = candidates.First();
        var statsSingle = await _gusService.GetCityStatsByUnitIdAsync(single.Id ?? string.Empty);
        return View(statsSingle);
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Mapa()
    {
        // Klucz API Google Maps przekazywany do widoku
        ViewData["GoogleMapsApiKey"] = "AIzaSyAZv4OqA2d1m5m9oQoUk5D83pRHosgrELA";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

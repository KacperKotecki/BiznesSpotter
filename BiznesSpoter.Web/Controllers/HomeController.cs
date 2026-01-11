using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BiznesSpoter.Web.Models;
using BiznesSpoter.Web.Services;


namespace BiznesSpoter.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GooglePlacesService _placesService;
    private readonly GusService _gusService;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger,
                          GooglePlacesService placesService,
                          GusService gusService,
                          IConfiguration configuration)
    {
        _logger = logger;
        _placesService = placesService;
        _gusService = gusService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string location, string industry, double radius)
    {
        // 1. Walidacja
        if (string.IsNullOrWhiteSpace(location) || string.IsNullOrWhiteSpace(industry))
        {
            return RedirectToAction("Index");
        }

        // 2. Pobierz koordynaty z Google
        // coords to obiekt klasy Location, a nie Nullable<Location>
        var coords = await _placesService.GetCoordinatesAsync(location);

        if (coords == null)
        {
            return RedirectToAction("Index");
        }

        // 3. Pobierz miejsca z Google
        // UWAGA: Metoda w serwisie nazywa się SearchPlacesAsync, a nie SearchNearbyAsync
        // UWAGA 2: Odwołujemy się do coords.Lat, a nie coords.Value.lat
        var places = await _placesService.SearchPlacesAsync(coords.Lat, coords.Lng, industry, radius);

        // 4. Pobierz dane z GUS ("Silent Search")
        GusStatsViewModel? gusStats = null;
        try
        {
            // Używamy nazwy location wpisanej przez użytkownika
            gusStats = await _gusService.GetStatsForCityNameAsync(location);
        }
        catch
        {
            // Ignorujemy błędy GUS, żeby nie zablokować mapy
        }

        // 5. Zbuduj ViewModel
        var viewModel = new SearchMapViewModel
        {
            Places = places,
            GusStats = gusStats,
            SearchLocation = location,
            SearchIndustry = industry,
            SearchRadius = radius,
            CenterLat = coords.Lat, // Poprawione z .Value.lat
            CenterLng = coords.Lng, // Poprawione z .Value.lng
            GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"]
        };

        ViewData["GoogleMapsApiKey"] = _configuration["GoogleMaps:ApiKey"];
        return View("mapa", viewModel);
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
            var searchVm = new SearchMapViewModel { Places = new List<PlaceResult>(), GusStats = statsById, SearchLocation = city, GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"] };
            ViewData["GoogleMapsApiKey"] = _configuration["GoogleMaps:ApiKey"];
            return View("mapa", searchVm);
        }

        // Otherwise search for candidates
        var candidates = await _gusService.SearchUnitsAsync(city);
        if (candidates == null || candidates.Count == 0)
        {
            var stats = await _gusService.GetCityStatsAsync(city);
            var searchVm = new SearchMapViewModel { Places = new List<PlaceResult>(), GusStats = stats, SearchLocation = city, GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"] };
            ViewData["GoogleMapsApiKey"] = _configuration["GoogleMaps:ApiKey"];
            return View("mapa", searchVm);
        }

        if (candidates.Count > 1)
        {
            return View("SelectGusUnit", candidates);
        }

        // single candidate
        var single = candidates.First();
        var statsSingle = await _gusService.GetCityStatsByUnitIdAsync(single.Id ?? string.Empty);
        var searchVmSingle = new SearchMapViewModel { Places = new List<PlaceResult>(), GusStats = statsSingle, SearchLocation = city, GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"] };
        ViewData["GoogleMapsApiKey"] = _configuration["GoogleMaps:ApiKey"];
        return View("mapa", searchVmSingle);
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

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
    public async Task<IActionResult> Search(string location, string industry, double? radius)
    {
        if (string.IsNullOrEmpty(location) || string.IsNullOrEmpty(industry))
        {
            return RedirectToAction("Index");
        }

        double radiusValue = radius ?? 1.0;

        var results = await _placesService.SearchPlacesAsync(location, industry, radiusValue);

        ViewData["SearchLocation"] = location;
        ViewData["SearchIndustry"] = industry;
        ViewData["SearchRadius"] = radiusValue;

        ViewData["GoogleMapsApiKey"] = _configuration["GoogleMaps:ApiKey"];

        return View("mapa", results);
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

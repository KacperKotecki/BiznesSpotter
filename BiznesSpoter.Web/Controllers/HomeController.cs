using Microsoft.AspNetCore.Mvc;
using BiznesSpoter.Web.Models;
using BiznesSpoter.Web.Services;

namespace BiznesSpoter.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BusinessAnalysisService _analysisService; 
    private readonly GusService _gusService;
    private readonly IConfiguration _configuration;

    public HomeController(
        ILogger<HomeController> logger,
        BusinessAnalysisService analysisService, 
        GusService gusService,
        IConfiguration configuration)
    {
        _logger = logger;
        _analysisService = analysisService;
        _gusService = gusService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string location, string industry, double radius)
    {
        var analysisResult = await _analysisService.AnalyzeBusinessPotentialAsync(
            location, 
            industry, 
            radius);

        if (analysisResult == null)
        {
            return RedirectToAction("Index");
        }

        var viewModel = MapToViewModel(analysisResult);

        return View("mapa", viewModel);
    }

    private SearchMapViewModel MapToViewModel(BusinessAnalysisResult result)
    {
        ViewData["GoogleMapsApiKey"] = _configuration["GoogleMaps:ApiKey"];

        return new SearchMapViewModel
        {
            Places = result.CompetitorPlaces,
            GusStats = result.DemographicStats,
            SearchLocation = result.SearchParameters.Location,
            SearchIndustry = result.SearchParameters.Industry,
            SearchRadius = result.SearchParameters.RadiusMeters,
            CenterLat = result.CenterCoordinates.Lat,
            CenterLng = result.CenterCoordinates.Lng,
            GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"]
        };
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

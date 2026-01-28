using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BiznesSpoter.Web.Models;
using BiznesSpoter.Web.Services;
using BiznesSpoter.Web.Models.ViewModels;

namespace BiznesSpoter.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BusinessAnalysisService _analysisService;
    private readonly IConfiguration _configuration;

    public HomeController(
        ILogger<HomeController> logger,
        BusinessAnalysisService analysisService,
        IConfiguration configuration)
    {
        _logger = logger;
        _analysisService = analysisService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string location, string industry, double radius)
    {
        var analysisResult = await _analysisService.AnalyzeBusinessPotentialAsync(
            location, 
            industry, 
            radius * 1000); // Konwersja km -> metry

        if (analysisResult == null)
        {
            _logger.LogWarning("Analysis failed for location={Location}, industry={Industry}", location, industry);
            return RedirectToAction("Index");
        }

        var viewModel = MapToViewModel(analysisResult);
        return View("mapa", viewModel);
    }

    private SearchMapViewModel MapToViewModel(BusinessAnalysisResult result)
    {
        ViewData["GoogleMapsApiKey"] = _configuration["GoogleMaps:ApiKey"];

        GusStatsViewModel? gusViewModel = null;
        
        if (result.DemographicData != null && result.MarketAnalysis != null)
        {
            gusViewModel = new GusStatsViewModel
            {
                CityName = result.DemographicData.CityName,
                UnitId = result.DemographicData.UnitId,
                Population = result.DemographicData.Population,
                Year = result.DemographicData.DataYear,
                
                PlacesCount = result.MarketAnalysis.CompetitorCount,
                CompetitionIndex = result.MarketAnalysis.CompetitionIndex,
                ResidentsPerBusiness = result.MarketAnalysis.ResidentsPerBusiness,
                MarketStatus = result.MarketAnalysis.Status
            };
        }

        return new SearchMapViewModel
        {
            Places = result.CompetitorPlaces,
            GusStats = gusViewModel,
            SearchLocation = result.SearchParameters.Location,
            SearchIndustry = result.SearchParameters.Industry,
            SearchRadius = result.SearchParameters.RadiusMeters / 1000,
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
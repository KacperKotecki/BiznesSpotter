using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BiznesSpoter.Web.Models.External.GooglePlaces;
using BiznesSpoter.Web.Models.External.Gus;
using BiznesSpoter.Web.Services;
using BiznesSpoter.Web.Models.ViewModels;
using BiznesSpoter.Web.Mappers;

namespace BiznesSpoter.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBusinessAnalysisService _analysisService;
    private readonly IConfiguration _configuration;

    public HomeController(
        ILogger<HomeController> logger,
        IBusinessAnalysisService analysisService,
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
            radius * 1000); 

        if (analysisResult == null)
        {
            _logger.LogWarning("Analysis failed for location={Location}, industry={Industry}", location, industry);
            TempData["ErrorMessage"] = "Nie udało się przeprowadzić analizy dla podanej lokalizacji. Sprawdź poprawność danych.";
            return RedirectToAction("Index");
        }

        var apiKey = _configuration["GoogleMaps:ApiKey"];
        ViewData["GoogleMapsApiKey"] = apiKey; 

        var viewModel = analysisResult.ToViewModel(apiKey);
        return View("mapa", viewModel);
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
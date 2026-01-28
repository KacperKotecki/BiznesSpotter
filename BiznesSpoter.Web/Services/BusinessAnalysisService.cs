using BiznesSpoter.Web.Models;

namespace BiznesSpoter.Web.Services
{
    public class BusinessAnalysisService
    {
        private readonly GooglePlacesService _placesService;
        private readonly GusService _gusService;
        private readonly ILogger<BusinessAnalysisService> _logger;

        public BusinessAnalysisService(
            GooglePlacesService placesService,
            GusService gusService,
            ILogger<BusinessAnalysisService> logger)
        {
            _placesService = placesService;
            _gusService = gusService;
            _logger = logger;
        }

        public async Task<BusinessAnalysisResult?> AnalyzeBusinessPotentialAsync(
            string location, 
            string industry, 
            double radiusMeters)
        {
            if (string.IsNullOrWhiteSpace(location) || string.IsNullOrWhiteSpace(industry))
            {
                _logger.LogWarning("Invalid search parameters: location={Location}, industry={Industry}", 
                    location, industry);
                return null;
            }

            var coordinates = await _placesService.GetCoordinatesAsync(location);
            if (coordinates == null)
            {
                _logger.LogWarning("Could not geocode location: {Location}", location);
                return null;
            }
            var placesTask = _placesService.SearchPlacesAsync(
                coordinates.Lat, 
                coordinates.Lng, 
                industry, 
                radiusMeters);

            var gusTask = _gusService.GetStatsForCityNameAsync(location);

            await Task.WhenAll(placesTask, gusTask);

            var places = await placesTask; 

            GusStatsViewModel? gusStats = null;
            try
            {
                gusStats = await gusTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch GUS statistics for location: {Location}", location);
            }

            if (gusStats != null)
            {
                gusStats.PlacesCount = places.Count;
            }

            return new BusinessAnalysisResult
            {
                CenterCoordinates = coordinates,
                CompetitorPlaces = places,
                DemographicStats = gusStats,
                SearchParameters = new SearchParameters
                {
                    Location = location,
                    Industry = industry,
                    RadiusMeters = radiusMeters
                }
            };
        }
    }

    public class BusinessAnalysisResult
    {
        public Location CenterCoordinates { get; set; } = null!;
        public List<PlaceResult> CompetitorPlaces { get; set; } = new();
        public GusStatsViewModel? DemographicStats { get; set; }
        public SearchParameters SearchParameters { get; set; } = null!;
    }

    public class SearchParameters
    {
        public string Location { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public double RadiusMeters { get; set; }
    }
}
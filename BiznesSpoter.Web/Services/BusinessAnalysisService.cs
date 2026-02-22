using BiznesSpoter.Web.Models.External.GooglePlaces;
using BiznesSpoter.Web.Models.External.Gus;
using BiznesSpoter.Web.Models.Domain;
using BiznesSpoter.Web.Data.Repositories;
using BiznesSpoter.Web.Data.Entities;

namespace BiznesSpoter.Web.Services
{
    public class BusinessAnalysisService : IBusinessAnalysisService
    {
        private readonly IGooglePlacesService _placesService;
        private readonly IGusService _gusService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BusinessAnalysisService> _logger;

        public BusinessAnalysisService(
            IGooglePlacesService placesService,
            IGusService gusService,
            IUnitOfWork unitOfWork,
            ILogger<BusinessAnalysisService> logger)
        {
            _placesService = placesService;
            _gusService = gusService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task<BusinessAnalysisResult?> AnalyzeBusinessPotentialAsync(
            string location, 
            string industry, 
            double radiusMeters,
            string? userId = null)
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
            
            DemographicData? demographicData = null;
            try
            {
                demographicData = await gusTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch GUS statistics for location: {Location}", location);
            }

            MarketAnalysis? marketAnalysis = null;
            if (demographicData?.Population.HasValue == true)
            {
                marketAnalysis = CalculateMarketAnalysis(places.Count, demographicData.Population.Value);
            }

            if (!string.IsNullOrEmpty(userId) && marketAnalysis != null && demographicData != null)
            {
                var history = new SearchHistory
                {
                    UserId = userId,
                    Location = location,
                    Industry = industry,
                    RadiusMeters = radiusMeters,
                    SearchDate = DateTime.UtcNow,
                    CompetitorCount = marketAnalysis.CompetitorCount,
                    Population = demographicData.Population,
                    CompetitionIndex = marketAnalysis.CompetitionIndex
                };

                await _unitOfWork.SearchHistories.AddAsync(history);
                await _unitOfWork.SaveChangesAsync();
            }

            return new BusinessAnalysisResult
            {
                CenterCoordinates = coordinates,
                CompetitorPlaces = places,
                DemographicData = demographicData,
                MarketAnalysis = marketAnalysis,
                SearchParameters = new SearchParameters
                {
                    Location = location,
                    Industry = industry,
                    RadiusMeters = radiusMeters
                }
            };
        }


        private MarketAnalysis CalculateMarketAnalysis(int competitorCount, double population)
        {
            var residentsPerBusiness = competitorCount > 0 
                ? population / competitorCount 
                : population;

            var competitionIndex = (competitorCount / population) * 10000;

            var status = competitionIndex switch
            {
                <= 0.5 => MarketStatus.VeryLowCompetition,
                <= 1.0 => MarketStatus.LowCompetition,
                <= 2.0 => MarketStatus.ModerateCompetition,
                <= 3.0 => MarketStatus.HighCompetition,
                _ => MarketStatus.VerySaturated
            };

            return new MarketAnalysis(
                competitorCount, 
                competitionIndex, 
                residentsPerBusiness, 
                status);
        }
    }

    public class BusinessAnalysisResult
    {
        public Location CenterCoordinates { get; set; } = null!;
        public List<PlaceResult> CompetitorPlaces { get; set; } = new();
        public DemographicData? DemographicData { get; set; }
        public MarketAnalysis? MarketAnalysis { get; set; }
        public SearchParameters SearchParameters { get; set; } = null!;
    }

    public class SearchParameters
    {
        public string Location { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public double RadiusMeters { get; set; }
    }
}
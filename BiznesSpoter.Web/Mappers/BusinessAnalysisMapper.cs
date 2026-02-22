using BiznesSpoter.Web.Models.ViewModels;
using BiznesSpoter.Web.Services;

namespace BiznesSpoter.Web.Mappers;

public static class BusinessAnalysisMapper
{
    public static SearchMapViewModel ToViewModel(this BusinessAnalysisResult result, string? googleMapsApiKey)
    {
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
            GoogleMapsApiKey = googleMapsApiKey
        };
    }
}
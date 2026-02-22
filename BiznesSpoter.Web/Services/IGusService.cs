using BiznesSpoter.Web.Models.Domain;

namespace BiznesSpoter.Web.Services;

public interface IGusService
{
    Task<DemographicData?> GetStatsForCityNameAsync(string cityName);
}
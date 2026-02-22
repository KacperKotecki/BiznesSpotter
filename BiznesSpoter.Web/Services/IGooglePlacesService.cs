using BiznesSpoter.Web.Models.External.GooglePlaces;

namespace BiznesSpoter.Web.Services;

public interface IGooglePlacesService
{
    Task<List<PlaceResult>> SearchPlacesAsync(double lat, double lng, string industry, double radiusKm);
    Task<Location?> GetCoordinatesAsync(string address);
}
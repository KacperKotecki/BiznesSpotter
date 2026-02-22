using System.Globalization;
using System.Text.Json;
using BiznesSpoter.Web.Models.External.GooglePlaces;

namespace BiznesSpoter.Web.Services
{
    public class GooglePlacesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GooglePlacesService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GoogleMaps:ApiKey"] 
                      ?? throw new Exception("Brak klucza GoogleMaps:ApiKey w konfiguracji.");
        }

        public async Task<List<PlaceResult>> SearchPlacesAsync(double lat, double lng, string industry, double radiusKm)
        {

            int radiusMeters = (int)(radiusKm * 1000); 

            var latStr = lat.ToString(CultureInfo.InvariantCulture);
            var lngStr = lng.ToString(CultureInfo.InvariantCulture);

            var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?" +
                      $"location={latStr},{lngStr}" +
                      $"&radius={radiusMeters}" +
                      $"&keyword={Uri.EscapeDataString(industry)}" + 
                      $"&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode) return new List<PlaceResult>();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GooglePlacesResponse>(content);
            return result?.Results ?? new List<PlaceResult>();
        }
        public async Task<Location?> GetCoordinatesAsync(string address)
        {
             var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";
             var response = await _httpClient.GetAsync(url);
             if (!response.IsSuccessStatusCode) return null;
             var content = await response.Content.ReadAsStringAsync();
             var result = JsonSerializer.Deserialize<GooglePlacesResponse>(content);
             return result?.Results?.FirstOrDefault()?.Geometry?.Location;
        }
    }
}
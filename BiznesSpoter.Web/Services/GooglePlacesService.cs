using System.Text.Json;
using BiznesSpoter.Web.Models;

namespace BiznesSpoter.Web.Services
{
    public class GooglePlacesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GooglePlacesService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Pobieramy klucz z appsettings.json
            _apiKey = configuration["GoogleMaps:ApiKey"] 
                      ?? throw new Exception("Brak klucza GoogleMaps:ApiKey w konfiguracji.");
        }

        public async Task<List<PlaceResult>> SearchPlacesAsync(string location, string industry)
        {
            // Budujemy zapytanie, np. "fryzjer w Warszawa Mokotów"
            var query = $"{industry} {location}"; 
            var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(query)}&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                return new List<PlaceResult>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GooglePlacesResponse>(content);

            return result?.Results ?? new List<PlaceResult>();
        }
    }
}
using System.Text.Json;
using BiznesSpoter.Web.Models;

namespace BiznesSpoter.Web.Services
{
    public class GusService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://bdl.stat.gov.pl/api/v1";

        public GusService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GusStatsViewModel?> GetCityStatsAsync(string cityName)
        {
            // 1. Znajdź ID miasta (jednostki terytorialnej)
            // Szukamy na poziomie gmin (level=6) - to zazwyczaj miasta/gminy
            var searchUrl = $"{BaseUrl}/units/search?name={Uri.EscapeDataString(cityName)}&level=6&format=json";
            
            var searchResponse = await _httpClient.GetAsync(searchUrl);
            if (!searchResponse.IsSuccessStatusCode) return null;

            var searchContent = await searchResponse.Content.ReadAsStringAsync();
            var searchData = JsonSerializer.Deserialize<GusSearchResponse>(searchContent);
            
            var unit = searchData?.Results?.FirstOrDefault();
            if (unit == null || unit.Id == null) return null;

            // 2. Pobierz dane o ludności dla tego ID
            // Zmienna 72305 = Ludność ogółem (kategoria P2137)
            var statsUrl = $"{BaseUrl}/data/by-unit/{unit.Id}?var-id=72305&format=json&page-size=1";

            var statsResponse = await _httpClient.GetAsync(statsUrl);
            if (!statsResponse.IsSuccessStatusCode) return null;

            var statsContent = await statsResponse.Content.ReadAsStringAsync();
            var statsData = JsonSerializer.Deserialize<GusDataDataResponse>(statsContent);

            // Wyciągamy ostatnią dostępną wartość
            var latestValue = statsData?.Results?.FirstOrDefault()?.Values?.OrderByDescending(x => x.Year).FirstOrDefault();

            if (latestValue == null) return null;

            return new GusStatsViewModel
            {
                CityName = unit.Name,
                Population = latestValue.Val,
                Year = latestValue.Year
            };
        }
    }
}
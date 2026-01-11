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

        // Szuka jednostek po nazwie (zwraca listę kandydatów)
        public async Task<List<GusUnit>> SearchUnitsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return new List<GusUnit>();
            var url = $"{BaseUrl}/units/search?name={Uri.EscapeDataString(name)}&format=json&page-size=100";
            var resp = await _httpClient.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return new List<GusUnit>();
            var content = await resp.Content.ReadAsStringAsync();
            try
            {
                var data = JsonSerializer.Deserialize<GusSearchResponse>(content);
                return data?.Results ?? new List<GusUnit>();
            }
            catch
            {
                return new List<GusUnit>();
            }
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

        // Pobierz statystyki po identyfikatorze jednostki
        public async Task<GusStatsViewModel?> GetCityStatsByUnitIdAsync(string unitId)
        {
            if (string.IsNullOrWhiteSpace(unitId)) return null;

            // Najpierw pobierz nazwę jednostki (jeśli możliwe)
            string? unitName = unitId;
            try
            {
                var metaUrl = $"{BaseUrl}/units/{Uri.EscapeDataString(unitId)}?format=json";
                var metaResp = await _httpClient.GetAsync(metaUrl);
                if (metaResp.IsSuccessStatusCode)
                {
                    var mcontent = await metaResp.Content.ReadAsStringAsync();
                    var meta = JsonSerializer.Deserialize<GusSearchResponse>(mcontent);
                    // GusSearchResponse may not be the exact shape; try simple parse
                    using var doc = JsonDocument.Parse(mcontent);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("name", out var pname) && pname.ValueKind == JsonValueKind.String)
                        unitName = pname.GetString();
                }
            }
            catch
            {
                // ignore
            }

            // Spróbuj pobrać dane (ludność) przez /data/by-unit
            var dataUrl = $"{BaseUrl}/data/by-unit/{Uri.EscapeDataString(unitId)}?var-id=72305&format=json&page-size=1";
            var dataResp = await _httpClient.GetAsync(dataUrl);
            if (dataResp.IsSuccessStatusCode)
            {
                var content = await dataResp.Content.ReadAsStringAsync();
                try
                {
                    var d = JsonSerializer.Deserialize<GusDataDataResponse>(content);
                    var latest = d?.Results?.FirstOrDefault()?.Values?.OrderByDescending(v => v.Year).FirstOrDefault();
                    if (latest != null)
                    {
                        return new GusStatsViewModel { CityName = unitName, UnitId = unitId, Population = latest.Val, Year = latest.Year };
                    }
                }
                catch
                {
                    // fallthrough to try localities
                }
            }

            // Jeśli powyżej nic nie dało, spróbuj /data/localities/by-unit (dla miejscowości statystycznych)
            var dataUrl2 = $"{BaseUrl}/data/localities/by-unit/{Uri.EscapeDataString(unitId)}?var-id=72305&format=json&page-size=1";
            var dataResp2 = await _httpClient.GetAsync(dataUrl2);
            if (dataResp2.IsSuccessStatusCode)
            {
                var content2 = await dataResp2.Content.ReadAsStringAsync();
                try
                {
                    var d2 = JsonSerializer.Deserialize<GusDataDataResponse>(content2);
                    var latest2 = d2?.Results?.FirstOrDefault()?.Values?.OrderByDescending(v => v.Year).FirstOrDefault();
                    if (latest2 != null)
                    {
                        return new GusStatsViewModel { CityName = unitName, UnitId = unitId, Population = latest2.Val, Year = latest2.Year };
                    }
                }
                catch
                {
                    // nothing
                }
            }

            return null;
        }
    }
}
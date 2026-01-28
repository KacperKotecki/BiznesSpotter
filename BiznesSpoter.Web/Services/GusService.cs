using System.Text.Json;
using BiznesSpoter.Web.Models.Domain;
using BiznesSpoter.Web.Models.Gus;

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

        public async Task<DemographicData?> GetStatsForCityNameAsync(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                return null;

            var units = await SearchUnitsAsync(cityName);
            if (units == null || !units.Any())
                return null;

            var bestUnit = units.First();
            if (bestUnit.Id == null) return null;

            return await GetCityStatsByUnitIdAsync(bestUnit.Id);
        }

        private async Task<DemographicData?> GetCityStatsByUnitIdAsync(string unitId)
        {
            if (string.IsNullOrWhiteSpace(unitId)) return null;

            string unitName = unitId;
            try
            {
                var metaUrl = $"{BaseUrl}/units/{Uri.EscapeDataString(unitId)}?format=json";
                var metaResp = await _httpClient.GetAsync(metaUrl);
                if (metaResp.IsSuccessStatusCode)
                {
                    var metaContent = await metaResp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(metaContent);
                    if (doc.RootElement.TryGetProperty("name", out var nameEl))
                        unitName = nameEl.GetString() ?? unitId;
                }
            }
            catch { /* ignore */ }

            var dataUrl = $"{BaseUrl}/data/by-unit/{Uri.EscapeDataString(unitId)}?var-id=72305&format=json&page-size=1";
            var dataResp = await _httpClient.GetAsync(dataUrl);
            
            if (!dataResp.IsSuccessStatusCode) return null;

            var dataContent = await dataResp.Content.ReadAsStringAsync();
            var dataResponse = JsonSerializer.Deserialize<GusDataDataResponse>(dataContent);

            var latestValue = dataResponse?.Results?.FirstOrDefault()?.Values?
                .OrderByDescending(v => v.Year)
                .FirstOrDefault();

            if (latestValue == null) return null;

            return new DemographicData
            {
                CityName = unitName,
                UnitId = unitId,
                Population = latestValue.Val,
                DataYear = latestValue.Year ?? "N/A"
            };
        }

        private async Task<List<GusUnit>> SearchUnitsAsync(string name)
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
    }
}
using System.Text.Json.Serialization;

namespace BiznesSpoter.Web.Models
{
    // --- 1. Modele do szukania jednostek (np. ID miasta po nazwie) ---
    public class GusSearchResponse
    {
        [JsonPropertyName("totalRecords")]
        public int TotalRecords { get; set; }

        [JsonPropertyName("results")]
        public List<GusUnit>? Results { get; set; }
    }

    public class GusUnit
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("levelId")] // Poziom: 5=powiat, 6=gmina
        public int LevelId { get; set; }
    }

    // --- 2. Modele do danych statystycznych (Zmienne) ---
    public class GusDataDataResponse
    {
        [JsonPropertyName("totalRecords")]
        public int TotalRecords { get; set; }

        [JsonPropertyName("results")]
        public List<GusDataResult>? Results { get; set; }
    }

    public class GusDataResult
    {
        [JsonPropertyName("id")] 
        public int Id { get; set; }
        
        [JsonPropertyName("values")]
        public List<GusValue>? Values { get; set; }
    }

    public class GusValue
    {
        [JsonPropertyName("year")]
        public string? Year { get; set; }

        [JsonPropertyName("val")]
        public double Val { get; set; }
    }
    
    // --- Model widoku (ViewModel) do wyświetlenia gotowych rezultatów ---
    public class GusStatsViewModel
    {
        public string? CityName { get; set; }
        public double Population { get; set; } // Ludność
        public double Unemployed { get; set; } // Bezrobotni (przykładowo)
        public string? Year { get; set; }
    }
}
using System.Text.Json.Serialization;
using System.Collections.Generic;

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

    // --- 3. Model widoku (ViewModel) - TYLKO JEDNA DEFINICJA ---
    public class GusStatsViewModel
    {
        public string? CityName { get; set; }
        public string? UnitId { get; set; }
        public int? UnitLevel { get; set; }
        public double? Population { get; set; }
        public double? Unemployed { get; set; }
        public string? Year { get; set; }

        // --- NOWE POLA DO ANALIZY NASYCENIA RYNKU ---

        // Liczba biznesów przekazana z Google Places API
        public int PlacesCount { get; set; }

        // WZÓR: (Liczba placówek / Populacja) * 10 000
        // Daje nam to informację: ile firm przypada na 10 tys. mieszkańców
        public double CompetitionIndex =>
            (Population.HasValue && Population.Value > 0)
            ? ((double)PlacesCount / Population.Value) * 10000
            : 0;

        // Logika kolorystyczna
        public string MarketStatusColor => CompetitionIndex switch
        {
            <= 0.5 => "#198754", // Zielony (Rynek wolny)
            <= 1.5 => "#ffc107", // Żółty (Mała konkurencja)
            <= 3.0 => "#fd7e14", // Pomarańczowy (Średnia konkurencja)
            _ => "#dc3545"      // Czerwony (Duża konkurencja)
        };

        // Opis tekstowy do wykresu/wskaźnika
        public string MarketStatusText => CompetitionIndex switch
        {
            <= 0.5 => "Rynek wolny - Bardzo wysoki potencjał",
            <= 1.5 => "Mała konkurencja - Dobra lokalizacja",
            <= 3.0 => "Średnia konkurencja - Rynek stabilny",
            _ => "Duża konkurencja - Rynek nasycony"
        };

        // Dodatkowa statystyka: Ilu mieszkańców przypada na 1 firmę
        public double ResidentsPerBusiness =>
            (PlacesCount > 0 && Population.HasValue)
            ? Population.Value / PlacesCount
            : 0;
    }
}
using BiznesSpoter.Web.Models.Domain;

namespace BiznesSpoter.Web.Models.ViewModels;

public class GusStatsViewModel
{
    public string? CityName { get; set; }
    public string? UnitId { get; set; }
    public double? Population { get; set; }
    public string? Year { get; set; }
    
    public int PlacesCount { get; set; }
    public double CompetitionIndex { get; set; }
    public double ResidentsPerBusiness { get; set; }
    public MarketStatus MarketStatus { get; set; }

    public string MarketStatusText => MarketStatus switch
    {
        MarketStatus.VeryLowCompetition => "Bardzo niska konkurencja",
        MarketStatus.LowCompetition => "Niska konkurencja",
        MarketStatus.ModerateCompetition => "Umiarkowana konkurencja",  
        MarketStatus.HighCompetition => "Wysoka konkurencja",
        MarketStatus.VerySaturated => "Rynek przesycony",             
        _ => "Brak danych"
    };

    public string MarketStatusColor => MarketStatus switch
    {
        MarketStatus.VeryLowCompetition => "#198754",
        MarketStatus.LowCompetition => "#20c997",
        MarketStatus.ModerateCompetition => "#ffc107", 
        MarketStatus.HighCompetition => "#fd7e14",
        MarketStatus.VerySaturated => "#dc3545",       
        _ => "#6c757d"
    };
}
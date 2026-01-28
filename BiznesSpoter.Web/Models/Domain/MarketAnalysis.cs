namespace BiznesSpoter.Web.Models.Domain;

/// <summary>
/// Analiza nasycenia rynku (logika biznesowa)
/// </summary>
public class MarketAnalysis
{
    public int CompetitorCount { get; }
    public double CompetitionIndex { get; }
    public double ResidentsPerBusiness { get; }
    public MarketStatus Status { get; }

    // Konstruktor (umożliwia przypisanie readonly properties)
    public MarketAnalysis(int competitorCount, double competitionIndex, double residentsPerBusiness, MarketStatus status)
    {
        CompetitorCount = competitorCount;
        CompetitionIndex = competitionIndex;
        ResidentsPerBusiness = residentsPerBusiness;
        Status = status;
    }
}

public enum MarketStatus
{
    VeryLowCompetition,      // < 0.5
    LowCompetition,          // 0.5-1.0
    ModerateCompetition,     // 1.0-2.0  ← DODAJ TO!
    HighCompetition,         // 2.0-3.0
    VerySaturated           // > 3.0     ← DODAJ TO!
}
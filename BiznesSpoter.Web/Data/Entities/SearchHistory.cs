namespace BiznesSpoter.Web.Data.Entities;

public class SearchHistory
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public double RadiusMeters { get; set; }
    public DateTime SearchDate { get; set; } = DateTime.UtcNow;
    
    public int CompetitorCount { get; set; }
    public double? Population { get; set; }
    public double? CompetitionIndex { get; set; }
}
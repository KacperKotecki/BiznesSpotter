namespace BiznesSpoter.Web.Models.Domain;

public class DemographicData
{
    public string CityName { get; set; } = string.Empty;
    public string? UnitId { get; set; }
    public int? UnitLevel { get; set; }
    public double? Population { get; set; }
    public double? UnemploymentRate { get; set; }
    public string DataYear { get; set; } = string.Empty;
}
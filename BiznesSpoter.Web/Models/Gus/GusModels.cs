using System.Text.Json.Serialization;

namespace BiznesSpoter.Web.Models.Gus;

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

    [JsonPropertyName("levelId")]
    public int LevelId { get; set; }
}

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
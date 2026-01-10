using System.Text.Json.Serialization;

namespace BiznesSpoter.Web.Models
{
    public class GooglePlacesResponse
    {
        [JsonPropertyName("results")]
        public List<PlaceResult>? Results { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class PlaceResult
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("user_ratings_total")]
        public int? UserRatingsTotal { get; set; }

        [JsonPropertyName("geometry")]
        public Geometry? Geometry { get; set; }
        
        [JsonPropertyName("business_status")]
        public string? BusinessStatus { get; set; }
    }

    public class Geometry
    {
        [JsonPropertyName("location")]
        public Location? Location { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lng")]
        public double Lng { get; set; }
    }
}
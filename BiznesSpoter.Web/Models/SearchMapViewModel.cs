using BiznesSpoter.Web.Models;

namespace BiznesSpoter.Web.Models
{
    public class SearchMapViewModel
    {
        // Lista miejsc z mapy
        public List<PlaceResult>? Places { get; set; } = new();

        // Dane demo z GUS (moze byc null, jesli nie znajdzie miasta)
        public GusStatsViewModel? GusStats { get; set; }

        // Parametry wyszukiwania (oznaczamy jako nullable string?)
        public string? SearchLocation { get; set; }
        public string? SearchIndustry { get; set; }
        public double SearchRadius { get; set; }

        // Srodek mapy (dane konieczne dla JS)
        public double CenterLat { get; set; }
        public double CenterLng { get; set; }

        public string? GoogleMapsApiKey { get; set; }
    }
}
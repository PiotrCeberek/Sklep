using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Projekt.Models
{
    [JsonObject("ExchangeRatesSeries")]
    public class WalutyAPI
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty("Currency")]
        public string Nazwa { get; set; }

        [JsonProperty("Mid")]
        public double Cena { get; set; }
    }
}

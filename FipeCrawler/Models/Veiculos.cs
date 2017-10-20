using Newtonsoft.Json;
using System.Collections.Generic;

namespace FipeCrawler.Models
{
    public partial class Veiculo
    {
        [JsonProperty("fipe_name")]
        public string FipeName { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("fipe_marca")]
        public string FipeMarca { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("marca")]
        public string Marca { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Veiculos
    {
        public static List<Veiculo> FromJson(string json) => JsonConvert.DeserializeObject<List<Veiculo>>(json, VeiculosConverter.Settings);
    }

    public class VeiculosConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
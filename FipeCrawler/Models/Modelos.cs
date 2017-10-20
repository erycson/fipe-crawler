using Newtonsoft.Json;
using System.Collections.Generic;

namespace FipeCrawler.Models
{
    public partial class Modelo
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("fipe_codigo")]
        public string FipeCodigo { get; set; }

        [JsonProperty("fipe_marca")]
        public string FipeMarca { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("marca")]
        public string Marca { get; set; }

        [JsonProperty("veiculo")]
        public string Veiculo { get; set; }
    }

    public partial class Modelos
    {
        public static List<Modelo> FromJson(string json) => JsonConvert.DeserializeObject<List<Modelo>>(json, ModelosConverter.Settings);
    }

    public class ModelosConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
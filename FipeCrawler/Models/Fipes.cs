using Newtonsoft.Json;
using System.Collections.Generic;

namespace FipeCrawler.Models
{
    public partial class Fipe
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("preco")]
        public string Preco { get; set; }

        [JsonProperty("combustivel")]
        public string Combustivel { get; set; }

        [JsonProperty("ano_modelo")]
        public int AnoModelo { get; set; }

        [JsonProperty("fipe_codigo")]
        public string FipeCodigo { get; set; }

        [JsonProperty("marca")]
        public string Marca { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("referencia")]
        public string Referencia { get; set; }

        [JsonProperty("veiculo")]
        public string Veiculo { get; set; }
    }

    public partial class Fipes
    {
        public static Fipe FromJson(string json) => JsonConvert.DeserializeObject<Fipe>(json, FipesConverter.Settings);
    }

    public class FipesConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
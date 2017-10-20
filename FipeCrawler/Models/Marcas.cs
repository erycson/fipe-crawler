using Newtonsoft.Json;
using System.Collections.Generic;

namespace FipeCrawler.Models
{
    public partial class Marca
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fipe_name")]
        public string FipeName { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }
    }

    public partial class Marcas
    {
        public static List<Marca> FromJson(string json) => JsonConvert.DeserializeObject<List<Marca>>(json, MarcasConverter.Settings);
    }

    public class MarcasConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}

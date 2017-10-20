/*
 * Copyright (c) 2017
 * Author: Érycson Nóbrega <egdn2004@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.If not, see<http://www.gnu.org/licenses/>.
 */

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
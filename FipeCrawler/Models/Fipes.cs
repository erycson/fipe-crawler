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
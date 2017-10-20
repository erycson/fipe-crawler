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

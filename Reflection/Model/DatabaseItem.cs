using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Model
{
    public class DatabaseItem
    {
        [JsonProperty("type")]
        public virtual string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}

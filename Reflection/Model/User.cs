using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Model
{
    public class User : DatabaseItem
    {
        [JsonProperty("type")]
        public override string Type { get; set; } = nameof(User);

        public string Name { get; set; }

        [JsonProperty("aadObjectId")]
        public string AadObjectId { get; set; }

        public string BotConversationId { get; set; }

        public string PersonalConversationId { get; set; }
    }
}

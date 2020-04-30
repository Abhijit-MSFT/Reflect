using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Model
{
    public class TaskInfo
    {
        //public int title { get; set; }
        //public int height { get; set; }
        //public int width { get; set; }
        //public int card { get; set; }
        [JsonProperty("action")]
        public string action { get; set; }
    }

    public class TaskModuleSubmitData<T>
    {
        [JsonProperty("commandId")]
        public T commandId { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

}

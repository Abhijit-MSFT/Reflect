using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Reflection.Model
{
    public class TaskInfo
    {
        public string question { get; set; }
        public bool? anonymousflag { get; set; }
        public string postCreateBy { get; set; }
        public DateTime? executionDate { get; set; }
        public DateTime? executionTime { get; set; }
        public DateTime? postDate { get; set; }
        public bool? postSendNow { get; set; }
        public string repeatFrequency { get; set; }
        public string recurssionType { get; set; }
        public string action { get; set; }
        public int? card { get; set; }
    }

    public class TaskModuleSubmitData<T>
    {
        [JsonProperty("commandId")]
        public T commandId { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

}

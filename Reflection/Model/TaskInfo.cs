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
        //public string question { get; set; }
        //public int anonymousflag { get; set; }
        //public int postCreateBy { get; set; }
        //public int postDate { get; set; }
        //public int postSendNow { get; set; }
        //public int repeatFrequency { get; set; }
        public string action { get; set; }
        //public int card { get; set; } 
    }

    public class TaskModuleSubmitData<T>
    {
        [JsonProperty("commandId")]
        public T commandId { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

}

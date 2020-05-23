using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Model
{
    public class RecurssionScreenData
    {
        //get Question, Ref Created By, Ref Created Time, Privacy, Recurssion date & time
        public Guid? RefID { get; set; }
        public string  CreatedBy { get; set; }
        public DateTime RefCreatedDate { get; set; }
        public string Privacy { get; set; }
        public string Question { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public string ExecutionTime { get; set; }
        public string RecurssionType { get; set; }
    }
}

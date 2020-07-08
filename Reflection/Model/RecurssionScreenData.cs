using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Model
{
    public class RecurssionScreenData
    {
        //get Question, Ref Created By, Ref Created Time, Privacy, Recurssion date & time
        /// <summary>
        /// Gets or sets RefID.
        /// </summary>
        public Guid? RefID { get; set; }
        /// <summary>
        /// Gets or sets CreatedBy.
        /// </summary>
        public string  CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets RefCreatedDate.
        /// </summary>
        public DateTime RefCreatedDate { get; set; }
        /// <summary>
        /// Gets or sets Privacy.
        /// </summary>
        public string Privacy { get; set; }
        /// <summary>
        /// Gets or sets Question.
        /// </summary>
        public string Question { get; set; }
        /// <summary>
        /// Gets or sets ExecutionDate.
        /// </summary>
        public DateTime? ExecutionDate { get; set; }
        /// <summary>
        /// Gets or sets ExecutionTime.
        /// </summary>
        public string ExecutionTime { get; set; }
        /// <summary>
        /// Gets or sets RecurssionType.
        /// </summary>
        public string RecurssionType { get; set; }
        /// <summary>
        /// Gets or sets CustomRecurssionTypeValue.
        /// </summary>
        public string CustomRecurssionTypeValue { get; set; }
    }
}

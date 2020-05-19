using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Repositories.RecurssionData
{
    public class RecurssionDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets RecurssionID.
        /// </summary>
        public Guid? RecurssionID { get; set; }

        /// <summary>
        /// Gets or sets ReflectionID.
        /// </summary>
        public Guid? ReflectionID { get; set; }

        /// <summary>
        /// Gets or sets RecursstionType.
        /// </summary>
        public string RecursstionType { get; set; }

        /// <summary>
        /// Gets or sets CreatedDate.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets ExecutionDate.
        /// </summary>
        public DateTime? ExecutionDate { get; set; }

        /// <summary>
        /// Gets or sets ExecutionTime.
        /// </summary>
        public DateTime? ExecutionTime { get; set; }

    }

    //public class RecurssionScreenData
    //{
    //    //get Question, Ref Created By, Ref Created Time, Privacy, Recurssion date & time
    //    public Guid? RefID { get; set; }
    //    public string CreatedBy { get; set; }
    //    public DateTime RefCreatedDate { get; set; }
    //    public string Privacy { get; set; }
    //    public string Question { get; set; }
    //    public DateTime? ExecutionDate { get; set; }
    //    public DateTime? ExecutionTime { get; set; }

    //    public string RecurssionType { get; set; }
    //}
}

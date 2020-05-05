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
}

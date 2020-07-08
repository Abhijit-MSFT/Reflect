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
        public string ExecutionTime { get; set; }

        public DateTime? RecurssionEndDate { get; set; }

        /// <summary>
        /// Gets or sets NextExecutionDate.
        /// </summary>
        public DateTime? NextExecutionDate { get; set; }

        /// <summary>
        /// Gets or sets ReflectionRowKey.
        /// </summary>
        public string ReflectionRowKey { get; set; }

        /// <summary>
        /// Gets or sets QuestionRowKey.
        /// </summary>
        public string QuestionRowKey { get; set; }

        /// <summary>
        /// Gets or sets CustomRecurssionTypeValue.
        /// </summary>
        public string CustomRecurssionTypeValue { get; set; }

    }
}

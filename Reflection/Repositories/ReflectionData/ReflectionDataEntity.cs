using Bogus.DataSets;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Repositories.ReflectionData
{
    public class ReflectionDataEntity :TableEntity
    {
        /// <summary>
        /// Gets or sets reflection Id.
        /// </summary>
        public Guid ReflectionID { get; set; }

        /// <summary>
        /// Gets or sets email Id as created by.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets question.
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets privacy.
        /// </summary>
        public string Privacy { get; set; }

        /// <summary>
        /// Gets or sets execution Date.
        /// </summary>
        public DateTime ExecutionDate { get; set; }

        /// <summary>
        /// Gets or sets execution Time.
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Gets or sets recurring flag.
        /// </summary>
        public string RecurringFlag { get; set; }

        /// <summary>
        /// Gets or sets active flag.
        /// </summary>
        public bool IsActive { get; set; }
    }
}

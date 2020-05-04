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
        /// Gets or sets Reflection creation date.
        /// </summary>
        public DateTime RefCreatedDate { get; set; }

        /// <summary>
        /// Gets or sets question.
        /// </summary>
        public Guid QuestionID { get; set; }

        /// <summary>
        /// Gets or sets privacy.
        /// </summary>
        public string Privacy { get; set; }

        /// <summary>
        /// Gets or sets recurring ID.
        /// </summary>
        public Guid RecurrsionID { get; set; }

        /// <summary>
        /// Gets or sets Message ID.
        /// </summary>
        public Guid MessageID { get; set; }

        /// <summary>
        /// Gets or sets Channel ID.
        /// </summary>
        public Guid ChannelID { get; set; }

        /// <summary>
        /// Gets or sets active flag.
        /// </summary>
        public bool IsActive { get; set; }
    }
}

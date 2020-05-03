using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Repositories.FeedbackData
{
    public class FeedbackDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets Feedback Id.
        /// </summary>
        public Guid FeedbackID { get; set; }
        /// <summary>
        /// Gets or sets reflection Id.
        /// </summary>
        public Guid ReflectionID { get; set; }
        /// <summary>
        /// Gets or sets FeedbackGivenBy.
        /// </summary>
        public Guid FeedbackGivenBy { get; set; }
        /// <summary>
        /// Gets or sets Feedback.
        /// </summary>
        public int Feedback { get; set; }
    }
}

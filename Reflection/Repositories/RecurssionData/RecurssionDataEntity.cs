// <copyright file="RecurssionDataEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.Azure.Cosmos.Table;
using System;

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

        /// <summary>
        /// Gets or sets RecurssionEndDate.
        /// </summary>
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

    }
}

// <copyright file="FeedbackDataEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.Azure.Cosmos.Table;
using System;


namespace Reflection.Repositories.FeedbackData
{
    public class FeedbackDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets Feedback Id.
        /// </summary>
        public Guid FeedbackID { get; set; }

        /// <summary>
        /// Gets or sets full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets reflection Id.
        /// </summary>
        public Guid ReflectionID { get; set; }

        /// <summary>
        /// Gets or sets FeedbackGivenBy.
        /// </summary>        
        public string FeedbackGivenBy { get; set; }

        /// <summary>
        /// Gets or sets Feedback.
        /// </summary>
        public int Feedback { get; set; }
    }
}

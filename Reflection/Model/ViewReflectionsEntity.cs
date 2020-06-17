// <copyright file="ViewReflectionEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Reflection.Repositories.FeedbackData;
using Reflection.Repositories.QuestionsData;
using Reflection.Repositories.ReflectionData;
using System.Collections.Generic;


namespace Reflection.Model
{
    public class ViewReflectionsEntity
    {
        /// <summary>
        /// Gets or sets ReflectionData.
        /// </summary>
        public ReflectionDataEntity ReflectionData { get; set; }

        /// <summary>
        /// Gets or sets FeedbackData.
        /// </summary>
        public Dictionary<int, List<FeedbackDataEntity>> FeedbackData { get; set; }

        /// <summary>
        /// Gets or sets Question.
        /// </summary>
        public QuestionsDataEntity Question { get; set; }
    }
}

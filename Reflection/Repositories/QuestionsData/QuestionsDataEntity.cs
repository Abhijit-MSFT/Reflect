// <copyright file="QuestionDataEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Bogus.DataSets;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Repositories.QuestionsData
{
    public class QuestionsDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets QuestionID.
        /// </summary>
        public Guid? QuestionID { get; set; }

        /// <summary>
        /// Gets or sets Question.
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets CreatedDate.
        /// </summary>
        public DateTime QuestionCreatedDate { get; set; }

        /// <summary>
        /// Gets or sets IsDefaultFlag.
        /// </summary>
        public bool IsDefaultFlag { get; set; }

        /// <summary>
        /// Gets or sets userCreatedBy.
        /// </summary>
        public string CreatedBy { get; set; }


        /// <summary>
        /// Gets or sets user CreatedByEmail.
        /// </summary>
        public string CreatedByEmail { get; set; }

    }
}

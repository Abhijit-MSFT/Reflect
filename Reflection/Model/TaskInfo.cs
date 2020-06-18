// <copyright file="TaskInfo.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Reflection.Model
{
    public class TaskInfo
    {
        /// <summary>
        /// Gets or sets question.
        /// </summary>
        public string question { get; set; }

        /// <summary>
        /// Gets or sets privacy.
        /// </summary>
        public string privacy { get; set; }

        /// <summary>
        /// Gets or sets postCreateBy.
        /// </summary>
        public string postCreateBy { get; set; }

        /// <summary>
        /// Gets or sets postCreatedByEmail.
        /// </summary>
        public string postCreatedByEmail { get; set; }

        /// <summary>
        /// Gets or sets executionDate.
        /// </summary>
        public DateTime executionDate { get; set; }

        /// <summary>
        /// Gets or sets executionTime.
        /// </summary>
        public string executionTime { get; set; }

        /// <summary>
        /// Gets or sets postSendNowFlag.
        /// </summary>
        [DefaultValue(false)]
        public bool postSendNowFlag { get; set; }

        /// <summary>
        /// Gets or sets recurssionType.
        /// </summary>
        public string recurssionType { get; set; }

        /// <summary>
        /// Gets or sets IsActive.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets action.
        /// </summary>
        public string action { get; set; }

        /// <summary>
        /// Gets or sets messageID.
        /// </summary>
        public string messageID { get; set; }

        /// <summary>
        /// Gets or sets channelID.
        /// </summary>
        public string channelID { get; set; }

        /// <summary>
        /// Gets or sets reflectionID.
        /// </summary>
        public Guid? reflectionID { get; set; }

        /// <summary>
        /// Gets or sets questionID.
        /// </summary>
        public Guid? questionID { get; set; }

        /// <summary>
        /// Gets or sets recurssionID.
        /// </summary>
        public Guid? recurssionID { get; set; }

        /// <summary>
        /// Gets or sets reflectionRowKey.
        /// </summary>
        public string reflectionRowKey { get; set; }

        /// <summary>
        /// Gets or sets recurrsionRowKey.
        /// </summary>
        public string recurrsionRowKey { get; set; }

        /// <summary>
        /// Gets or sets questionRowKey.
        /// </summary>
        public string questionRowKey { get; set; }

        /// <summary>
        /// Gets or sets reflectMessageId.
        /// </summary>
        public string reflectMessageId { get; set; }

        /// <summary>
        /// Gets or sets teantId.
        /// </summary>
        public string teantId { get; set; }

        /// <summary>
        /// Gets or sets serviceUrl.
        /// </summary>
        public string serviceUrl { get; set; }

        /// <summary>
        /// Gets or sets feedback.
        /// </summary>
        public int feedback { get; set; }
    }

    public class UserfeedbackInfo
    {
        /// <summary>
        /// Gets or sets feedbackId.
        /// </summary>
        public int feedbackId { get; set; }

        /// <summary>
        /// Gets or sets reflectionId.
        /// </summary>
        public string reflectionId { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets userName.
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// Gets or sets emailId.
        /// </summary>
        public string emailId { get; set; }
    }
        
    public class ReflctionData
    {
        /// <summary>
        /// Gets or sets data.
        /// </summary>
        public Data data { get; set; }
    }

    public class Data
    {
        /// <summary>
        /// Gets or sets URL.
        /// </summary>
        public string URL { get; set; }
    }

    public class TaskModuleActionHelper
    {
        public class AdaptiveCardValue<T>
        {
            /// <summary>
            /// Gets or sets Data.
            /// </summary>
            [JsonProperty("data")]
            public T Data { get; set; }
        }
    }

    public class ActionDetails
    {
        /// <summary>
        /// Gets or sets type.
        /// </summary>
        public string type { get; set; }
    }
    public class TaskModuleActionDetails : ActionDetails
    {
        /// <summary>
        /// Gets or sets URL.
        /// </summary>
        public string URL { get; set; }
    }
}

// <copyright file="PartitionKeyNames.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Reflection.Repositories
{
    /// <summary>
    /// Partition key names used in the table storage.
    /// </summary>
    /// 

    public class PartitionKeyNames
    {
        public static class ReflectionDataTable
        {
            /// <summary>
            /// Table name for Reflection data table
            /// </summary>
            public static readonly string TableName = "ReflectionData";

            /// <summary>
            /// Reflection data partition key name.
            /// </summary>
            public static readonly string ReflectionDataPartition = "ReflectionData";
        }

        public static class QuestionsDataTable
        {
            /// <summary>
            /// Table name for Questions data table
            /// </summary>
            public static readonly string TableName = "QuestionsData";

            /// <summary>
            /// Questions data partition key name.
            /// </summary>
            public static readonly string QuestionsDataPartition = "QuestionsData";
        }

        public static class RecurssionDataTable
        {
            /// <summary>
            /// Table name for Recurssion data table
            /// </summary>
            public static readonly string TableName = "RecurssionData";

            /// <summary>
            /// Recurssion data partition key name.
            /// </summary>
            public static readonly string RecurssionDataPartition = "RecurssionData";
        }

        public static class FeedbackDataTable
        {
            /// <summary>
            /// Table name for Recurssion data table
            /// </summary>
            public static readonly string TableName = "FeedbackData";

            /// <summary>
            /// Recurssion data partition key name.
            /// </summary>
            public static readonly string FeedbackDataPartition = "FeedbackData";
        }
    }
}



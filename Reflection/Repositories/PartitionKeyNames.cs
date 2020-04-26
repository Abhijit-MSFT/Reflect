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
        /// <summary>
        /// Users data table partition key names.
        /// </summary>
        /// 

        public static class UserDataTable
        {
            /// <summary>
            /// Table name for user data table
            /// </summary>
            public static readonly string TableName = "UserData";

            /// <summary>
            /// Users data partition key name.
            /// </summary>
            public static readonly string UserDataPartition = "UserData";
        }
    }
}

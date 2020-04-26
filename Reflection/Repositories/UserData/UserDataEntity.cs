// <copyright file="UserDataEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>


namespace Reflection.Repositories.UserData
{
    using Microsoft.Azure.Cosmos.Table;

    public class UserDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets name.
        /// </summary>
        public string Name { get; set; }

    }
}

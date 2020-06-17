// <copyright file="User.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace Reflection.Model
{
    public class User : DatabaseItem
    {
        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [JsonProperty("type")]
        public override string Type { get; set; } = nameof(User);
    }
}

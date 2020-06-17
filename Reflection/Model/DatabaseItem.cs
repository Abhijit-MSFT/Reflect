// <copyright file="DatabaseItem.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace Reflection.Model
{
    public class DatabaseItem
    {
        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }
}

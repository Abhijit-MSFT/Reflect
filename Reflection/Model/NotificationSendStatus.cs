// <copyright file="NotificationSendStatus.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>


namespace Reflection.Model
{
    public class NotificationSendStatus
    {
        /// <summary>
        /// Gets or sets IsSuccessful.
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets FailureMessage.
        /// </summary>
        public string FailureMessage { get; set; }

        /// <summary>
        /// Gets or sets MessageId.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }
    }
}

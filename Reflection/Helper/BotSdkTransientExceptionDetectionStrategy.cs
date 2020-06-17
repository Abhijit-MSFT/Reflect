// <copyright file="BotSdkTransientExceptionDetectionStrategy.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.Rest;
using System;
using System.Collections.Generic;

namespace Reflection.Helper
{
    public class BotSdkTransientExceptionDetectionStrategy : ITransientErrorDetectionStrategy
    {
        // List of error codes to retry on
        readonly List<int> transientErrorStatusCodes = new List<int>() { 429 };

        public bool IsTransient(Exception ex)
        {
            if (ex.Message.Contains("429"))
                return true;

            if (ex is HttpOperationException httpOperationException)
            {
                return httpOperationException.Response != null &&
                        transientErrorStatusCodes.Contains((int)httpOperationException.Response.StatusCode);
            }

            return false;
        }
    }
}

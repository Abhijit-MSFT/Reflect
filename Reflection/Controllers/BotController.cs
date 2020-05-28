// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace Microsoft.Teams.Samples.HelloWorld.Web.Controllers
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IBot Bot;
        private readonly TelemetryClient _telemetry;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot, TelemetryClient telemetry)
        {
            Adapter = adapter;
            Bot = bot;
            _telemetry = telemetry;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            _telemetry.TrackEvent("PostAsync");
            try
            {
                // Delegate the processing of the HTTP POST to the adapter.
                // The adapter will invoke the bot.
                await Adapter.ProcessAsync(Request, Response, Bot);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }


        }
    }
}

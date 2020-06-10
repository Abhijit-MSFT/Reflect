using Microsoft.ApplicationInsights;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Reflection.Repositories.RecurssionData;
using Reflection.Repositories.ReflectionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reflection.Helper
{
    public class SchedulerHelper : IHostedService
    {
        private Timer _timer;
        private readonly TelemetryClient _telemetry;
        private readonly IConfiguration _configuration;

        public SchedulerHelper(TelemetryClient telemetry, IConfiguration configuration)
        {
            _telemetry = telemetry;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("StartAsync");

            _timer = new Timer(RunJob, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void RunJob(object state)
        {
            _telemetry.TrackEvent("RunJob");
            ChannelAccount channelAccount = new ChannelAccount(_configuration["MicrosoftAppId"]);
            Attachment attachment = new Attachment();
            TeamsChannelData channelData = new TeamsChannelData() { Notification = new NotificationInfo(true) };
            RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
            var recurssionData = await recurssionDataRepository.GetAllRecurssionData();
            foreach (RecurssionDataEntity recurssionDataEntity in recurssionData)
            {
                var reflectionData = await reflectionDataRepository.GetReflectionData(recurssionDataEntity.ReflectionID);
                await ProactiveMessageHelper.SendChannelNotification(channelAccount, reflectionData.ServiceUrl, reflectionData.ChannelID, "", null);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("StopAsync");

            return Task.CompletedTask;
        }
    }
}

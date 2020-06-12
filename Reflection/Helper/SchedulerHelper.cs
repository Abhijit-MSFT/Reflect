using AdaptiveCards;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Reflection.Interfaces;
using Reflection.Model;
using Reflection.Repositories.QuestionsData;
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
        private readonly ICard _cardHelper;
        private readonly IDataBase _dbHelper;

        public SchedulerHelper(TelemetryClient telemetry, IConfiguration configuration, ICard cardHelper, IDataBase dbHelper)
        {
            _telemetry = telemetry;
            _configuration = configuration;
            _cardHelper = cardHelper;
            _dbHelper = dbHelper;
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
            try
            {
                ChannelAccount channelAccount = new ChannelAccount(_configuration["MicrosoftAppId"]);
                Attachment attachment = new Attachment();
                TeamsChannelData channelData = new TeamsChannelData() { Notification = new NotificationInfo(true) };
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                QuestionsDataRepository questiondatarepository = new QuestionsDataRepository(_configuration, _telemetry);

                var recurssionData = await recurssionDataRepository.GetAllRecurssionData();
                foreach (RecurssionDataEntity recurssionDataEntity in recurssionData)
                {
                    var reflectionData = await reflectionDataRepository.GetReflectionData(recurssionDataEntity.ReflectionID);
                    QuestionsDataEntity question = await questiondatarepository.GetQuestionData(reflectionData.QuestionID);
                    TaskInfo taskInfo = new TaskInfo();
                    taskInfo.question = question.Question;
                    taskInfo.postCreateBy = reflectionData.CreatedBy;
                    taskInfo.privacy = reflectionData.Privacy;
                    taskInfo.reflectionID = reflectionData.ReflectionID;
                    var newPostCard = _cardHelper.CreateNewPostCard(taskInfo, 0);
                    Attachment newPostCardAttachment = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = newPostCard
                    };
                    var proactiveNotification = await new ProactiveMessageHelper(_configuration).SendChannelNotification(channelAccount, reflectionData.ServiceUrl, reflectionData.ChannelID, "", newPostCardAttachment);
                    if(proactiveNotification.IsSuccessful && proactiveNotification.MessageId != null)
                    {
                        reflectionData.ReflectMessageId = proactiveNotification.MessageId.Split("=")[1];
                        await _dbHelper.UpdateReflectionMessageIdAsync(reflectionData);
                    }
                    await _dbHelper.UpdateRecurssionDataNextExecutionDateTimeAsync(recurssionDataEntity);
                }
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("StopAsync");

            return Task.CompletedTask;
        }
    }
}

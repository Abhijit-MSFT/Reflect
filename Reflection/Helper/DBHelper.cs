using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Reflection.Model;
using Reflection.Repositories;
using Reflection.Repositories.FeedbackData;
using Reflection.Repositories.QuestionsData;
using Reflection.Repositories.RecurssionData;
using Reflection.Repositories.ReflectionData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Helper
{
    public static class DBHelper
    {

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>

        public static async Task SaveReflectionDataAsync(TaskInfo taskInfo, IConfiguration configuration, ITurnContext<IInvokeActivity> turnContext)
        {
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(configuration);
            
            if (taskInfo != null)
            {
                taskInfo.reflectionID = Guid.NewGuid();
                taskInfo.questionID = Guid.NewGuid();
                taskInfo.recurssionID = Guid.NewGuid();
                var rowKey = Guid.NewGuid();
                
                ReflectionDataEntity reflectEntity = new ReflectionDataEntity
                {
                    ReflectionID = taskInfo.reflectionID,
                    PartitionKey = PartitionKeyNames.ReflectionDataTable.ReflectionDataPartition, // read it from json
                    RowKey = rowKey.ToString(),
                    CreatedBy = taskInfo.postCreateBy,
                    RefCreatedDate = DateTime.Now,
                    QuestionID = taskInfo.questionID,
                    Privacy = taskInfo.privacy,
                    RecurrsionID = taskInfo.recurssionID,
                    MessageID = taskInfo.messageID,
                    ChannelID = taskInfo.channelID,
                    SendNowFlag = taskInfo.postSendNowFlag,
                    IsActive = taskInfo.IsActive
                };
                await reflectionDataRepository.InsertOrMergeAsync(reflectEntity);
                await SaveQuestionsDataAsync(configuration, taskInfo);
                await SaveRecurssionDataAsync(configuration, taskInfo);                
            }            
        }

        public static async Task SaveQuestionsDataAsync(IConfiguration configuration, TaskInfo taskInfo)
        {
            QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(configuration);
            var rowKey = Guid.NewGuid();

            QuestionsDataEntity questionEntity = new QuestionsDataEntity
            {
                QuestionID = taskInfo.questionID,
                PartitionKey = PartitionKeyNames.QuestionsDataTable.QuestionsDataPartition,
                RowKey = rowKey.ToString(),
                Question = taskInfo.question,
                QuestionCreatedDate = DateTime.Now,
                IsDefaultFlag = false, //handle default flag logic
                CreatedBy = taskInfo.postCreateBy
            };

            await questionsDataRepository.CreateOrUpdateAsync(questionEntity);
        }

        public static async Task SaveRecurssionDataAsync(IConfiguration configuration, TaskInfo taskInfo)
        {
            RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(configuration);
            var rowKey = Guid.NewGuid();

            RecurssionDataEntity recurssionEntity = new RecurssionDataEntity
            {
                RecurssionID = taskInfo.recurssionID,
                PartitionKey = PartitionKeyNames.RecurssionDataTable.RecurssionDataPartition, // read it from json
                RowKey = rowKey.ToString(),
                ReflectionID = taskInfo.reflectionID,
                RecursstionType = taskInfo.recurssionType,
                CreatedDate = DateTime.Now,
                ExecutionDate = taskInfo.executionDate,
                ExecutionTime = taskInfo.executionTime
            };
            await recurssionDataRepository.CreateOrUpdateAsync(recurssionEntity);
        }

        public static async Task<Dictionary<int, int>> SaveReflectionFeedbackDataAsync(UserfeedbackInfo taskInfo, IConfiguration configuration, ITurnContext<IInvokeActivity> turnContext)
        {
            FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(configuration);

            if (taskInfo != null)
            {
                var feedbackID = Guid.NewGuid();
                var refID = Guid.NewGuid();
                var rowKey = Guid.NewGuid();
                string email = await GetUserEmailId(turnContext);

                FeedbackDataEntity feedbackDataEntity = new FeedbackDataEntity
                {
                    PartitionKey = PartitionKeyNames.FeedbackDataTable.FeedbackDataPartition, // read it from json
                    RowKey = rowKey.ToString(),
                    FeedbackID = feedbackID,
                    FullName ="",
                    ReflectionID = taskInfo.reflectionID,
                    FeedbackGivenBy = email,
                    Feedback = 1

                };
                await feedbackDataRepository.InsertOrMergeAsync(feedbackDataEntity);
            }
            
            Dictionary<int, int> feedbacks = await feedbackDataRepository.GetReflectionFeedback(taskInfo.reflectionID);
            return feedbacks ?? null;
        }

        private static async Task<ReflectionDataEntity> ParseReflectionData(ITurnContext<IInvokeActivity> turnContext)
        {
            var row = turnContext.Activity?.From?.AadObjectId;
            if (row != null)
            {
                var reflectionDataEntity = new ReflectionDataEntity
                {
                    PartitionKey = PartitionKeyNames.ReflectionDataTable.ReflectionDataPartition,
                    RowKey = turnContext.Activity?.From?.AadObjectId,
                    //CreatedBy = await GetUserName(turnContext),
                    CreatedBy = await GetUserEmailId(turnContext),
                };

                return reflectionDataEntity;
            }

            return null;
        }

        public static async Task<string> GetUserEmailId(ITurnContext<IInvokeActivity> turnContext)
        {
            // Fetch the members in the current conversation
            try
            {
                IConnectorClient connector = turnContext.TurnState.Get<IConnectorClient>();

                var members = await connector.Conversations.GetConversationMembersAsync(turnContext.Activity.Conversation.Id);
                return AsTeamsChannelAccounts(members).FirstOrDefault(m => m.Id == turnContext.Activity.From.Id).UserPrincipalName;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static IEnumerable<TeamsChannelAccount> AsTeamsChannelAccounts(IEnumerable<ChannelAccount> channelAccountList)
        {
            foreach (ChannelAccount channelAccount in channelAccountList)
            {
                yield return JObject.FromObject(channelAccount).ToObject<TeamsChannelAccount>();
            }
        }


        //public static async Task<List<string>> GetTeamMember(ITurnContext<IInvokeActivity> turnContext)
        //{
        //    IConnectorClient connector = turnContext.TurnState.Get<IConnectorClient>();
        //    var members = await connector.Conversations.GetConversationMembersAsync(turnContext.Activity.Conversation.Id);

        //    return members.ToList();
        //}

        //public static async Task<List<ReflectionDataEntity>> GetAllReflection(IConfiguration configuration)
        //{
        //    RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(configuration);
        //    var allReflections = recurssionDataRepository.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
        //}

    }
}



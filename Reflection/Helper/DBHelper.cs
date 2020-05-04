using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Reflection.Model;
using Reflection.Repositories;
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

            //below two lines will bring default questions
            QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(configuration);
            var questions = await questionsDataRepository.GetAllDefaultQuestions();


            //var connector = new ConnectorClient(new Uri(turnContext.Activity.ServiceUrl));
            //var teamId = turnContext.Activity.GetChannelData<TeamsChannelData>().Team.Id;
            //var members = await connector.Conversations.GetConversationMembersAsync(teamId);

            if (taskInfo != null)
            {
                var refID = Guid.NewGuid();
                var qid = Guid.NewGuid();
                var recurID = Guid.NewGuid();
                var rowKey = Guid.NewGuid();
                
                ReflectionDataEntity reflectEntity = new ReflectionDataEntity
                {
                    ReflectionID = refID,
                    PartitionKey = PartitionKeyNames.ReflectionDataTable.ReflectionDataPartition, // read it from json
                    RowKey = rowKey.ToString(),
                    CreatedBy = taskInfo.postCreateBy,
                    RefCreatedDate = DateTime.Now,
                    QuestionID = qid,
                    Privacy = "personal",
                    RecurrsionID = recurID,
                    MessageID = new Guid(),
                    ChannelID = new Guid(),
                    IsActive = false
                };
                await reflectionDataRepository.InsertOrMergeAsync(reflectEntity);
                await DBHelper.SaveQuestionsDataAsync(configuration, qid, taskInfo);
                await DBHelper.SaveRecurssionDataAsync(configuration, recurID, refID, taskInfo);
            }           
        }

        public static async Task SaveQuestionsDataAsync(IConfiguration configuration, Guid qID, TaskInfo taskInfo)
        {
            QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(configuration);
            var rowKey = Guid.NewGuid();

            QuestionsDataEntity questionEntity = new QuestionsDataEntity
            {
                QuestionID = qID,
                PartitionKey = PartitionKeyNames.QuestionsDataTable.QuestionsDataPartition, // read it from json
                RowKey = rowKey.ToString(),
                Question = taskInfo.question,
                QuestionCreatedDate = DateTime.Now,
                IsDefaultFlag = false, //handle default flag logic
                CreatedBy = ""
            };

            await questionsDataRepository.CreateOrUpdateAsync(questionEntity);
        }

        public static async Task SaveRecurssionDataAsync(IConfiguration configuration, Guid recurID, Guid refID, TaskInfo taskInfo)
        {
            RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(configuration);
            var rowKey = Guid.NewGuid();

            RecurssionDataEntity recurssionEntity = new RecurssionDataEntity
            {
                RecurssionID = recurID,
                PartitionKey = PartitionKeyNames.RecurssionDataTable.RecurssionDataPartition, // read it from json
                RowKey = rowKey.ToString(),
                ReflectionID = refID,
                RecursstionType = taskInfo.recurssionType,
                CreatedDate= DateTime.Now,
                ExecutionDate = taskInfo.executionDate,
                ExecutionTime = taskInfo.executionTime            
            };
            await recurssionDataRepository.CreateOrUpdateAsync(recurssionEntity);
        }

        private static async Task<ReflectionDataEntity> ParseReflectionData(ITurnContext<IInvokeActivity> turnContext)
        {
            var row = turnContext.Activity?.From?.AadObjectId;
            if(row != null)
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

        private static async Task<string> GetUserEmailId(ITurnContext<IInvokeActivity> turnContext)
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

        public static async Task GetAllReflections()
        {

        }

        public static async Task GetTeamMember(ITurnContext<IInvokeActivity> turnContext)
        {
            var connector = new ConnectorClient(new Uri(turnContext.Activity.ServiceUrl));
            var teamId = turnContext.Activity.GetChannelData<TeamsChannelData>().Team.Id;
            var members = await connector.Conversations.GetConversationMembersAsync(teamId);
        }
    }
}

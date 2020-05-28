using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
using System.Threading;
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
        public static async Task SaveReflectionDataAsync(TaskInfo taskInfo, IConfiguration configuration)
        {
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(configuration);
            QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(configuration);

            if (taskInfo != null)
            {
                taskInfo.reflectionID = Guid.NewGuid();
                taskInfo.recurssionID = Guid.NewGuid();
                if (taskInfo.questionID == null)
                {
                    taskInfo.questionID = Guid.NewGuid();
                }
                var rowKey = Guid.NewGuid();

                ReflectionDataEntity reflectEntity = new ReflectionDataEntity
                {
                    ReflectionID = taskInfo.reflectionID,
                    PartitionKey = PartitionKeyNames.ReflectionDataTable.ReflectionDataPartition, // read it from json
                    RowKey = rowKey.ToString(),
                    CreatedBy = taskInfo.postCreateBy,
                    CreatedByEmail = taskInfo.postCreatedByEmail,
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

                if (!(taskInfo.recurssionType == "Does not repeat" && taskInfo.postSendNowFlag == true))
                {
                    await SaveRecurssionDataAsync(configuration, taskInfo);
                }
            }
        }


       

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public static async Task SaveQuestionsDataAsync(IConfiguration configuration, TaskInfo taskInfo)
        {
            QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(configuration);
            var rowKey = Guid.NewGuid();

            if (await questionsDataRepository.IsQuestionAlreadtPresent(taskInfo.question, taskInfo.postCreatedByEmail) == false)
            {
                QuestionsDataEntity questionEntity = new QuestionsDataEntity
                {
                    QuestionID = taskInfo.questionID,
                    PartitionKey = PartitionKeyNames.QuestionsDataTable.QuestionsDataPartition,
                    RowKey = rowKey.ToString(),
                    Question = taskInfo.question,
                    QuestionCreatedDate = DateTime.Now,
                    IsDefaultFlag = false, //handle default flag logic
                    CreatedBy = taskInfo.postCreateBy,
                    CreatedByEmail = taskInfo.postCreatedByEmail
                };

                await questionsDataRepository.CreateOrUpdateAsync(questionEntity);
            }
        }

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public static async Task SaveRecurssionDataAsync(IConfiguration configuration, TaskInfo taskInfo)
        {
            RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(configuration);
            var rowKey = Guid.NewGuid();

            RecurssionDataEntity recurssionEntity = new RecurssionDataEntity
            {
                RecurssionID = taskInfo.recurssionID,
                PartitionKey = PartitionKeyNames.RecurssionDataTable.RecurssionDataPartition,
                RowKey = rowKey.ToString(),
                ReflectionID = taskInfo.reflectionID,
                RecursstionType = taskInfo.recurssionType,
                CreatedDate = DateTime.Now,
                ExecutionDate = taskInfo.executionDate,
                ExecutionTime = taskInfo.executionTime,
                RecurssionEndDate = taskInfo.executionDate.AddDays(30)

            };
            await recurssionDataRepository.CreateOrUpdateAsync(recurssionEntity);
        }

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public static async Task SaveReflectionFeedbackDataAsync(UserfeedbackInfo taskInfo, IConfiguration configuration)
        {
            FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(configuration);

            if (taskInfo != null)
            {
                var feedbackID = Guid.NewGuid();
                var rowKey = Guid.NewGuid();
                //string email = await GetUserEmailId(turnContext);

                FeedbackDataEntity feedbackDataEntity = new FeedbackDataEntity
                {
                    PartitionKey = PartitionKeyNames.FeedbackDataTable.FeedbackDataPartition,
                    RowKey = rowKey.ToString(),
                    FeedbackID = feedbackID,
                    FullName = taskInfo.userName,
                    ReflectionID = taskInfo.reflectionId,
                    FeedbackGivenBy = taskInfo.emailId, //need changes - send it in card response and capture it
                    Feedback = Convert.ToInt32(taskInfo.feedbackId)

                };
                await feedbackDataRepository.InsertOrMergeAsync(feedbackDataEntity);
            }
        }

        //make above method and below method generic - need this change
        public static async Task<string> GetUserEmailId<T>(ITurnContext<T> turnContext) where T : Microsoft.Bot.Schema.IActivity
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

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
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

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public static async Task<ViewReflectionsEntity> GetViewReflectionsData(Guid reflectionId, IConfiguration configuration)
        {

            //var response = JsonConvert.DeserializeObject<UserfeedbackInfo>(turnContext.Activity.Value.ToString());
            //var refID = response.reflectionId;

            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(configuration);
            FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(configuration);
            ViewReflectionsEntity viewReflectionsEntity = new ViewReflectionsEntity();
            QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(configuration);
            //Guid gID = Guid.Parse("933a3991-29e9-4391-bdce-a81096b23c20"); for testing purpose

            //Get reflection data
            ReflectionDataEntity refData = await reflectionDataRepository.GetReflectionData(reflectionId) ?? null;
            Dictionary<int, List<FeedbackDataEntity>> feedbackData = await feedbackDataRepository.GetReflectionFeedback(reflectionId) ?? null;
            List<QuestionsDataEntity> questions = await questionsDataRepository.GetQuestionsByQID(refData.QuestionID) ?? null;

            viewReflectionsEntity.ReflectionData = refData;
            viewReflectionsEntity.FeedbackData = feedbackData;
            viewReflectionsEntity.Question = questions.Find(x => x.QuestionID == refData.QuestionID);
            return viewReflectionsEntity;

        }

        public static async Task<List<RecurssionScreenData>> GetRecurrencePostsDataAsync(IConfiguration configuration,string email)
        {
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(configuration);
            QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(configuration);
            RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(configuration);
            //RecurssionScreenData recurssionScreenData = new RecurssionScreenData();


            List<ReflectionDataEntity> allActiveRefs = await reflectionDataRepository.GetAllActiveReflection(email);
            List<Guid?> allActiveRefIDs = allActiveRefs.Select(c => c.ReflectionID).ToList();
            List<Guid?> allActiveQuestionIDs = allActiveRefs.Select(c => c.QuestionID).ToList();


            List<QuestionsDataEntity> allQuestionsData = await questionsDataRepository.GetAllQuestionData(allActiveQuestionIDs);
            List<RecurssionDataEntity> allRecurssionData = await recurssionDataRepository.GetAllRecurssionData(allActiveRefIDs);

            List<RecurssionScreenData> screenData = new List<RecurssionScreenData>();
            //recurssionScreenData.Add
            //foreach (var x in allActiveRefs)
            //{
            //    RecurssionScreenData recurssionScreenData = new RecurssionScreenData();
            //    recurssionScreenData.RefID = x.ReflectionID;
            //    recurssionScreenData.CreatedBy = x.CreatedBy;
            //    recurssionScreenData.RefCreatedDate = x.RefCreatedDate;
            //    recurssionScreenData.Privacy = x.Privacy;
            //    recurssionScreenData.Question = allQuestionsData.Where(c => c.QuestionID.ToString() == x.QuestionID.ToString()).Select(d => d.Question).FirstOrDefault().ToString();
            //    recurssionScreenData.ExecutionDate = allRecurssionData.Where(c => c.RecurssionID.ToString() == x.RecurrsionID.ToString()).Select(d => d.ExecutionDate).FirstOrDefault();
            //    recurssionScreenData.ExecutionTime = allRecurssionData.Where(c => c.RecurssionID.ToString() == x.RecurrsionID.ToString()).Select(d => d.ExecutionTime).FirstOrDefault();
            //    recurssionScreenData.RecurssionType = allRecurssionData.Where(c => c.RecurssionID.ToString() == x.RecurrsionID.ToString()).Select(d => d.RecursstionType).FirstOrDefault();
            //    if (recurssionScreenData.RecurssionType != null)
            //        screenData.Add(recurssionScreenData);
            //}
            foreach(var rec in allRecurssionData)
            {
                RecurssionScreenData recurssionScreenData = new RecurssionScreenData();
                recurssionScreenData.RefID = rec.ReflectionID;
                var reflection= await reflectionDataRepository.GetReflectionData(rec.ReflectionID); 
                recurssionScreenData.CreatedBy = reflection.CreatedBy;
                recurssionScreenData.RefCreatedDate = reflection.RefCreatedDate;
                recurssionScreenData.Privacy =reflection.Privacy;
                recurssionScreenData.Question = allQuestionsData.Where(c => c.QuestionID.ToString() == reflection.QuestionID.ToString()).Select(d => d.Question).FirstOrDefault().ToString();
                recurssionScreenData.ExecutionDate = rec.ExecutionDate;
                recurssionScreenData.ExecutionTime = rec.ExecutionTime;
                recurssionScreenData.RecurssionType = rec.RecursstionType;
                if (recurssionScreenData.RecurssionType != null)
                    screenData.Add(recurssionScreenData);
            }


            return screenData;
        }

        public static async Task DeleteRecurrsionDataAsync(Guid reflectionId, IConfiguration configuration)
        {
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(configuration);
            RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(configuration);
            var reflection = await reflectionDataRepository.GetReflectionData(reflectionId);
            var recurssion = await recurssionDataRepository.GetRecurssionData(reflection.RecurrsionID);
            await recurssionDataRepository.DeleteAsync(recurssion);
            await reflectionDataRepository.DeleteAsync(reflection);

            
        }




    }
}



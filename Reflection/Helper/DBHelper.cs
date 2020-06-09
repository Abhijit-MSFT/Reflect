using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Reflection.Interfaces;
using Reflection.Model;
using Reflection.Repositories;
using Reflection.Repositories.FeedbackData;
using Reflection.Repositories.QuestionsData;
using Reflection.Repositories.RecurssionData;
using Reflection.Repositories.ReflectionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Helper
{
    public class DBHelper : IDataBase
    {
        private IConfiguration _configuration;
        private TelemetryClient _telemetry;

        public DBHelper(IConfiguration configuration, TelemetryClient telemetry)
        {
            _configuration = configuration;
            _telemetry = telemetry;
        }

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task SaveReflectionDataAsync(TaskInfo taskInfo)
        {
            _telemetry.TrackEvent("SaveReflectionDataAsync");
            try
            {
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration, _telemetry);

                if (taskInfo != null)
                {
                    taskInfo.reflectionID = Guid.NewGuid();
                    taskInfo.recurssionID = Guid.NewGuid();
                    if (taskInfo.questionID == null)
                    {
                        taskInfo.questionID = Guid.NewGuid();
                    }

                    ReflectionDataEntity reflectEntity = new ReflectionDataEntity
                    {
                        ReflectionID = taskInfo.reflectionID,
                        PartitionKey = PartitionKeyNames.ReflectionDataTable.ReflectionDataPartition, // read it from json
                        RowKey = taskInfo.reflectionRowKey,
                        CreatedBy = taskInfo.postCreateBy,
                        CreatedByEmail = taskInfo.postCreatedByEmail,
                        RefCreatedDate = DateTime.Now,
                        QuestionID = taskInfo.questionID,
                        Privacy = taskInfo.privacy,
                        RecurrsionID = taskInfo.recurssionID,
                        MessageID = taskInfo.messageID,
                        ChannelID = taskInfo.channelID,
                        SendNowFlag = taskInfo.postSendNowFlag,
                        IsActive = taskInfo.IsActive,
                        ReflectMessageId = taskInfo.reflectMessageId,
                        TenantId = taskInfo.teantId,
                        ServiceUrl = taskInfo.serviceUrl
                    };
                    await reflectionDataRepository.InsertOrMergeAsync(reflectEntity);
                    if (await questionsDataRepository.IsQuestionAlreadtPresent(taskInfo.question, taskInfo.postCreatedByEmail) == false)
                    {
                        await SaveQuestionsDataAsync(taskInfo);
                    }
                    else
                    {
                        var ques = await questionsDataRepository.GetQuestionData(taskInfo.questionID);
                        taskInfo.questionRowKey = ques.RowKey;
                    }

                    if (!(taskInfo.postSendNowFlag == true))
                    {
                        await SaveRecurssionDataAsync(taskInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task SaveQuestionsDataAsync(TaskInfo taskInfo)
        {
            _telemetry.TrackEvent("DeleteReflections");
            try
            {
                QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration, _telemetry);
                QuestionsDataEntity questionEntity = new QuestionsDataEntity
                {
                    QuestionID = taskInfo.questionID,
                    PartitionKey = PartitionKeyNames.QuestionsDataTable.QuestionsDataPartition,
                    RowKey = taskInfo.questionRowKey,
                    Question = taskInfo.question,
                    QuestionCreatedDate = DateTime.Now,
                    IsDefaultFlag = false, //handle default flag logic
                    CreatedBy = taskInfo.postCreateBy,
                    CreatedByEmail = taskInfo.postCreatedByEmail
                };

                await questionsDataRepository.CreateOrUpdateAsync(questionEntity);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task SaveRecurssionDataAsync(TaskInfo taskInfo)
        {
            _telemetry.TrackEvent("SaveRecurssionDataAsync");
            try
            {
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);

                RecurssionDataEntity recurssionEntity = new RecurssionDataEntity
                {
                    RecurssionID = taskInfo.recurssionID,
                    PartitionKey = PartitionKeyNames.RecurssionDataTable.RecurssionDataPartition,
                    RowKey = taskInfo.recurrsionRowKey,
                    ReflectionRowKey = taskInfo.reflectionRowKey,
                    QuestionRowKey = taskInfo.questionRowKey,
                    ReflectionID = taskInfo.reflectionID,
                    RecursstionType = taskInfo.recurssionType,
                    CreatedDate = DateTime.Now,
                    ExecutionDate = taskInfo.executionDate,
                    ExecutionTime = taskInfo.executionTime,
                    RecurssionEndDate = taskInfo.executionDate.AddDays(30)

                };
                await recurssionDataRepository.CreateOrUpdateAsync(recurssionEntity);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task SaveReflectionFeedbackDataAsync(UserfeedbackInfo taskInfo)
        {
            _telemetry.TrackEvent("DeleteReflections");
            try
            {

                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);

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
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        //make above method and below method generic - need this change
        public async Task<string> GetUserEmailId<T>(ITurnContext<T> turnContext) where T : Microsoft.Bot.Schema.IActivity
        {
            _telemetry.TrackEvent("GetUserEmailId");

            // Fetch the members in the current conversation
            try
            {
                IConnectorClient connector = turnContext.TurnState.Get<IConnectorClient>();

                var members = await connector.Conversations.GetConversationMembersAsync(turnContext.Activity.Conversation.Id);
                var user = AsTeamsChannelAccounts(members).FirstOrDefault(m => m.Id == turnContext.Activity.From.Id);
                return AsTeamsChannelAccounts(members).FirstOrDefault(m => m.Id == turnContext.Activity.From.Id).UserPrincipalName;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return "";
            }
        }

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        private IEnumerable<TeamsChannelAccount> AsTeamsChannelAccounts(IEnumerable<ChannelAccount> channelAccountList)
        {
            _telemetry.TrackEvent("AsTeamsChannelAccounts");

            foreach (ChannelAccount channelAccount in channelAccountList)
            {
                yield return JObject.FromObject(channelAccount).ToObject<TeamsChannelAccount>();
            }
        }

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task<ViewReflectionsEntity> GetViewReflectionsData(Guid reflectionId)
        {
            _telemetry.TrackEvent("GetViewReflectionsData");

            try
            {

                //var response = JsonConvert.DeserializeObject<UserfeedbackInfo>(turnContext.Activity.Value.ToString());
                //var refID = response.reflectionId;

                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                ViewReflectionsEntity viewReflectionsEntity = new ViewReflectionsEntity();
                QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration, _telemetry);
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
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }

        }

        public async Task<List<RecurssionScreenData>> GetRecurrencePostsDataAsync(string email)
        {
            _telemetry.TrackEvent("GetRecurrencePostsDataAsync");
            try
            {
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration, _telemetry);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                List<ReflectionDataEntity> allActiveRefs = await reflectionDataRepository.GetAllActiveReflection(email);
                List<Guid?> allActiveRefIDs = allActiveRefs.Select(c => c.ReflectionID).ToList();
                List<Guid?> allActiveQuestionIDs = allActiveRefs.Select(c => c.QuestionID).ToList();
                List<QuestionsDataEntity> allQuestionsData = await questionsDataRepository.GetAllQuestionData(allActiveQuestionIDs);
                List<RecurssionDataEntity> allRecurssionData = await recurssionDataRepository.GetAllRecurssionData(allActiveRefIDs);
                List<RecurssionScreenData> screenData = new List<RecurssionScreenData>();
                foreach (var rec in allRecurssionData)
                {
                    RecurssionScreenData recurssionScreenData = new RecurssionScreenData();
                    recurssionScreenData.RefID = rec.ReflectionID;
                    var reflection = await reflectionDataRepository.GetReflectionData(rec.ReflectionID);
                    recurssionScreenData.CreatedBy = reflection.CreatedBy;
                    recurssionScreenData.RefCreatedDate = reflection.RefCreatedDate;
                    recurssionScreenData.Privacy = reflection.Privacy;
                    recurssionScreenData.Question = allQuestionsData.Where(c => c.QuestionID.ToString() == reflection.QuestionID.ToString()).Select(d => d.Question).FirstOrDefault().ToString();
                    recurssionScreenData.ExecutionDate = rec.ExecutionDate;
                    recurssionScreenData.ExecutionTime = rec.ExecutionTime;
                    recurssionScreenData.RecurssionType = rec.RecursstionType;
                    if (recurssionScreenData.RecurssionType != null)
                        screenData.Add(recurssionScreenData);
                }
                return screenData;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }


        /// <summary>
        /// Delete Reflection and recurssion related to that reflection
        /// </summary>
        /// <param name="Iconfiguration">Reads The config from app settings</param>
        /// <param name="reflectionId">Specific reflectionid that is to be deleted</param>
        public  async Task DeleteRecurrsionDataAsync(Guid reflectionId)
        {
            try
            {
                _telemetry.TrackEvent("DeleteRecurrsionDataAsync");
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                var reflection = await reflectionDataRepository.GetReflectionData(reflectionId);
                var recurssion = await recurssionDataRepository.GetRecurssionData(reflection.RecurrsionID);
                await recurssionDataRepository.DeleteAsync(recurssion);
                await reflectionDataRepository.DeleteAsync(reflection);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Remove Reflection 
        /// </summary>
        /// <param name="Iconfiguration">Reads The config from app settings</param>
        /// <param name="reflectionMessageId">Specific MessageId that is to be deleted</param>
        public async Task<string> RemoveReflectionId(string reflectionMessageId)
        {
            string messageId = null;
            try
            {
                _telemetry.TrackEvent("RemoveMessageId");
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                var reflection = await reflectionDataRepository.GetReflectionData(reflectionMessageId);
                messageId = reflection.MessageID;
                var feedbackCount = await feedbackDataRepository.GetFeedbackonRefId(reflection.ReflectionID);
                await feedbackDataRepository.DeleteAsync(feedbackCount);
                await reflectionDataRepository.DeleteAsync(reflection);
                
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
            return messageId;
        }
        /// <summary>
        /// update Reflection and recurssion related to that reflection
        /// </summary>
        /// <param name="Iconfiguration">Reads The config from app settings</param>
        /// <param name="reflection">COmbination of reflection and recurssion to save data</param>


        public  async Task SaveEditRecurssionDataAsync(RecurssionScreenData reflection)
        {
            try
            {
                _telemetry.TrackEvent("SaveEditRecurssionDataAsync");
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration,_telemetry);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration,_telemetry);
                var reflectiondata = await reflectionDataRepository.GetReflectionData(reflection.RefID);
                var recurssion = await recurssionDataRepository.GetRecurssionData(reflectiondata.RecurrsionID);
                reflectiondata.Privacy = reflection.Privacy;
                recurssion.ExecutionDate = reflection.ExecutionDate;
                recurssion.ExecutionTime = reflection.ExecutionTime;
                recurssion.RecursstionType = reflection.RecurssionType;
                await recurssionDataRepository.CreateOrUpdateAsync(recurssion);
                await reflectionDataRepository.CreateOrUpdateAsync(reflectiondata);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }




    }
}



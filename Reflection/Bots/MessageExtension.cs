// <copyright file="MessageExtension.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using AdaptiveCards;
using Bogus;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reflection.Interfaces;
using Reflection.Model;
using Reflection.Repositories.FeedbackData;
using Reflection.Repositories.ReflectionData;
using Reflection.Repositories.RecurssionData;
using Reflection.Repositories.QuestionsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Teams.Samples.HelloWorld.Web
{
    public class MessageExtension : TeamsActivityHandler
    {
        private readonly IConfiguration _configuration;
        private readonly TelemetryClient _telemetry;
        private readonly ICard _cardHelper;
        private readonly IDataBase _dbHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExtension"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="telemetry">Represents the application telemetry.</param>
        /// <param name="cardHelper">Represents the application cardHelper.</param>
        /// <param name="dbHelper">Represents the application dbHelper.</param>
        public MessageExtension(IConfiguration configuration, TelemetryClient telemetry, ICard cardHelper, IDataBase dbHelper)
        {
            _configuration = configuration;
            _telemetry = telemetry;
            _cardHelper = cardHelper;
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// Method for on message activity.
        /// </summary>
        /// <param name="turnContext">Turn context input for the method.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnMessageActivityAsync");

            try
            {
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                QuestionsDataRepository questiondatarepository = new QuestionsDataRepository(_configuration, _telemetry);

                if (turnContext.Activity.Value != null)
                {
                    var response = JsonConvert.DeserializeObject<UserfeedbackInfo>(turnContext.Activity.Value.ToString());
                    var reply = Activity.CreateMessageActivity();
                    var adaptiveupdatereply = Activity.CreateMessageActivity();
                    if (response.type == ReflectConstants.SaveFeedBack)
                    {
                        var name = (turnContext.Activity.From.Name).Split();
                        response.userName = name[0] + ' ' + name[1];
                        response.emailId = await _dbHelper.GetUserEmailId(turnContext);

                        FeedbackDataEntity feebackData = await feedbackDataRepository.GetReflectionFeedback(Guid.Parse(response.reflectionId), response.emailId);
                        if (feebackData != null && response.emailId == feebackData.FeedbackGivenBy)
                        {
                            feebackData.Feedback = response.feedbackId;
                            await feedbackDataRepository.CreateOrUpdateAsync(feebackData);
                        }
                        else
                        {
                            await _dbHelper.SaveReflectionFeedbackDataAsync(response);
                        }

                        try
                        {
                            ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(Guid.Parse(response.reflectionId));
                            QuestionsDataEntity question = await questiondatarepository.GetQuestionData(reflectData.QuestionID);
                            Dictionary<int, List<FeedbackDataEntity>> feedbacks = await feedbackDataRepository.GetReflectionFeedback(Guid.Parse(response.reflectionId));
                            var adaptiveCard = _cardHelper.FeedBackCard(feedbacks, Guid.Parse(response.reflectionId));
                            TaskInfo taskInfo = new TaskInfo();
                            taskInfo.question = question.Question;
                            taskInfo.postCreateBy = reflectData.CreatedBy;
                            taskInfo.privacy = reflectData.Privacy;
                            taskInfo.reflectionID = reflectData.ReflectionID;
                            var updateadaptivecard = _cardHelper.CreateNewReflect(taskInfo); 
                            Attachment attachment = new Attachment()
                            {
                                ContentType = AdaptiveCard.ContentType,
                                Content = adaptiveCard
                            };
                            Attachment attachmentadaptive = new Attachment()
                            {
                                ContentType = AdaptiveCard.ContentType,
                                Content = updateadaptivecard
                            };
                            reply.Attachments.Add(attachment);
                            adaptiveupdatereply.Attachments.Add(attachmentadaptive);
                            adaptiveupdatereply.Id = reflectData.ReflectMessageId;
                            await turnContext.UpdateActivityAsync(adaptiveupdatereply);
                            if (reflectData.MessageID == null)
                            {
                                var result = turnContext.SendActivityAsync(reply, cancellationToken);
                                reflectData.MessageID = result.Result.Id;
                                await reflectionDataRepository.InsertOrMergeAsync(reflectData);
                            }
                            else
                            {
                                reply.Id = reflectData.MessageID;
                                await turnContext.UpdateActivityAsync(reply);
                            }
                        }
                        catch (System.Exception e)
                        {
                            _telemetry.TrackException(e);
                            Console.WriteLine(e.Message.ToString());
                        }
                    }
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello from the TeamsMessagingExtensionsActionPreviewBot."), cancellationToken);
                }

            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Method for submitting teams task module.
        /// </summary>
        /// <param name="turnContext">Turn context input for the method.</param>
        /// <param name="taskModuleRequest">Task request input for the method.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
            QuestionsDataRepository questiondatarepository = new QuestionsDataRepository(_configuration, _telemetry);
            FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
            try
            {
                TaskInfo taskInfo = JsonConvert.DeserializeObject<TaskInfo>(taskModuleRequest.Data.ToString());
                var reply = Activity.CreateMessageActivity();
                //Check if message id is present in reflect data
                ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(taskInfo.reflectionID);
                QuestionsDataEntity question = await questiondatarepository.GetQuestionData(reflectData.QuestionID);
                Dictionary<int, List<FeedbackDataEntity>> feedbacks = await feedbackDataRepository.GetReflectionFeedback(taskInfo.reflectionID);
                var adaptiveCard = _cardHelper.FeedBackCard(feedbacks, taskInfo.reflectionID);

                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = adaptiveCard
                };
                reply.Attachments.Add(attachment);
                if (reflectData.MessageID == null)
                {
                    var result = turnContext.SendActivityAsync(reply, cancellationToken);
                    reflectData.MessageID = result.Result.Id;
                    //update messageid in reflectio table
                    await reflectionDataRepository.InsertOrMergeAsync(reflectData);
                }
                else
                {
                    reply.Id = reflectData.MessageID;
                    await turnContext.UpdateActivityAsync(reply);
                }
                return null;
            }
            catch (System.Exception e)
            {
                _telemetry.TrackException(e);
                Console.WriteLine(e.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// Method for fetching teams task module.
        /// </summary>
        /// <param name="turnContext">Turn context input for the method.</param>
        /// <param name="taskModuleRequest">Task request input for the method.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsTaskModuleFetchAsync");
            try
            {
                ReflctionData reldata = JsonConvert.DeserializeObject<ReflctionData>(taskModuleRequest.Data.ToString());
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                QuestionsDataRepository questiondatarepository = new QuestionsDataRepository(_configuration, _telemetry);
                var response = new UserfeedbackInfo();
                var name = (turnContext.Activity.From.Name).Split();
                response.userName = name[0] + ' ' + name[1];
                response.emailId = await _dbHelper.GetUserEmailId(turnContext);
                var reflectionid = reldata.data.URL.Replace("https://", "").Split('/')[2];
                var feedbackId = Convert.ToInt32(reldata.data.URL.Replace("https://", "").Split('/')[3]);
                response.reflectionId = reflectionid;
                response.feedbackId = feedbackId;
                // Check if this is user's second feedback
                FeedbackDataEntity feebackData = await feedbackDataRepository.GetReflectionFeedback(Guid.Parse(response.reflectionId), response.emailId);
                if (response.feedbackId != 0)
                {
                    if (feebackData != null && response.emailId == feebackData.FeedbackGivenBy)
                    {
                        feebackData.Feedback = response.feedbackId;
                        await feedbackDataRepository.CreateOrUpdateAsync(feebackData);
                    }
                    else
                    {
                        await _dbHelper.SaveReflectionFeedbackDataAsync(response);
                    }
                    try
                    {
                        //Check if message id is present in reflect data
                        ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(Guid.Parse(response.reflectionId));
                        QuestionsDataEntity question = await questiondatarepository.GetQuestionData(reflectData.QuestionID);
                        Dictionary<int, List<FeedbackDataEntity>> feedbacks = await feedbackDataRepository.GetReflectionFeedback(Guid.Parse(response.reflectionId));
                        var adaptiveCard = _cardHelper.FeedBackCard(feedbacks, Guid.Parse(response.reflectionId));
                        TaskInfo taskInfo = new TaskInfo();
                        taskInfo.question = question.Question;
                        taskInfo.postCreateBy = reflectData.CreatedBy;
                        taskInfo.privacy = reflectData.Privacy;
                        taskInfo.reflectionID = reflectData.ReflectionID;
                        Attachment attachment = new Attachment()
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = adaptiveCard
                        };
                        var reply = Activity.CreateMessageActivity();
                        reply.Attachments.Add(attachment);
                        if (reflectData.MessageID == null)
                        {
                            var result = turnContext.SendActivityAsync(reply, cancellationToken);
                            reflectData.MessageID = result.Result.Id;
                            //update messageid in reflectio table
                            await reflectionDataRepository.InsertOrMergeAsync(reflectData);
                        }
                        else
                        {
                            reply.Id = reflectData.MessageID;
                            await turnContext.UpdateActivityAsync(reply);
                        }
                    }
                    catch (System.Exception e)
                    {
                        _telemetry.TrackException(e);
                        Console.WriteLine(e.Message.ToString());
                    }
                }

                return new TaskModuleResponse
                {
                    Task = new TaskModuleContinueResponse
                    {
                        Value = new TaskModuleTaskInfo()
                        {
                            Height = 700,
                            Width = 600,
                            Title = "Check the pulse on emotinal well-being",
                            Url = reldata.data.URL
                        },
                    },
                };
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Method for submitting teams messaging extension.
        /// </summary>
        /// <param name="turnContext">Turn context input for the method.</param>
        /// <param name="action">Message action input.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionSubmitActionAsync");
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
            FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
            RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
            QuestionsDataRepository questiondatarepository = new QuestionsDataRepository(_configuration, _telemetry);
            try
            {
                TaskInfo taskInfo = JsonConvert.DeserializeObject<TaskInfo>(action.Data.ToString());
                switch (taskInfo.action)
                {
                    case "reflection":
                        return await OnTeamsMessagingExtensionFetchTaskAsync(turnContext, action, cancellationToken);
                    case "sendAdaptiveCard":
                        try
                        {
                            var name = (turnContext.Activity.From.Name).Split();
                            taskInfo.postCreateBy = name[0] + ' ' + name[1];
                            taskInfo.postCreatedByEmail = await _dbHelper.GetUserEmailId(turnContext);
                            taskInfo.channelID = turnContext.Activity.TeamsGetChannelId();
                            taskInfo.postSendNowFlag = (taskInfo.executionTime == "Send now") ? true : false;
                            taskInfo.IsActive = true;
                            taskInfo.questionRowKey = Guid.NewGuid().ToString();
                            taskInfo.recurrsionRowKey = Guid.NewGuid().ToString();
                            taskInfo.reflectionRowKey = Guid.NewGuid().ToString();
                            taskInfo.serviceUrl = turnContext.Activity.ServiceUrl;
                            taskInfo.teantId = turnContext.Activity.Conversation.TenantId;
                            await _dbHelper.SaveReflectionDataAsync(taskInfo);
                            if (taskInfo.postSendNowFlag == true)
                            {
                                var typingActivity = MessageFactory.Text(string.Empty);
                                typingActivity.Type = ActivityTypes.Typing;
                                await turnContext.SendActivityAsync(typingActivity);
                                var adaptiveCard = _cardHelper.CreateNewReflect(taskInfo);
                                var message = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = adaptiveCard });
                                var resultid = await turnContext.SendActivityAsync(message, cancellationToken);
                                ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(taskInfo.reflectionID);
                                reflectData.ReflectMessageId = resultid.Id;
                                await reflectionDataRepository.InsertOrMergeAsync(reflectData);
                            }
                            else
                            {
                                var reply = MessageFactory.Text(string.Empty);
                                if (taskInfo.recurssionType == "Does not repeat")
                                    reply.Text = "Your data is recorded and will be executed at time specified by you.";
                                else if(taskInfo.recurssionType == "Custom")
                                    reply.Text = "Your data is recorded and will be executed on " + taskInfo.customRecurssionTypeValue + " intervals.";
                                else
                                    reply.Text = "Your data is recorded and will be executed on " + taskInfo.recurssionType + " intervals.";
                                await turnContext.SendActivityAsync(reply);
                            }
                            return null;
                        }
                        catch (Exception ex)
                        {
                            _telemetry.TrackException(ex);
                            return null;
                        }
                    case "ManageRecurringPosts":
                        var postCreatedByEmail = await _dbHelper.GetUserEmailId(turnContext);
                        var response = new MessagingExtensionActionResponse()
                        {
                            Task = new TaskModuleContinueResponse()
                            {
                                Value = new TaskModuleTaskInfo()
                                {
                                    Height = 600,
                                    Width = 780,
                                    Title = "Check the pulse on emotinal well-being",
                                    Url = this._configuration["BaseUri"] + "/ManageRecurringPosts/" + postCreatedByEmail
                                },
                            },
                        };
                        return response;

                    case "OpenDetailfeedback":
                        var responsefeedback = new MessagingExtensionActionResponse()
                        {
                            Task = new TaskModuleContinueResponse()
                            {
                                Value = new TaskModuleTaskInfo()
                                {
                                    Height = 600,
                                    Width = 780,
                                    Title = "Check the pulse on emotinal well-being",
                                    Url = this._configuration["BaseUri"] + "/openReflectionFeedback/" + taskInfo.reflectionID + "/" + taskInfo.feedback
                                },
                            },
                        };
                        return responsefeedback;

                    default:
                        return null;
                };
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Method for fetching teams messaging extension.
        /// </summary>
        /// <param name="turnContext">Turn context input for the method.</param>
        /// <param name="action">Message action input.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionFetchTaskAsync");

            try
            {
                var url = this._configuration["BaseUri"];
                if (action.CommandId == ReflectConstants.RecurringPosts)
                {
                    var postCreatedByEmail = await _dbHelper.GetUserEmailId(turnContext);
                    url = this._configuration["BaseUri"] + "/ManageRecurringPosts/" + postCreatedByEmail;
                    var response = new MessagingExtensionActionResponse()
                    {
                        Task = new TaskModuleContinueResponse()
                        {
                            Value = new TaskModuleTaskInfo()
                            {
                                Height = 620,
                                Width = 800,
                                Title = "Invite people to share how they feel",
                                Url = url
                            },
                        },
                    };
                    return response;
                }
                else if (action.CommandId == ReflectConstants.RemovePosts)
                {
                    if (turnContext.Activity.Conversation.Id != null)
                    {
                        var replymessageid = turnContext.Activity.Conversation.Id.Split("=");
                        var activity = Activity.CreateMessageActivity();
                        string messageId = await _dbHelper.RemoveReflectionId(replymessageid[1]);
                        if (messageId != null)
                        {
                            await turnContext.DeleteActivityAsync(messageId);

                        }
                        await turnContext.DeleteActivityAsync(replymessageid[1]);
                        activity.Text = "Refelct Id removed successfully";
                        await turnContext.SendActivityAsync(activity);

                    }
                    return null;
                }
                else if (action.CommandId == ReflectConstants.CreateReflect)
                {
                    var name = (turnContext.Activity.From.Name).Split();
                    var userName = name[0] + ' ' + name[1];
                    url = this._configuration["BaseUri"] + "/" + userName;
                    var response = new MessagingExtensionActionResponse()
                    {
                        Task = new TaskModuleContinueResponse()
                        {
                            Value = new TaskModuleTaskInfo()
                            {
                                Height = 620,
                                Width = 800,
                                Title = "Invite people to share how they feel",
                                Url = url
                            },
                        },
                    };
                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Method for querying teams messaging extension.
        /// </summary>
        /// <param name="turnContext">Turn context input for the method.</param>
        /// <param name="query">Query input.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        protected override Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionQueryAsync");

            try
            {
                var title = "";
                var titleParam = query.Parameters?.FirstOrDefault(p => p.Name == "cardTitle");
                if (titleParam != null)
                {
                    title = titleParam.Value.ToString();
                }

                if (query == null || query.CommandId != "getRandomText")
                {
                    // We only process the 'getRandomText' queries with this message extension
                    throw new NotImplementedException($"Invalid CommandId: {query.CommandId}");
                }

                var attachments = new MessagingExtensionAttachment[5];

                for (int i = 0; i < 5; i++)
                {
                    attachments[i] = GetAttachment(title);
                }

                var result = new MessagingExtensionResponse
                {
                    ComposeExtension = new MessagingExtensionResult
                    {
                        AttachmentLayout = "list",
                        Type = "result",
                        Attachments = attachments.ToList()
                    },
                };
                return Task.FromResult(result);

            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets attachment.
        /// </summary>
        /// <param name="title">Title string as input.</param>
        private MessagingExtensionAttachment GetAttachment(string title)
        {
            _telemetry.TrackEvent("GetAttachment");

            try
            {
                var card = new ThumbnailCard
                {
                    Title = !string.IsNullOrWhiteSpace(title) ? title : new Faker().Lorem.Sentence(),
                    Text = new Faker().Lorem.Paragraph(),
                    Images = new List<CardImage> { new CardImage("http://lorempixel.com/640/480?rand=" + DateTime.Now.Ticks.ToString()) }
                };

                return card
                    .ToAttachment()
                    .ToMessagingExtensionAttachment();
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Method for teams extension item on select.
        /// </summary>
        /// <param name="turnContext">Turn context input for the method.</param>
        /// <param name="query">Query input.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        protected override Task<MessagingExtensionResponse> OnTeamsMessagingExtensionSelectItemAsync(ITurnContext<IInvokeActivity> turnContext, JObject query, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionSelectItemAsync");

            try
            {
                return Task.FromResult(new MessagingExtensionResponse
                {
                    ComposeExtension = new MessagingExtensionResult
                    {
                        AttachmentLayout = "list",
                        Type = "result",
                        Attachments = new MessagingExtensionAttachment[]{
                        new ThumbnailCard()
                            .ToAttachment()
                            .ToMessagingExtensionAttachment()
                    }
                    },
                });

            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }

        }

        /// <summary>
        /// Method for teams extension on button click.
        /// </summary>
        /// <param name="turnContext">Turn context input for the method.</param>
        /// <param name="cardData">Card Data input.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        protected async override Task OnTeamsMessagingExtensionCardButtonClickedAsync(ITurnContext<IInvokeActivity> turnContext, JObject cardData, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionCardButtonClickedAsync");
            try
            {
                var reply = MessageFactory.Text("OnTeamsMessagingExtensionCardButtonClickedAsync Value: " + JsonConvert.SerializeObject(turnContext.Activity.Value));
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }
    }

}

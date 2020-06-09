﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Collections.Generic;
using Bogus;
using Newtonsoft.Json;
using Reflection.Helper;
using Reflection.Model;
using Microsoft.Extensions.Configuration;
using AdaptiveCards;
using Reflection.Repositories.FeedbackData;
using Reflection.Repositories.QuestionsData;
using Reflection.Repositories.ReflectionData;
using Microsoft.ApplicationInsights;
using Reflection.Interfaces;

namespace Microsoft.Teams.Samples.HelloWorld.Web
{
    public class MessageExtension : TeamsActivityHandler
    {
        private readonly IConfiguration _configuration;
        private readonly TelemetryClient _telemetry;
        private readonly ICard _cardHelper;
        private readonly IDataBase _dbHelper;

        //private readonly FeedbackDataRepository feedbackDataRepository;
        public MessageExtension(IConfiguration configuration, TelemetryClient telemetry, ICard cardHelper, IDataBase dbHelper)
        {
            _configuration = configuration;
            _telemetry = telemetry;
            _cardHelper = cardHelper;
            _dbHelper = dbHelper;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnMessageActivityAsync");

            try
            {
                //CardHelper cardhelper = new CardHelper(_configuration);

                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                if (turnContext.Activity.Value != null)
                {
                    var response = JsonConvert.DeserializeObject<UserfeedbackInfo>(turnContext.Activity.Value.ToString());
                    var reply = Activity.CreateMessageActivity();

                    if (response.type == ReflectConstants.SaveFeedBack)
                    {
                        var name = (turnContext.Activity.From.Name).Split();
                        response.userName = name[0] + ' ' + name[1];
                        response.emailId = await _dbHelper.GetUserEmailId(turnContext);

                        //Check if this is user's second feedback
                        FeedbackDataEntity feebackData = await feedbackDataRepository.GetReflectionFeedback(response.reflectionId, response.emailId);
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
                            //Get reflect data to check if mseeage id is present - if not update it
                            ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(response.reflectionId);
                            Dictionary<int, List<FeedbackDataEntity>> feedbacks = await feedbackDataRepository.GetReflectionFeedback(response.reflectionId);
                            var adaptiveCard = _cardHelper.FeedBackCard(feedbacks, response.reflectionId);

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
                        }
                        catch (System.Exception e)
                        {
                            _telemetry.TrackException(e);
                            //messageid = null;
                            Console.WriteLine(e.Message.ToString());
                        }
                    }
                }
                else
                {
                    // This is a regular text message.
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello from the TeamsMessagingExtensionsActionPreviewBot."), cancellationToken);
                }

            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)

        {
            _telemetry.TrackEvent("OnTeamsTaskModuleFetchAsync");

            try
            {
                ReflctionData reldata = JsonConvert.DeserializeObject<ReflctionData>(taskModuleRequest.Data.ToString());

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

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionSubmitActionAsync");

            
            
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);

            try
            {
                TaskInfo taskInfo = JsonConvert.DeserializeObject<TaskInfo>(action.Data.ToString());
                //below two lines of code is added to test proactive message for scheduler
                //ProactiveMessageHelper proactiveMessageHelper = new ProactiveMessageHelper(_cardHelper);
                //await proactiveMessageHelper.SendCardToTeamAsync(turnContext, taskInfo, cancellationToken, _configuration);
                switch (taskInfo.action)
                {
                    case "reflection":
                        return await OnTeamsMessagingExtensionFetchTaskAsync(turnContext, action, cancellationToken);
                    case "sendAdaptiveCard":
                        try
                        {
                            var name = (turnContext.Activity.From.Name).Split();
                            //CardHelper cardhelper = new CardHelper(_configuration);
                            taskInfo.postCreateBy = name[0] + ' ' + name[1];
                            taskInfo.postCreatedByEmail = await _dbHelper.GetUserEmailId(turnContext);
                            taskInfo.channelID = turnContext.Activity.TeamsGetChannelId();
                            taskInfo.postSendNowFlag = (taskInfo.executionTime == "Send now") ? true : false;
                            taskInfo.IsActive = (taskInfo.executionTime == "Send now") ? false : true;
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
                                var adaptiveCard = _cardHelper.CreateNewPostCard(taskInfo);
                                var message = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = adaptiveCard });
                                var resultid=await turnContext.SendActivityAsync(message, cancellationToken);
                                ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(taskInfo.reflectionID);
                                reflectData.ReflectMessageId = resultid.Id;
                                await reflectionDataRepository.InsertOrMergeAsync(reflectData);
                            }
                            else
                            {
                                var reply = MessageFactory.Text(string.Empty);
                                reply.Text = "Your data is recorded and will be executed on " + taskInfo.recurssionType + " intervals";
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
                    if(turnContext.Activity.Conversation.Id!=null)
                    {
                        var replymessageid = turnContext.Activity.Conversation.Id.Split("=");
                        var activity = Activity.CreateMessageActivity();
                        bool isDelete = await _dbHelper.RemoveReflectionId(replymessageid[1]);
                        if(isDelete)
                        {
                            await turnContext.DeleteActivityAsync(replymessageid[1]);
                            activity.Text = "Refelct Id removed successfully";
                        }
                        else
                        {
                            activity.Text = "Refelct have feedback and not removed";
                        }
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

            //return base.OnTeamsMessagingExtensionCardButtonClickedAsync(turnContext, cardData, cancellationToken);
        }
    }

}

//QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration);
//var defaultQuestions = await questionsDataRepository.GetAllDefaultQuestions();

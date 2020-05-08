using System.Threading;
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

namespace Microsoft.Teams.Samples.HelloWorld.Web
{
    public class MessageExtension : TeamsActivityHandler
    {
        private readonly IConfiguration _configuration;
        private readonly FeedbackDataRepository feedbackDataRepository;
        public MessageExtension(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            CardHelper cardhelper = new CardHelper(_configuration);
            FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration);
            if (turnContext.Activity.Value != null)
            {
                var response = JsonConvert.DeserializeObject<UserfeedbackInfo>(turnContext.Activity.Value.ToString());
                if (response.type == "saveFeedback")
                {
                    response.userName = turnContext.Activity.From.Name;
                    var reply = Activity.CreateMessageActivity();
                    await DBHelper.SaveReflectionFeedbackDataAsync(response, _configuration);
                    try
                    {
                        Dictionary<int, int> feedbacks = await feedbackDataRepository.GetReflectionFeedback(response.reflectionID);

                        var adaptiveCard = cardhelper.FeedBackCard(feedbacks, response.reflectionID);
                        Attachment attachment = new Attachment()
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = adaptiveCard
                        };
                        reply.Attachments.Add(attachment);
                    }
                    catch(System.Exception e)
                    {
                        Console.WriteLine(e.Message.ToString());
                    }
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
                else if(response.type == "viewReflections")
                {

                }
            }
            else
            {
                // This is a regular text message.
                await turnContext.SendActivityAsync(MessageFactory.Text($"Hello from the TeamsMessagingExtensionsActionPreviewBot."), cancellationToken);
            }
        }
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            TaskInfo taskInfo = JsonConvert.DeserializeObject<TaskInfo>(action.Data.ToString());
            switch (taskInfo.action)
            {
                case "reflection":
                    return await OnTeamsMessagingExtensionFetchTaskAsync(turnContext, action, cancellationToken);
                case "sendAdaptiveCard":
                    CardHelper cardhelper = new CardHelper(_configuration);
                    taskInfo.postCreateBy = turnContext.Activity.From.Name;
                    taskInfo.channelID = turnContext.Activity.TeamsGetChannelId();
                    if(taskInfo.postSendNowFlag == true)
                    {
                        await DBHelper.SaveReflectionDataAsync(taskInfo, _configuration, turnContext);
                        try
                        {
                            var adaptiveCard = cardhelper.CreateNewPostCard(taskInfo);
                            var message = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = adaptiveCard });
                            await turnContext.SendActivityAsync(message, cancellationToken);
                            return null;
                        }
                        catch (System.Exception ex)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        await DBHelper.SaveReflectionDataAsync(taskInfo, _configuration, turnContext);
                    }
                    
                    try
                    {
                        var adaptiveCard = cardhelper.CreateNewPostCard(taskInfo);
                        var message = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = adaptiveCard });
                        await turnContext.SendActivityAsync(message, cancellationToken);
                        return null;
                    }
                    catch(System.Exception ex)
                    {
                        return null;
                    }

                case "ManageRecurringPosts":
                    var response = new MessagingExtensionActionResponse()
                    {
                        Task = new TaskModuleContinueResponse()
                        {
                            Value = new TaskModuleTaskInfo()
                            {
                                Height = 600,
                                Width = 780,
                                Title = "Check the pulse on emotinal well-being",
                                Url = this._configuration["BaseUri"] + "/ManageRecurringPosts"
                            },
                        },
                    };

                    return response;
                default:
                    return null;
            };

        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            string url = this._configuration["BaseUri"];
            if(action.MessagePayload != null)
                url = this._configuration["BaseUri"] + "/ManageRecurringPosts";
            QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration);
            var defaultQuestions = questionsDataRepository.GetAllDefaultQuestions();
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

            //if (action.MessagePayload == null)
            //{
            //    //    var messageText = action.MessagePayload.Body.Content;
            //    //    var fromId = action.MessagePayload.From.User.Id;


            //    var response = new MessagingExtensionActionResponse()
            //    {
            //        Task = new TaskModuleContinueResponse()
            //        {
            //            Value = new TaskModuleTaskInfo()
            //            {
            //                Height = 720,
            //                Width = 900,
            //                Title = "Invite people to share how they feel",
            //                Url = this._configuration["BaseUri"]
            //            },
            //        },
            //    };

            //    return response;
            //}
            //else
            //{
            //    var response = new MessagingExtensionActionResponse()
            //    {
            //        Task = new TaskModuleContinueResponse()
            //        {
            //            Value = new TaskModuleTaskInfo()
            //            {
            //                Height = 720,
            //                Width = 900,
            //                Title = "Invite people to share how they feel",
            //                Url = this._configuration["BaseUri"] + "/ManageRecurringPosts"
            //            },
            //        },
            //    };

            //    return response;
            //}
        }

        protected override Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
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

        private static MessagingExtensionAttachment GetAttachment(string title)
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

        protected override Task<MessagingExtensionResponse> OnTeamsMessagingExtensionSelectItemAsync(ITurnContext<IInvokeActivity> turnContext, JObject query, CancellationToken cancellationToken)
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

        protected async override Task OnTeamsMessagingExtensionCardButtonClickedAsync(ITurnContext<IInvokeActivity> turnContext, JObject cardData, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OnTeamsMessagingExtensionCardButtonClickedAsync Value: " + JsonConvert.SerializeObject(turnContext.Activity.Value));
            await turnContext.SendActivityAsync(reply, cancellationToken);

            //return base.OnTeamsMessagingExtensionCardButtonClickedAsync(turnContext, cardData, cancellationToken);
        }

    }
}

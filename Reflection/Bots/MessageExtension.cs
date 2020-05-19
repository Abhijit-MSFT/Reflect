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
using Reflection.Repositories.ReflectionData;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using Reflection.Bots;

namespace Microsoft.Teams.Samples.HelloWorld.Web
{
    //public class MessageExtension<T> : TeamsActivityHandler where T : Dialog
    public class MessageExtension : TeamsActivityHandler 
    {
        private readonly IConfiguration _configuration;

        public MessageExtension(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            //Logger.LogInformation("Running dialog with Message Activity.");

            //// Run the Dialog with the new message Activity.
            //await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);

            CardHelper cardhelper = new CardHelper(_configuration);

            FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration);
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration);
            if (turnContext.Activity.Value != null)
            {
                var response = JsonConvert.DeserializeObject<UserfeedbackInfo>(turnContext.Activity.Value.ToString());
                var reply = Activity.CreateMessageActivity();

                if (response.type == "saveFeedback")
                {
                    
                    response.userName = turnContext.Activity.From.Name;
                    response.emailId = await DBHelper.GetUserEmailId(turnContext);

                    //Check if this is user's second feedback
                    FeedbackDataEntity feebackData = await feedbackDataRepository.GetReflectionFeedback(response.reflectionId, response.emailId);
                    if (feebackData != null && response.emailId == feebackData.FeedbackGivenBy)
                    {
                        feebackData.Feedback = response.feedbackId;
                        await feedbackDataRepository.CreateOrUpdateAsync(feebackData);
                    }
                    else
                    {
                        await DBHelper.SaveReflectionFeedbackDataAsync(response, _configuration);
                    }

                    try
                    {
                        //Get reflect data to check if mseeage id is present - if not update it
                        ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(response.reflectionId);
                        Dictionary<int, List<string>> feedbacks = await feedbackDataRepository.GetReflectionFeedback(response.reflectionId);
                        var adaptiveCard = cardhelper.FeedBackCard(feedbacks, response.reflectionId);
                        
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

        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)        
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

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            TaskInfo taskInfo = JsonConvert.DeserializeObject<TaskInfo>(action.Data.ToString());
            switch (taskInfo.action)
            {
                case "reflection":
                    return await OnTeamsMessagingExtensionFetchTaskAsync(turnContext, action, cancellationToken);
                case "sendAdaptiveCard":
                    try
                    {
                        CardHelper cardhelper = new CardHelper(_configuration);
                        taskInfo.postCreateBy = turnContext.Activity.From.Name;
                        taskInfo.postCreatedByEmail = await DBHelper.GetUserEmailId(turnContext);
                        taskInfo.channelID = turnContext.Activity.TeamsGetChannelId();
                        await DBHelper.SaveReflectionDataAsync(taskInfo, _configuration);
                        if (taskInfo.postSendNowFlag == true)
                        {
                            var adaptiveCard = cardhelper.CreateNewPostCard(taskInfo);
                            var message = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = adaptiveCard });
                            await turnContext.SendActivityAsync(message, cancellationToken);                         
                        }
                        return null;
                    }
                    catch (Exception)
                    {

                        throw;
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
            //there are 3 types of user roles available Admin - User - Guest
            //var role = await DBHelper.GetUserEmailId(turnContext);
            string url = this._configuration["BaseUri"];
            if (action.MessagePayload != null)
                url = this._configuration["BaseUri"] + "/ManageRecurringPosts";            
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

//QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration);
//var defaultQuestions = await questionsDataRepository.GetAllDefaultQuestions();

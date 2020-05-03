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
using AdaptiveCards;
using Reflection.Helper;
using Reflection.Model;
using System.Net;
using Reflection.Repositories.ReflectionData;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Bogus.DataSets;

namespace Microsoft.Teams.Samples.HelloWorld.Web
{
    public class MessageExtension : TeamsActivityHandler
    {
        private readonly IConfiguration _configuration;
        public MessageExtension(IConfiguration configuration)
        {
            _configuration = configuration;                
        }
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var val = JsonConvert.DeserializeObject<TaskInfo>(action.Data.ToString()).action;
            //var cardData = JsonConvert.DeserializeObject            

            switch (val)
            {
                case "reflection":
                    return await OnTeamsMessagingExtensionFetchTaskAsync(turnContext, action, cancellationToken);
                case "sendAdaptiveCard":
                    try
                    {
                        ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration);
                        ReflectionDataEntity reflectEntity = new ReflectionDataEntity();
                        reflectEntity.PartitionKey = "ReflectionDataEntity";
                        reflectEntity.RowKey = "270712";
                        reflectEntity.ReflectionID = new Guid();
                        reflectEntity.CreatedBy = "Arun Kumar";
                        reflectEntity.Question = "How are you feeling today?";
                        reflectEntity.Privacy = "personal";
                        reflectEntity.RecurringFlag = "false";
                        reflectEntity.ExecutionDate = DateTime.Now;
                        reflectEntity.ExecutionTime = DateTime.Now;
                        reflectEntity.RecurringFlag = "daily";
                        reflectEntity.IsActive = false;


                        //await ReflectionDataRepositoryExtensionHelper.SaveReflectionDataAsync(reflectionDataRepository, turnContext);
                        await reflectionDataRepository.CreateOrUpdateAsync(reflectEntity);

                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                    
                    //await re.SaveReflectionDataAsync()
                    return await CardHelper.SendNewPost(turnContext, action, cancellationToken, action.Data.ToString()); //create model for action data
                case "Chaining":
                    var response = new MessagingExtensionActionResponse()
                    {
                        Task = new TaskModuleContinueResponse()
                        {
                            Value = new TaskModuleTaskInfo()
                            {
                                Height = 550,
                                Width = 780,
                                Title = "Check the pulse on emotinal well-being",
                                Url = "https://bc5066ec.ngrok.io/ManageRecurringPosts"
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
            if (action.MessagePayload != null)
            {
                var messageText = action.MessagePayload.Body.Content;
                var fromId = action.MessagePayload.From.User.Id;
            }

            var response = new MessagingExtensionActionResponse()
            {
                Task = new TaskModuleContinueResponse()
                {
                    Value = new TaskModuleTaskInfo()
                    {
                        Height = 720,
                        Width = 900,
                        Title = "Invite people to share how they feel",
                        Url = "https://bc5066ec.ngrok.io"
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

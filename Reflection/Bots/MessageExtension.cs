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

namespace Microsoft.Teams.Samples.HelloWorld.Web
{
    public class MessageExtension : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Value != null)
            {
                // This was a message from the card.
                //var obj = (JObject)turnContext.Activity.Value;
                //var answer = obj["Answer"]?.ToString();
                //var choices = obj["Choices"]?.ToString();
                //await turnContext.SendActivityAsync(MessageFactory.Text($"{turnContext.Activity.From.Name} answered '{answer}' and chose '{choices}'."), cancellationToken);
                var reply = Activity.CreateMessageActivity();
                string card = "{\r\n    \"type\": \"AdaptiveCard\",\r\n    \"version\": \"1.0\",\r\n    \"body\": [\r\n        {\r\n            \"type\": \"Image\",\r\n            \"altText\": \"\",\r\n            \"url\": \"https://1f0bd229.ngrok.io/images/Firstresponsecolor.png\",\r\n            \"width\": \"\"\r\n        },\r\n        {\r\n            \"type\": \"ColumnSet\",\r\n            \"columns\": [\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref1.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref2.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"1\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref3.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref4.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref5.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\"\r\n}";

                AdaptiveCardParseResult result = AdaptiveCard.FromJson(card);
                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = result.Card
                };
                reply.Attachments.Add(attachment);
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else
            {
                // This is a regular text message.
                await turnContext.SendActivityAsync(MessageFactory.Text($"Hello from the TeamsMessagingExtensionsActionPreviewBot."), cancellationToken);
            }
        }
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var val = JsonConvert.DeserializeObject<TaskInfo>(action.Data.ToString()).action;
            //var cardData = JsonConvert.DeserializeObject
            string card = "{\r\n    \"type\": \"AdaptiveCard\",\r\n    \"version\": \"1.0\",\r\n    \"body\": [\r\n        {\r\n            \"type\": \"ColumnSet\",\r\n            \"columns\": [\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"Created by: Arun Kumar\"\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"| Responses are public\"\r\n                        }\r\n                    ]\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"type\": \"Container\",\r\n            \"items\": [\r\n                {\r\n                    \"type\": \"TextBlock\",\r\n                    \"text\": \"How are you felling about the test today?\"\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"type\": \"ColumnSet\",\r\n            \"columns\": [\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://1f0bd229.ngrok.io/images/1.png\",\r\n                                \"altText\":\"1\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://1f0bd229.ngrok.io/images/2.png\",\r\n                                \"altText\":\"2\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                        \r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://1f0bd229.ngrok.io/images/3.png\",\r\n                                \"altText\":\"3\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://1f0bd229.ngrok.io/images/4.png\",\r\n                                \"altText\":\"4\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://1f0bd229.ngrok.io/images/5.png\",\r\n                                \"altText\":\"5\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"actions\": [\r\n          {\r\n            \"type\": \"Action.Submit\",\r\n            \"title\": \"Remove Post\"\r\n          },\r\n          {\r\n            \"type\": \"Action.Submit\",\r\n            \"title\": \"Manage recurring posts\"\r\n          }\r\n        ],\r\n    \"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\"\r\n}";

            AdaptiveCardParseResult result = AdaptiveCard.FromJson(card);

            var message = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = result.Card });
            var channelId = turnContext.Activity.TeamsGetChannelId();


            // THIS WILL WORK IF THE BOT IS INSTALLED. (SendActivityAsync will throw if the bot is not installed.)
            await turnContext.SendActivityAsync(message, cancellationToken);

            // await turnContext. TeamsCreateConversationAsync(channelId, message, cancellationToken);
            return null;

            //switch (val)
            //{
            //    case "reflection":
            //        return await OnTeamsMessagingExtensionFetchTaskAsync(turnContext, action, cancellationToken);
            //    case "sendAdaptiveCard":
            //        return await CardHelper.SendNewPost(turnContext, action, cancellationToken, action.Data.ToString()); //create model for action data
            //    case "Chaining":
            //        var response = new MessagingExtensionActionResponse()
            //        {
            //            Task = new TaskModuleContinueResponse()
            //            {
            //                Value = new TaskModuleTaskInfo()
            //                {
            //                    Height = 550,
            //                    Width = 780,
            //                    Title = "Check the pulse on emotinal well-being",
            //                    Url = "https://1f0bd229.ngrok.io/ManageRecurringPosts"
            //                },
            //            },
            //        };

            //        return response;
            //    default:
            //        return null;
            //};

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
                        Url = "https://1f0bd229.ngrok.io/"
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
        //var reply = MessageFactory.Text("OnTeamsMessagingExtensionCardButtonClickedAsync Value: " + JsonConvert.SerializeObject(turnContext.Activity.Value));
            var reply = Activity.CreateMessageActivity();
            string card = "{\r\n    \"type\": \"AdaptiveCard\",\r\n    \"version\": \"1.0\",\r\n    \"body\": [\r\n        {\r\n            \"type\": \"Image\",\r\n            \"altText\": \"\",\r\n            \"url\": \"https://1f0bd229.ngrok.io/images/Firstresponsecolor.png\",\r\n            \"width\": \"\"\r\n        },\r\n        {\r\n            \"type\": \"ColumnSet\",\r\n            \"columns\": [\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref1.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref2.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"1\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref3.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref4.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref5.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\"\r\n}";

            AdaptiveCardParseResult result = AdaptiveCard.FromJson(card);
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = result.Card
            };
            reply.Attachments.Add(attachment);
            await turnContext.SendActivityAsync(reply, cancellationToken);


            //return base.OnTeamsMessagingExtensionCardButtonClickedAsync(turnContext, cardData, cancellationToken);
        }

    }
}

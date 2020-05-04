using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Reflection.Helper
{
    public class CardHelper
    {
        

        public async static Task<MessagingExtensionActionResponse> SendNewPost(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken, string Data)
     {
            //var exampleData = JsonConvert.DeserializeObject<ExampleData>(action.Data.ToString());

            //var adaptiveCard = AdaptiveCardHelper.CreateAdaptiveCard(exampleData);


            string card = "{\r\n    \"type\": \"AdaptiveCard\",\r\n    \"version\": \"1.0\",\r\n    \"body\": [\r\n        {\r\n            \"type\": \"Image\",\r\n            \"altText\": \"\",\r\n            \"url\": \"https://1f0bd229.ngrok.io/images/Firstresponsecolor.png\",\r\n            \"width\": \"\"\r\n        },\r\n        {\r\n            \"type\": \"ColumnSet\",\r\n            \"columns\": [\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref1.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref2.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"1\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref3.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref4.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref5.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\"\r\n}";

            AdaptiveCardParseResult result = AdaptiveCard.FromJson(card);

            var message = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = result });
            var channelId = turnContext.Activity.TeamsGetChannelId();


            // THIS WILL WORK IF THE BOT IS INSTALLED. (SendActivityAsync will throw if the bot is not installed.)
            await turnContext.SendActivityAsync(message, cancellationToken);

            // await turnContext. TeamsCreateConversationAsync(channelId, message, cancellationToken);
            return null;


            //  var previewedCard = JsonConvert.DeserializeObject<AdaptiveCard>(result.ToString(),
            //new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


            //  var responseActivity = Activity.CreateMessageActivity();
            //  Attachment attachment = new Attachment()
            //  {
            //      ContentType = AdaptiveCard.ContentType,
            //      Content = previewedCard
            //  };
            //  responseActivity.Attachments.Add(attachment);

            //  await turnContext.SendActivityAsync(responseActivity);
            //  return new MessagingExtensionActionResponse();

            //var attachment = new MessagingExtensionAttachment
            //{
            //    ContentType = AdaptiveCard.ContentType,
            //    Content = result.Card,
            //};



            //return await Task.FromResult(new MessagingExtensionActionResponse
            //{

            //    ComposeExtension = new MessagingExtensionResult
            //    {
            //        Type = "result",
            //        AttachmentLayout = "list",
            //        Attachments = new List<MessagingExtensionAttachment> { attachment }
            //    }
            //});
        }

        public async static Task<MessagingExtensionActionResponse> ShowResponseCard(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken, string Data)
        {
            string card = "{\r\n    \"type\": \"AdaptiveCard\",\r\n    \"version\": \"1.0\",\r\n    \"body\": [\r\n        {\r\n            \"type\": \"Image\",\r\n            \"altText\": \"\",\r\n            \"url\": \"https://1f0bd229.ngrok.io/images/Firstresponsecolor.png\",\r\n            \"width\": \"\"\r\n        },\r\n        {\r\n            \"type\": \"ColumnSet\",\r\n            \"columns\": [\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref1.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref2.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"1\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref3.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref4.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"Image\",\r\n                            \"altText\": \"\",\r\n                            \"url\": \"https://1f0bd229.ngrok.io/images/ref5.png\"\r\n                        },\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"1\"\r\n                        }\r\n                    ],\r\n                    \"width\": \"stretch\"\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\"\r\n}";

            AdaptiveCardParseResult result = AdaptiveCard.FromJson(card);
            var attachment = new MessagingExtensionAttachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = result.Card,
            };

            return await Task.FromResult(new MessagingExtensionActionResponse
            {

                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "result",
                    AttachmentLayout = "list",
                    Attachments = new List<MessagingExtensionAttachment> { attachment }
                }
            });
        }
    }
}

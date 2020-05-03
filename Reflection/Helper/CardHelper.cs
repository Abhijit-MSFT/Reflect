using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;

namespace Reflection.Helper
{
    public class CardHelper
    {
      public async static Task<MessagingExtensionActionResponse> SendNewPost(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken, string Data)
     {
            string card = "{\r\n    \"type\": \"AdaptiveCard\",\r\n    \"version\": \"1.0\",\r\n    \"body\": [\r\n        {\r\n            \"type\": \"ColumnSet\",\r\n            \"columns\": [\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"Created by: Arun Kumar\"\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"| Responses are public\"\r\n                        }\r\n                    ]\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"type\": \"Container\",\r\n            \"items\": [\r\n                {\r\n                    \"type\": \"TextBlock\",\r\n                    \"text\": \"How are you felling about the test today?\"\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"type\": \"ColumnSet\",\r\n            \"columns\": [\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://d0009dbb.ngrok.io/images/1.png\",\r\n                                \"altText\":\"1\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://d0009dbb.ngrok.io/images/2.png\",\r\n                                \"altText\":\"2\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                        \r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://d0009dbb.ngrok.io/images/3.png\",\r\n                                \"altText\":\"3\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://d0009dbb.ngrok.io/images/4.png\",\r\n                                \"altText\":\"4\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://d0009dbb.ngrok.io/images/5.png\",\r\n                                \"altText\":\"5\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"actions\": [\r\n          {\r\n            \"type\": \"Action.Submit\",\r\n            \"title\": \"Remove Post\"\r\n          },\r\n          {\r\n            \"type\": \"Action.Submit\",\r\n            \"title\": \"Manage recurring posts\"\r\n          }\r\n        ],\r\n    \"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\"\r\n}";

            AdaptiveCardParseResult result = AdaptiveCard.FromJson(card);
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

        public async static Task<MessagingExtensionActionResponse> ShowResponseCard(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken, string Data)
        {
            string card = "{\r\n    \"type\": \"AdaptiveCard\",\r\n    \"version\": \"1.0\",\r\n    \"body\": [\r\n        {\r\n            \"type\": \"ColumnSet\",\r\n            \"columns\": [\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"Created by: Arun Kumar\"\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                            \"type\": \"TextBlock\",\r\n                            \"text\": \"| Responses are public\"\r\n                        }\r\n                    ]\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"type\": \"Container\",\r\n            \"items\": [\r\n                {\r\n                    \"type\": \"TextBlock\",\r\n                    \"text\": \"How are you felling about the test today?\"\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"type\": \"ColumnSet\",\r\n            \"columns\": [\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://d0009dbb.ngrok.io/images/1.png\",\r\n                                \"altText\":\"1\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://d0009dbb.ngrok.io/images/2.png\",\r\n                                \"altText\":\"2\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                        \r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://d0009dbb.ngrok.io/images/3.png\",\r\n                                \"altText\":\"3\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://d0009dbb.ngrok.io/images/4.png\",\r\n                                \"altText\":\"4\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                },\r\n                {\r\n                    \"type\": \"Column\",\r\n                    \"width\": \"stretch\",\r\n                    \"items\": [\r\n                        {\r\n                                \"type\": \"Image\",\r\n                                \"url\": \"https://d0009dbb.ngrok.io/images/5.png\",\r\n                                \"altText\":\"5\",\r\n                                \"selectAction\": {\r\n                                                    \"type\": \"Action.Submit\"\r\n                                                }\r\n                        }\r\n                    ]\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"actions\": [\r\n          {\r\n            \"type\": \"Action.Submit\",\r\n            \"title\": \"Remove Post\"\r\n          },\r\n          {\r\n            \"type\": \"Action.Submit\",\r\n            \"title\": \"Manage recurring posts\"\r\n          }\r\n        ],\r\n    \"$schema\": \"http://adaptivecards.io/schemas/adaptive-card.json\"\r\n}";

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

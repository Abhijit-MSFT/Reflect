using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Teams.Samples.HelloWorld.Web;

namespace Reflection.Helper
{
    public class CardHelper
    {
      public async static Task<MessagingExtensionActionResponse> DefaultCard(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var adaptiveCard1 = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock()
                    {
                        Text = "https://docs.microsoft.com/en-us/graph/api/user-post-events?view=graph-rest-beta&tabs=http#example-5-create-and-enable-an-event-as-an-online-meeting"
                    }
                    //Posted Date- Add value here
                }
            };

            var attachment = new MessagingExtensionAttachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard1,
                // Preview=res.ToAttachment()
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

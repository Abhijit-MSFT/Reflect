// <copyright file="ProactiveMessageHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Reflection.Model;
using System;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.Extensions.Configuration;

namespace Reflection.Helper
{
    public class ProactiveMessageHelper
    {
        private IConfiguration _configuration;

        public ProactiveMessageHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Method is used to send the proactive messages based on recurssion 
        /// </summary>
        /// <param name="botAccount">ChannelAccount from Microsoft.Bot.Schema </param>
        /// <param name="serviceUrl">serviceUrl</param>
        /// <param name="channelId">ChannelID to which channel the notification is to be sent</param>
        /// <param name="messageText"></param>
        /// <param name="attachment">Adaptive card attachement</param>
        /// <returns>Returns notification data in NotificationSendStatus model</returns>
        public async Task<NotificationSendStatus> SendChannelNotification(ChannelAccount botAccount, string serviceUrl, string channelId, string messageText, Attachment attachment)
        {
            try
            {
                var replyMessage = Activity.CreateMessageActivity();
                replyMessage.Text = messageText;

                if (attachment != null)
                    replyMessage.Attachments.Add(attachment);
                MicrosoftAppCredentials.TrustServiceUrl(serviceUrl, DateTime.MaxValue);
                using var connectorClient = new ConnectorClient(new Uri(serviceUrl), _configuration["MicrosoftAppId"], _configuration["MicrosoftAppPassword"]);
                var parameters = new ConversationParameters
                {
                    Bot = botAccount,
                    ChannelData = new TeamsChannelData
                    {
                        Channel = new ChannelInfo(channelId),
                        Notification = new NotificationInfo() { Alert = true }
                    },
                    IsGroup = true,
                    Activity = (Activity)replyMessage
                };

                ExponentialBackoff exponentialBackoffRetryStrategy = new ExponentialBackoff(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(1));

                // Define the Retry Policy
                var retryPolicy = new RetryPolicy(new BotSdkTransientExceptionDetectionStrategy(), exponentialBackoffRetryStrategy);

                var conversationResource = await connectorClient.Conversations.CreateConversationAsync(parameters);

                return new NotificationSendStatus() { MessageId = conversationResource.Id, IsSuccessful = true };
            }
            catch (Exception ex)
            {                
                return new NotificationSendStatus() { IsSuccessful = false, FailureMessage = ex.Message };
            }
        }
    }
}

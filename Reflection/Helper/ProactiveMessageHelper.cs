using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Reflection.Model;
using System;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace Reflection.Helper
{
    public class ProactiveMessageHelper
    {

        public static async Task<NotificationSendStatus> SendPersonalNotification(string serviceUrl, string tenantId, User userDetails, string messageText, Attachment attachment)
        {
            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl, DateTime.MaxValue);
            var connectorClient = new ConnectorClient(new Uri(serviceUrl));
            if (string.IsNullOrEmpty(userDetails.PersonalConversationId))
            {
                var createConversationResult = await GetConversationId(connectorClient, tenantId, userDetails.BotConversationId);
                if (createConversationResult.IsSuccessful)
                {
                    userDetails.PersonalConversationId = createConversationResult.MessageId;
                    //await Cache.Users.AddOrUpdateItemAsync(userDetails.Id, userDetails);
                }
                else
                    return createConversationResult; // Failed
            }

            return await SendNotificationToConversationId(connectorClient, tenantId, userDetails.PersonalConversationId, messageText, attachment);

        }

        private static async Task<NotificationSendStatus> SendNotificationToConversationId(ConnectorClient connectorClient, string tenantId, string conversationId, string messageText, Attachment attachment)
        {

            try
            {
                var replyMessage = Activity.CreateMessageActivity();

                replyMessage.Conversation = new ConversationAccount(id: conversationId);
                replyMessage.ChannelData = new TeamsChannelData() { Notification = new NotificationInfo(true) };
                replyMessage.Text = messageText;
                if (attachment != null)
                    replyMessage.Attachments.Add(attachment);

                var exponentialBackoffRetryStrategy = new ExponentialBackoff(5, TimeSpan.FromSeconds(2),
                       TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(1));

                // Define the Retry Policy
                var retryPolicy = new RetryPolicy(new BotSdkTransientExceptionDetectionStrategy(), exponentialBackoffRetryStrategy);

                var resourceResponse = await retryPolicy.ExecuteAsync(() =>
                                        connectorClient.Conversations.SendToConversationAsync(conversationId, (Activity)replyMessage)
                                        ).ConfigureAwait(false);

                //var resourceResponse = await
                //                       connectorClient.Conversations.SendToConversationAsync(conversationId, (Activity)replyMessage)
                //                       ;

                return new NotificationSendStatus() { MessageId = resourceResponse.Id, IsSuccessful = true };
            }
            catch (Exception ex)
            {
                //ErrorLogService.LogError(ex);
                return new NotificationSendStatus() { IsSuccessful = false, FailureMessage = ex.Message };
            }

        }

        private static async Task<NotificationSendStatus> GetConversationId(ConnectorClient connectorClient, string tenantId, string userId)
        {
            var parameters = new ConversationParameters
            {
                Members = new ChannelAccount[] { new ChannelAccount(userId) },
                ChannelData = new TeamsChannelData
                {
                    Tenant = new TenantInfo(tenantId),
                    Notification = new NotificationInfo() { Alert = true },

                },
                IsGroup = false,
                //Bot = new ChannelAccount(ApplicationSettings.AppId)
            };

            try
            {
                var exponentialBackoffRetryStrategy = new ExponentialBackoff(5, TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(1));


                // Define the Retry Policy
                var retryPolicy = new RetryPolicy(new BotSdkTransientExceptionDetectionStrategy(), exponentialBackoffRetryStrategy);

                var conversationResource = await retryPolicy.ExecuteAsync(() =>
                                            connectorClient.Conversations.CreateConversationAsync(parameters)
                                            ).ConfigureAwait(false);

                //var conversationResource = await
                //                            connectorClient.Conversations.CreateConversationAsync(parameters)
                //                            ;

                return new NotificationSendStatus() { MessageId = conversationResource.Id, IsSuccessful = true };
            }
            catch (Exception ex)
            {
                // Handle the error.
                //ErrorLogService.LogError(ex);
                return new NotificationSendStatus() { IsSuccessful = false, FailureMessage = ex.Message };
            }

        }

        public static async Task<NotificationSendStatus> SendChannelNotification(ChannelAccount botAccount, string serviceUrl, string channelId, string messageText, Attachment attachment, TeamsChannelData channelData)
        {
            try
            {
                var replyMessage = Activity.CreateMessageActivity();
                replyMessage.Text = messageText;

                if (attachment != null)
                    replyMessage.Attachments.Add(attachment);

                using (var connectorClient = new ConnectorClient(new Uri(serviceUrl)))
                {
                    var parameters = new ConversationParameters
                    {
                        Bot = botAccount,
                        ChannelData = new TeamsChannelData
                        {
                            Channel = new ChannelInfo(channelId),
                            Tenant = channelData.Tenant,
                            Notification = new NotificationInfo() { Alert = true }
                        },
                        IsGroup = true,
                        Activity = (Activity)replyMessage
                    };

                    var exponentialBackoffRetryStrategy = new ExponentialBackoff(3, TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(1));


                    // Define the Retry Policy
                    var retryPolicy = new RetryPolicy(new BotSdkTransientExceptionDetectionStrategy(), exponentialBackoffRetryStrategy);

                    //var conversationResource = await retryPolicy.ExecuteAsync(() =>
                    //                        connectorClient.Conversations.CreateConversationAsync(parameters)
                    //                        ).ConfigureAwait(false);

                    var conversationResource = await
                                            connectorClient.Conversations.CreateConversationAsync(parameters)
                                            ;

                    return new NotificationSendStatus() { MessageId = conversationResource.Id, IsSuccessful = true };
                }
            }
            catch (Exception ex)
            {
                //ErrorLogService.LogError(ex);
                return new NotificationSendStatus() { IsSuccessful = false, FailureMessage = ex.Message };
            }
        }
    }
}

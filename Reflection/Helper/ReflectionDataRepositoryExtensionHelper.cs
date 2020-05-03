using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using Reflection.Repositories;
using Reflection.Repositories.ReflectionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Helper
{
    public static class ReflectionDataRepositoryExtensionHelper
    {
        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="reflectionDataRepository">The reflection data repository.</param>
        /// <param name="turnContext">Bot conversation update activity instance.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public static async Task SaveReflectionDataAsync(
            ReflectionDataRepository reflectionDataRepository, ITurnContext<IInvokeActivity> turnContext)
        {
            var reflectionDataEntity = await ReflectionDataRepositoryExtensionHelper.ParseReflectionData(turnContext);
            if(reflectionDataEntity != null)
            {
                await reflectionDataRepository.CreateOrUpdateAsync(reflectionDataEntity);
            }
        }

        private static async Task<ReflectionDataEntity> ParseReflectionData(ITurnContext<IInvokeActivity> turnContext)
        {
            var row = turnContext.Activity?.From?.AadObjectId;
            if(row != null)
            {
                var reflectionDataEntity = new ReflectionDataEntity
                {
                    PartitionKey = PartitionKeyNames.ReflectionDataTable.UserDataPartition,
                    RowKey = turnContext.Activity?.From?.AadObjectId,
                    //CreatedBy = await GetUserName(turnContext),
                    CreatedBy = await GetUserEmailId(turnContext),
                };

                return reflectionDataEntity;
            }

            return null;
        }

        private static async Task<string> GetUserEmailId(ITurnContext<IInvokeActivity> turnContext)
        {
            // Fetch the members in the current conversation
            try
            {
                IConnectorClient connector = turnContext.TurnState.Get<IConnectorClient>();
                
                var members = await connector.Conversations.GetConversationMembersAsync(turnContext.Activity.Conversation.Id);
                return AsTeamsChannelAccounts(members).FirstOrDefault(m => m.Id == turnContext.Activity.From.Id).UserPrincipalName;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static IEnumerable<TeamsChannelAccount> AsTeamsChannelAccounts(IEnumerable<ChannelAccount> channelAccountList)
        {
            foreach (ChannelAccount channelAccount in channelAccountList)
            {
                yield return JObject.FromObject(channelAccount).ToObject<TeamsChannelAccount>();
            }
        }

        public static async Task GetAllReflections()
        {

        }
    }
}

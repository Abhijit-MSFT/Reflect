using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Repositories.FeedbackData
{
    using Microsoft.Extensions.Configuration;

    public class FeedbackDataRepository : BaseRepository<FeedbackDataEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataRepository"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public FeedbackDataRepository(IConfiguration configuration, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.FeedbackDataTable.TableName,
                PartitionKeyNames.FeedbackDataTable.FeedbackDataPartition,
                isFromAzureFunction)
        {
        }


        /// <summary>
        /// Get the default questions.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Questions which have default flag true</returns>
        /// 
        public async Task<Dictionary<int, int>> GetReflectionFeedback(Guid? reflectionid)
        {
            var allFeedbacks = await this.GetAllAsync(PartitionKeyNames.FeedbackDataTable.TableName);
            var feedbackResult = allFeedbacks.Where(d => d.ReflectionID == reflectionid);
            Dictionary<int, int> feeds = new Dictionary<int, int>();
            feeds = feedbackResult.GroupBy(x => x.Feedback).ToDictionary(x=>x.Key,x=>x.Count());
            return feeds;
        }

    }
}

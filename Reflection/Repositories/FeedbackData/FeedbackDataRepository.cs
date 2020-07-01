using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Repositories.FeedbackData
{
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;

    public class FeedbackDataRepository : BaseRepository<FeedbackDataEntity>
    {
        private TelemetryClient _telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataRepository"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public FeedbackDataRepository(IConfiguration configuration, TelemetryClient telemetry, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.FeedbackDataTable.TableName,
                PartitionKeyNames.FeedbackDataTable.FeedbackDataPartition,
                isFromAzureFunction)
        {
            _telemetry = telemetry;
        }


        /// <summary>
        /// Get the default questions.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Questions which have default flag true</returns>
        /// 
        public async Task<Dictionary<int, List<FeedbackDataEntity>>> GetReflectionFeedback(Guid? reflectionId)
        {
            _telemetry.TrackEvent("GetReflectionFeedback");

            try
            {
                var allFeedbacks = await this.GetAllAsync(PartitionKeyNames.FeedbackDataTable.TableName);
                var feedbackResult = allFeedbacks.Where(d => d.ReflectionID == reflectionId);
                Dictionary<int, List<FeedbackDataEntity>> feeds = new Dictionary<int, List<FeedbackDataEntity>>();
                feeds = feedbackResult.GroupBy(x => x.Feedback).ToDictionary(x => x.Key, x => x.ToList());
                return feeds;
            }
            catch (Exception ex)
            {

                _telemetry.TrackException(ex);
                return null;

            }
        }

        public async Task<FeedbackDataEntity> GetFeedbackonRefId(Guid? reflid)
        {
            _telemetry.TrackEvent("GetReflectionFeedback");
            try
            {
                var allFeedbacks = await this.GetAllAsync(PartitionKeyNames.FeedbackDataTable.TableName);
                FeedbackDataEntity feedbackResult = allFeedbacks.Where(c => c.ReflectionID == reflid).FirstOrDefault();
                return feedbackResult ?? null;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;

            }
        }

        public async Task<FeedbackDataEntity> GetReflectionFeedback(Guid reflid, string email)
        {
            _telemetry.TrackEvent("GetReflectionFeedback");
            try
            {
                var allFeedbacks = await this.GetAllAsync(PartitionKeyNames.FeedbackDataTable.TableName);
                FeedbackDataEntity feedbackResult = allFeedbacks.Where(c => c.ReflectionID == reflid && c.FeedbackGivenBy == email).FirstOrDefault();
                return feedbackResult ?? null;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;

            }
        }

        public async Task<string>  DeleteFeedback(FeedbackDataEntity feedback)
        {
            _telemetry.TrackEvent("DeleteFeedback");
            try
            {
                await this.DeleteAsync(feedback);
                return "true";
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return "false";

            }
        }
    }
}

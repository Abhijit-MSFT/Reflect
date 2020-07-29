// -----------------------------------------------------------------------
// <copyright file="FeedbackDataRepository.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories.FeedbackData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// FeedbackData Repository.
    /// </summary>
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
        /// Get reflection feedback.
        /// </summary>
        /// <param name="reflectionId">reflectionId.</param>
        /// <returns>Questions which have default flag true.</returns>
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

        /// <summary>
        /// Get reflection ref id.
        /// </summary>
        /// <param name="reflid">reflid.</param>
        /// <returns>.</returns>
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

        /// <summary>
        /// Get Reflection Feedback.
        /// </summary>
        /// <param name="reflid">reflid.</param>
        /// <param name="email">email.</param>
        /// <returns>.</returns>
        public async Task<FeedbackDataEntity> GetReflectionFeedback(Guid? reflid, string email)
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

        /// <summary>
        /// Delete Feedback.
        /// </summary>
        /// <param name="feedback">feedback.</param>
        /// <returns>.</returns>
        public async Task<string> DeleteFeedback(FeedbackDataEntity feedback)
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

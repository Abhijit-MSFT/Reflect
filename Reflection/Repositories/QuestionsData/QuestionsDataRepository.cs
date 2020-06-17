// <copyright file="QuestionDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Repositories.QuestionsData
{
    public class QuestionsDataRepository : BaseRepository<QuestionsDataEntity>
    {
        private TelemetryClient _telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionsDataEntity"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public QuestionsDataRepository(IConfiguration configuration, TelemetryClient telemetry, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.QuestionsDataTable.TableName,
                PartitionKeyNames.QuestionsDataTable.QuestionsDataPartition,
                isFromAzureFunction)
        {
            _telemetry = telemetry;

        }

        /// <summary>
        /// Get the default questions.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Questions which have default flag true<</returns>
        public async Task<List<QuestionsDataEntity>> GetAllDefaultQuestionsForUser(string userEmail)
        {
            _telemetry.TrackEvent("GetAllDefaultQuestionsForUser");

            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                var result = allRows.Where(d => d.IsDefaultFlag == true || d.CreatedByEmail == userEmail);
                return result.ToList();
            }
            catch (Exception ex)
            {

                _telemetry.TrackException(ex);
                return null;

            }
        }

        /// <summary>
        /// Get questions by id.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Get questions by question id</returns>
        public async Task<List<QuestionsDataEntity>> GetQuestionsByQID(Guid? qID)
        {
            _telemetry.TrackEvent("GetQuestionsByQID");

            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                var result = allRows.Where(d => d.IsDefaultFlag == true || d.QuestionID == qID);
                return result.ToList();
            }
            catch (Exception ex)
            {

                _telemetry.TrackException(ex);
                return null;

            }
        }

        /// <summary>
        /// Get all the questions.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Get all questions</returns>
        public async Task<List<QuestionsDataEntity>> GetAllQuestionData(List<Guid?> quesID)
        {
            _telemetry.TrackEvent("GetAllQuestionData");
            try
            {

                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                List<QuestionsDataEntity> result = allRows.Where(c => quesID.Contains(c.QuestionID)).ToList();
                return result ?? null;
            }
            catch (Exception ex)
            {

                _telemetry.TrackException(ex);
                return null;

            }

        }

        /// <summary>
        /// Check if question is already present.
        /// </summary>
        /// <param name=""></param>
        /// <returns>True/False - question is present/not present</returns>
        public async Task<bool> IsQuestionAlreadtPresent(string question, string email)
        {
            _telemetry.TrackEvent("IsQuestionAlreadtPresent");
            try
            {

                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                var result = allRows.Where(c => c.Question == question);

                if (result.Any(c => c.IsDefaultFlag == true || c.CreatedByEmail == email))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        /// Get all the questions.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Get all questions</returns>
        public async Task<QuestionsDataEntity> GetQuestionData(Guid? quesID)
        {
            _telemetry.TrackEvent("GetQuestionData");
            try
            {

                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                QuestionsDataEntity result = allRows.Where(c => c.QuestionID == quesID).FirstOrDefault();
                return result ?? null;
            }
            catch (Exception ex)
            {

                _telemetry.TrackException(ex);
                return null;

            }
        }

    }


}



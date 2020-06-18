// <copyright file="ReflectionDataRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Reflection.Repositories.ReflectionData
{
    public class ReflectionDataRepository : BaseRepository<ReflectionDataEntity>
    {
        private TelemetryClient _telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataRepository"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public ReflectionDataRepository(IConfiguration configuration, TelemetryClient telemetry, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.ReflectionDataTable.TableName,
                PartitionKeyNames.ReflectionDataTable.ReflectionDataPartition,
                isFromAzureFunction)
        {
            _telemetry = telemetry;
        }

        /// <summary>
        /// Get reflectiondata by reflection id.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Refleaction data</returns>
        public async Task<ReflectionDataEntity> GetReflectionData(Guid? refID)
        {
            _telemetry.TrackEvent("GetReflectionData");
            try
            {
                var allReflections = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
                ReflectionDataEntity refData = allReflections.Where(c => c.ReflectionID == refID).FirstOrDefault();
                return refData;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get reflection data by messageid.
        /// </summary>
        /// <param name=""></param>
        /// <returns>receurssion data interval wise for give reflection id</returns>
        public async Task<ReflectionDataEntity> GetReflectionData(string reflectMessagId)
        {
            _telemetry.TrackEvent("GetReflectionData");
            try
            {
                var allReflections = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
                ReflectionDataEntity refData = allReflections.Where(c => c.ReflectMessageId == reflectMessagId).FirstOrDefault();
                return refData;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }


        /// <summary>
        /// Get all active reflections by email id.
        /// </summary>
        /// <param name=""></param>
        /// <returns>all active reflection data</returns>
        public async Task<List<ReflectionDataEntity>> GetAllActiveReflection(string email)
        {
            _telemetry.TrackEvent("GetAllActiveReflection");
            try
            {
                var allRefs = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
                List<ReflectionDataEntity> RefDataEntity = allRefs.Where(c => c.IsActive == true && c.CreatedByEmail == email).ToList();
                return RefDataEntity;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }

        }
    }
}

using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Reflection.Repositories.RecurssionData
{
    public class RecurssionDataRepository : BaseRepository<RecurssionDataEntity>
    {
        private TelemetryClient _telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurssionDataEntity"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public RecurssionDataRepository(IConfiguration configuration, TelemetryClient telemetry, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.RecurssionDataTable.TableName,
                PartitionKeyNames.RecurssionDataTable.RecurssionDataPartition,
                isFromAzureFunction)
        {
            _telemetry = telemetry;
        }
        public async Task<List<RecurssionDataEntity>> GetAllRecurssionData(List<Guid?> RefIds)
        {
            _telemetry.TrackEvent("GetAllRecurssionData");

            try
            {


                var allRows = await this.GetAllAsync(PartitionKeyNames.RecurssionDataTable.TableName);
                List<RecurssionDataEntity> result = allRows.Where(c => RefIds.Contains(c.ReflectionID) && c.RecursstionType != "Does not repeat").ToList();
                return result;
            }
            catch (Exception ex)
            {

                _telemetry.TrackException(ex);
                return null;

            }
        }

        public async Task<RecurssionDataEntity> GetRecurssionData(Guid? RecurssionId)
        {
            _telemetry.TrackEvent("GetRecurssionData");
            try
            {


                var allRows = await this.GetAllAsync(PartitionKeyNames.RecurssionDataTable.TableName);
                RecurssionDataEntity result = allRows.Where(c => c.RecurssionID == RecurssionId).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {

                _telemetry.TrackException(ex);
                return null;

            }
        }

        public async Task<List<RecurssionDataEntity>> GetAllRecurssionData()
        {
            DateTime dateTime = DateTime.UtcNow;
            _telemetry.TrackEvent("GetAllRecurssionData");
            try
            {
                var recurssionData = await this.GetAllAsync(PartitionKeyNames.RecurssionDataTable.TableName);
                var recData = recurssionData.Where(c => c.NextExecutionDate != null).ToList();
                var intervalRecords = recData.Where(r => dateTime.Subtract((DateTime)r.NextExecutionDate).TotalSeconds < 60 && dateTime.Subtract((DateTime)r.NextExecutionDate).TotalSeconds > 0).ToList();
                return intervalRecords;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;

            }
        }
    }
}
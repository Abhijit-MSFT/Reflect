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
        /// <summary>
        /// Initializes a new instance of the <see cref="RecurssionDataEntity"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public RecurssionDataRepository(IConfiguration configuration, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.RecurssionDataTable.TableName,
                PartitionKeyNames.RecurssionDataTable.RecurssionDataPartition,
                isFromAzureFunction)
        {
        }
        public async Task<List<RecurssionDataEntity>> GetAllRecurssionData(List<Guid?> RefIds)
        {
            var allRows = await this.GetAllAsync(PartitionKeyNames.RecurssionDataTable.TableName);
            List<RecurssionDataEntity> result = allRows.Where(c => RefIds.Contains(c.ReflectionID) && c.RecursstionType!= "Does not repeat").ToList();
            return result;
        }
        public async Task<RecurssionDataEntity> GetRecurssionData(Guid? RecurssionId)
        {
            var allRows = await this.GetAllAsync(PartitionKeyNames.RecurssionDataTable.TableName);
            RecurssionDataEntity result = allRows.Where(c => c.RecurssionID== RecurssionId).FirstOrDefault();
            return result;
        }



    }
}
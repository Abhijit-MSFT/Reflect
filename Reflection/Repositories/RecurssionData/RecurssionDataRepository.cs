using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
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
        public async Task<IEnumerable<RecurssionDataEntity>> GetRecurrionData()
        {
            var allRecurssions = await this.GetAllAsync(PartitionKeyNames.RecurssionDataTable.TableName);
            return allRecurssions;
        }

    }
}
using Microsoft.Extensions.Configuration;

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
    }
}
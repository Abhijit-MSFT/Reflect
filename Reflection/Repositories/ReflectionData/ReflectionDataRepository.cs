


namespace Reflection.Repositories.ReflectionData
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ReflectionDataRepository : BaseRepository<ReflectionDataEntity>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataRepository"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public ReflectionDataRepository(IConfiguration configuration, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.ReflectionDataTable.TableName,
                PartitionKeyNames.ReflectionDataTable.ReflectionDataPartition,
                isFromAzureFunction)
        {
        }

        public async Task<ReflectionDataEntity> GetReflectionData(Guid refID)
        {
            var allReflections = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
            ReflectionDataEntity refData = allReflections.Where(c => c.ReflectionID == refID).FirstOrDefault();
            return refData;
        }
    }
}

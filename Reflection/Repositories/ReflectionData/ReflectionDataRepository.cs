using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Reflection.Repositories.ReflectionData
{
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

        public async Task<ReflectionDataEntity> GetReflectionData(Guid? refID)
        {
            var allReflections = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
            ReflectionDataEntity refData = allReflections.Where(c => c.ReflectionID == refID).FirstOrDefault();
            return refData;
        }

        public async Task<string> GetmessageIdfromReflection(Guid refId)
        {
            var allRefs = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
            var dataEntity = allRefs.Where(c => c.ReflectionID == refId).FirstOrDefault();
            return dataEntity.MessageID;
        }
        public async Task<List<ReflectionDataEntity>> GetAllActiveReflection(string email)
        {
            var allRefs = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
            List<ReflectionDataEntity> RefDataEntity = allRefs.Where(c => c.IsActive == true && c.CreatedByEmail== email).ToList();
            return RefDataEntity;
        }
    }
}

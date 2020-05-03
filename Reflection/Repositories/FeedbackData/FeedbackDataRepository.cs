using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Repositories.FeedbackData
{
    using Microsoft.Extensions.Configuration;

    public class FeedbackDataRepository : BaseRepository<FeedbackDataEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataRepository"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public FeedbackDataRepository(IConfiguration configuration, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.UserDataTable.TableName,
                PartitionKeyNames.UserDataTable.UserDataPartition,
                isFromAzureFunction)
        {
        }
    }
}

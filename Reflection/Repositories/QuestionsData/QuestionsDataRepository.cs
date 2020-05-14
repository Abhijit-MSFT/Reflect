using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Repositories.QuestionsData
{
    public class QuestionsDataRepository : BaseRepository<QuestionsDataEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionsDataEntity"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public QuestionsDataRepository(IConfiguration configuration, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.QuestionsDataTable.TableName,
                PartitionKeyNames.QuestionsDataTable.QuestionsDataPartition,
                isFromAzureFunction)
        {
        }

        /// <summary>
        /// Get the default questions.
        /// </summary>
        /// <param name=""></param>
        /// <returns>Questions which have default flag true</returns>
        /// 
        public async Task<List<string>> GetAllDefaultQuestions()
        {
            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                var result = allRows.Where(d => d.IsDefaultFlag == true).Select(c => c.Question);
                return result.ToList();
            }
            catch (System.Exception e)
            {
                return null;
            }
        }

        public async Task<List<QuestionsDataEntity>> GetAllQuestionData(List<Guid?> quesID)
        {
            var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
            List<QuestionsDataEntity> result = allRows.Where(c => quesID.Contains(c.QuestionID)).ToList();
            return result ?? null;
        }

        public async Task<bool> IsQuestionAlreadyPresent(string question)
        {
            var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
            bool result = allRows.Any(c => c.Question == question);
            return result;
        }
    }


}



using Microsoft.Extensions.Configuration;
using Reflection.Helper;
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
        public async Task<List<QuestionsDataEntity>> GetAllDefaultQuestionsForUser(string userEmail)
        {
            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                var result = allRows.Where(d => d.IsDefaultFlag == true || d.CreatedByEmail == userEmail);
                return result.ToList();
            }
            catch (System.Exception e)
            {
                return null;
            }
        }

        public async Task<List<QuestionsDataEntity>> GetQuestionsByQID(Guid? qID)
        {
            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                var result = allRows.Where(d => d.IsDefaultFlag == true || d.QuestionID == qID);
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

        public async Task<bool> IsQuestionAlreadtPresent(string question, string email)
        {
            var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
            //var result = allRows.Any(c => c.Question == question && c.IsDefaultFlag == true ? true : c.CreatedByEmail == email);
            var result = allRows.Where(c => c.Question == question);

            if (result.Any(c => c.IsDefaultFlag == true || c.CreatedByEmail == email))
                return true;

            return false;
        }

    }


}



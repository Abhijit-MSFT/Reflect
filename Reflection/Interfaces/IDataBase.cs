using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Reflection.Model;
using Reflection.Repositories.RecurssionData;
using Reflection.Repositories.ReflectionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Interfaces
{
    public interface IDataBase
    {
        Task SaveReflectionDataAsync(TaskInfo taskInfo);
        Task SaveQuestionsDataAsync(TaskInfo taskInfo);
        Task SaveRecurssionDataAsync(TaskInfo taskInfo);
        Task UpdateRecurssionDataNextExecutionDateTimeAsync(RecurssionDataEntity recurssionEntity);
        Task UpdateReflectionMessageIdAsync(ReflectionDataEntity reflectionDataEntity);
        Task SaveReflectionFeedbackDataAsync(UserfeedbackInfo taskInfo);
        Task<string> GetUserEmailId<T>(ITurnContext<T> turnContext) where T : Microsoft.Bot.Schema.IActivity;
        Task<ViewReflectionsEntity> GetViewReflectionsData(Guid reflectionId);
        Task<List<RecurssionScreenData>> GetRecurrencePostsDataAsync(string email);
        Task DeleteRecurrsionDataAsync(Guid reflectionId);

        Task SaveEditRecurssionDataAsync(RecurssionScreenData reflection);

        Task<string> RemoveReflectionId(string reflectMessageId);
    }
}

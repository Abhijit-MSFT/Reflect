// <copyright file="IDataBase.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.Bot.Builder;
using Reflection.Model;
using Reflection.Repositories.RecurssionData;
using Reflection.Repositories.ReflectionData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reflection.Interfaces
{
    public interface IDataBase
    {
        Task SaveReflectionDataAsync(TaskInfo taskInfo);
        Task SaveQuestionsDataAsync(TaskInfo taskInfo);
        Task SaveRecurssionDataAsync(TaskInfo taskInfo);
        Task SaveReflectionFeedbackDataAsync(UserfeedbackInfo taskInfo);
        Task SaveEditRecurssionDataAsync(RecurssionScreenData reflection);
        Task<ViewReflectionsEntity> GetViewReflectionsData(Guid reflectionId);
        Task<List<RecurssionScreenData>> GetRecurrencePostsDataAsync(string email);
        Task UpdateReflectionMessageIdAsync(ReflectionDataEntity reflectionDataEntity);
        Task UpdateRecurssionDataNextExecutionDateTimeAsync(RecurssionDataEntity recurssionEntity);
        Task DeleteRecurrsionDataAsync(Guid reflectionId);
        Task<string> RemoveReflectionId(string reflectMessageId);
        Task<string> GetUserEmailId<T>(ITurnContext<T> turnContext) where T : Microsoft.Bot.Schema.IActivity;

        string Encrypt(string text);

        string Decrypt(string text);
    }
}
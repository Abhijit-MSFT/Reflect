
// <copyright file="ICard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using AdaptiveCards;
using Reflection.Model;
using Reflection.Repositories.FeedbackData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Reflection.Interfaces
{
    public interface ICard
    {
        AdaptiveCard FeedBackCard(Dictionary<int, List<FeedbackDataEntity>> keyValues, Guid reflectionId);
        Task<string> saveImage(Image data, string Filepath);
        AdaptiveCard CreateNewReflect(TaskInfo data, int FeedbackId);

    }
}
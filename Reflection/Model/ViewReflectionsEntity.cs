using Reflection.Repositories.QuestionsData;
using Reflection.Repositories.ReflectionData;
using System.Collections.Generic;


namespace Reflection.Model
{
    public class ViewReflectionsEntity
    {
        public ReflectionDataEntity ReflectionData { get; set; }
        public Dictionary<int, int> FeedbackData { get; set; }

        public QuestionsDataEntity Question { get; set; }
    }
}

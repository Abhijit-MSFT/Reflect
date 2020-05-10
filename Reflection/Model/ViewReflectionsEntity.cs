using Reflection.Repositories.ReflectionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Model
{
    public class ViewReflectionsEntity
    {
        public ReflectionDataEntity ReflectionData { get; set; }
        public Dictionary<int, int> FeedbackData { get; set; }
    }
}

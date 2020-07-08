using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Reflection.Model
{
    public class TaskInfo
    {
        public string question { get; set; }
        public string privacy { get; set; }
        public string postCreateBy { get; set; }

        public string postCreatedByEmail { get; set; }

        public DateTime executionDate { get; set; }
        public string executionTime { get; set; }
        public DateTime? nextExecutionDate { get; set; }
        public DateTime? postDate { get; set; }
        [DefaultValue(false)]
        public bool postSendNowFlag { get; set; }
        public string repeatFrequency { get; set; }
        public string recurssionType { get; set; }
        public string customRecurssionTypeValue { get; set; }

        public bool IsActive { get; set; }
        public string action { get; set; }
        public int? card { get; set; }
        public int? userResponce { get; set; }
        public string messageID { get; set; }
        public string channelID { get; set; }
        public Guid? reflectionID { get; set; }
        public Guid? questionID { get; set; }
        public Guid? recurssionID { get; set; }
        public bool isDefaultQuestion { get; set; }

        public string reflectionRowKey { get; set; }

        public string recurrsionRowKey { get; set; }

        public string questionRowKey { get; set; }
        public string type { get; set; }

        public string reflectMessageId { get; set; }
        public string teantId { get; set; }
        public string serviceUrl { get; set; }

        public int feedback { get; set; }
    }

    public class UserfeedbackInfo
    {
        public int feedbackId { get; set; }
        public string reflectionId { get; set; }
        public string action { get; set; }
        public string type { get; set; }
        public string userName { get; set; }
        public string emailId { get; set; }
        public string messageId { get; set; }
    }

    public class MessageDetails
    {
        public string messageid { get; set; }
    }

    public class ReflctionData
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string URL { get; set; }
        public string type { get; set; }
    }

    public class QuestionTest
    {
        public string Question { get; set; }
    }

    public class TaskModuleActionHelper
    {
        public class AdaptiveCardValue<T>
        {
            [JsonProperty("msteams")]
            public object Type { get; set; } = JsonConvert.DeserializeObject("{\"type\": \"task/fetch\" }");
            [JsonProperty("data")]
            public T Data { get; set; }
        }
    }

    public class ActionDetails
    {
        public string type { get; set; }
        public string ActionType { get; set; }
    }
    public class TaskModuleActionDetails : ActionDetails
    {
        public string URL { get; set; }
        public string Title { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }

    public class Responses
    {
        public string Createdby { get; set; }
        public string QuestionTitle { get; set; }

        public List<OptionResponse> OptionResponses { get; set; }

    }
    public class OptionResponse
    {
        public int Width { get; set; }
        public string Color { get; set; }

        public string Description { get; set; }
    }

    public static class ReflectConstants
    {
        public static readonly string SaveFeedBack = "saveFeedback";
        public static readonly string RemovePosts = "removeposts";
        public static readonly string RecurringPosts = "recurringreflections";
        public static readonly string CreateReflect = "createreflect";
    }


}

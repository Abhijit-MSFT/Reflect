// <copyright file="HomeController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reflection.Helper;
using Reflection.Interfaces;
using Reflection.Model;
using Reflection.Repositories.FeedbackData;
using Reflection.Repositories.QuestionsData;
using Reflection.Repositories.RecurssionData;
using Reflection.Repositories.ReflectionData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Teams.Samples.HelloWorld.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuestionsDataRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ReflectionDataRepository _refrepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly TelemetryClient _telemetry;
        private readonly IDataBase _dbHelper;

        public HomeController(QuestionsDataRepository dataRepository, IConfiguration configuration,
            ReflectionDataRepository refrepository, IWebHostEnvironment webHostEnvironment, TelemetryClient telemetry, IDataBase dbHelper)
        {
            _repository = dataRepository;
            _configuration = configuration;
            _refrepository = refrepository;
            _webHostEnvironment = webHostEnvironment;
            _telemetry = telemetry;
            _dbHelper = dbHelper;
        }

        [Route("{userName}")]
        public ActionResult Index(string userName)
        {
            _telemetry.TrackEvent("Index");
            if (userName != null)
            {
                ViewBag.UserName = userName;
            }
            return View();
        }

        [Route("hello")]
        public ActionResult Hello()
        {
            _telemetry.TrackEvent("Hello");
            return View("Index");
        }

        [Route("manageRecurringPosts/{emailid}")]
        public ActionResult ManageRecurringPosts(string emailid)
        {
            bool showBack = Request.QueryString.Value.Contains("pathfromindex");
            _telemetry.TrackEvent("ManageRecurringPosts");
            ViewBag.emailid = emailid;
            ViewBag.showBack = showBack.ToString();
            return View();
        }


        [Route("api/DeleteReflection/{reflectionid}")]
        public async Task<string> DeleteReflection(string reflectionid)
        {
            _telemetry.TrackEvent("DeleteReflections");
            try
            {
                await _dbHelper.DeleteRecurrsionDataAsync(Guid.Parse(reflectionid));
                return "Deleted";
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        [Route("openReflections/{reflectionid}/{feedbackId}/{userName}")]
        public ActionResult OpenReflections(Guid reflectionId, int feedbackId,string userName)
        {
            _telemetry.TrackEvent("OpenReflections");
            ViewBag.reflectionId = reflectionId;
            ViewBag.feedbackId = feedbackId;
            ViewBag.userName = userName;
            return View();
        }

        [Route("openReflectionFeedback/{reflectionid}/{feedbackId}")]
        public ActionResult OpenReflectionFeedback(Guid reflectionId, int feedbackId)
        {
            _telemetry.TrackEvent("OpenReflectionFeedback");
            ViewBag.reflectionId = reflectionId;
            ViewBag.feedbackId = feedbackId;
            return View();
        }

       

        [Route("api/GetReflections/{reflectionid}")]
        public async Task<string> GetReflections(Guid reflectionid)
        {
            _telemetry.TrackEvent("GetReflections");
            try
            {
                var data = await _dbHelper.GetViewReflectionsData(reflectionid);
                var jsondata = new JObject();
                jsondata["feedback"] = JsonConvert.SerializeObject(data.FeedbackData);
                jsondata["reflection"] = JsonConvert.SerializeObject(data.ReflectionData);
                jsondata["question"] = JsonConvert.SerializeObject(data.Question);
                return jsondata.ToString();
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }


        }
        [Route("api/GetRecurssions/{email}")]
        public async Task<string> GetRecurssions(string email)
        {
            try
            {
                _telemetry.TrackEvent("GetRecurssions");
                var data = await _dbHelper.GetRecurrencePostsDataAsync(email);
                var jsondata = new JObject();
                jsondata["recurssions"] = JsonConvert.SerializeObject(data);
                return jsondata.ToString();
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }

        }

        [Route("configure")]
        public ActionResult Configure()
        {
            _telemetry.TrackEvent("Configure");
            return View();
        }

        [Route("api/GetAllDefaultQuestions/{userName}")]
        public async Task<List<QuestionsDataEntity>> GetAllDefaultQuestions(string userName)
        {
            try
            {
                _telemetry.TrackEvent("GetAllDefaultQuestions");
                var questions = await _repository.GetAllDefaultQuestionsForUser(userName);
                return questions;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }

        }
        [HttpPost]
        [Route("ReflectionAdaptiveCard")]
        public string ReflectionAdaptiveCard([FromBody]TaskInfo taskInfo)
        {
            try
            {
                _telemetry.TrackEvent("ReflectionAdaptiveCard");
                CardHelper card = new CardHelper(_configuration, _telemetry);
                var data = card.CreateNewReflect(taskInfo);
                string output = JsonConvert.SerializeObject(data);
                return output;
            }
            catch (Exception e)
            {
                _telemetry.TrackEvent("ReflectionAdaptiveCard Exception " + e);
                return null;
            }
        }

        [HttpPost]
        [Route("api/SaveRecurssionData")]
        public async Task<string> SaveRecurssionData([FromBody]RecurssionScreenData data)
        {
            try
            {
                _telemetry.TrackEvent("SaveRecurssionData");
                await _dbHelper.SaveEditRecurssionDataAsync(data);
                return "true";
            }
            catch (Exception e)
            {
                _telemetry.TrackEvent("SaveRecurssionData Exception " + e);
                return "false";
            }
        }

        [HttpPost]
        [Route("api/SaveUserFeedback")]
        public async Task<string> SaveUserFeedback([FromBody]UserfeedbackInfo data)
        {
            try
            {
                _telemetry.TrackEvent("SaveUserFeedback");
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                // Check if this is user's second feedback
                FeedbackDataEntity feebackData = await feedbackDataRepository.GetReflectionFeedback(Guid.Parse(data.reflectionId), data.emailId);
                if (data.feedbackId == 0)
                {
                    await feedbackDataRepository.DeleteFeedback(feebackData);
                }
                else {
                    if (feebackData != null && data.emailId == feebackData.FeedbackGivenBy)
                    {
                        feebackData.Feedback = data.feedbackId;
                        await feedbackDataRepository.CreateOrUpdateAsync(feebackData);
                    }
                    else
                    {
                        await _dbHelper.SaveReflectionFeedbackDataAsync(data);
                    }
                }
                return "true";
            }
            catch (Exception e)
            {
                _telemetry.TrackEvent("SaveRecurssionData Exception " + e);
                return "false";
            }
        }


        [HttpPost]
        [Route("api/GetUserFeedback")]
        public async Task<int?> GetUserFeedback([FromBody]UserfeedbackInfo data)
        {
            try
            {
                _telemetry.TrackEvent("GetUserFeedback");
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                // Check if this is user's second feedback
                FeedbackDataEntity feebackData = await feedbackDataRepository.GetReflectionFeedback(Guid.Parse(data.reflectionId), data.emailId);
               
                    if (feebackData != null && data.emailId == feebackData.FeedbackGivenBy)
                    {
                    return feebackData.Feedback;
                    }
                    else
                    {
                    return 0;
                    }
                
            }
            catch (Exception e)
            {
                _telemetry.TrackEvent("SaveRecurssionData Exception " + e);
                return null;
            }
        }
    }
}
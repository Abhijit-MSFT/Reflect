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
using Reflection.Repositories.QuestionsData;
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
            if (userName!=null)
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
            _telemetry.TrackEvent("ManageRecurringPosts");
            ViewBag.emailid = emailid;
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

        [Route("openReflections/{reflectionid}")]
        public ActionResult OpenReflections(Guid reflectionId)
        {
            _telemetry.TrackEvent("OpenReflections");
            ViewBag.reflectionId = reflectionId;
            return View();
        }

        [Route("openReflectionFeedback/{reflectionid}/{feedbackId}")]
        public ActionResult OpenReflectionFeedback(Guid reflectionId,int feedbackId)
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
                var data = card.CreateNewPostCard(taskInfo,0);
                string output = JsonConvert.SerializeObject(data);
                return output;
            }
            catch(Exception e)
            {
                _telemetry.TrackEvent("ReflectionAdaptiveCard Exception "+ e);
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

    }
}
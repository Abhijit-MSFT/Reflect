using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reflection.Helper;
using Reflection.Model;
using Reflection.Repositories.QuestionsData;
using Reflection.Repositories.RecurssionData;
using Reflection.Repositories.ReflectionData;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Teams.Samples.HelloWorld.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuestionsDataRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ReflectionDataRepository _refrepository;
        public HomeController(QuestionsDataRepository dataRepository, IConfiguration configuration, ReflectionDataRepository refrepository)
        {
            _repository = dataRepository;
            _configuration = configuration;
            _refrepository = refrepository;

        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("hello")]
        public ActionResult Hello()
        {
            return View("Index");
        }

        [Route("manageRecurringPosts")]
        public ActionResult ManageRecurringPosts()
        {


            return View();
        }

        [Route("openReflections/{reflectionid}")]
        public ActionResult OpenReflections(Guid reflectionId)
        {
            ViewBag.reflectionId = reflectionId;
            return View();
        }

        [Route("api/GetReflections/{reflectionid}")]
        public async Task<string> GetReflections(Guid reflectionid)
        {
            var data = await DBHelper.GetViewReflectionsData(reflectionid, _configuration);
            var jsondata = new JObject();
            jsondata["feedback"] = JsonConvert.SerializeObject(data.FeedbackData);
            jsondata["reflection"] = JsonConvert.SerializeObject(data.ReflectionData);
            jsondata["question"] = JsonConvert.SerializeObject(data.Question);
            return jsondata.ToString();
        }
        [Route("api/GetRecurssions")]
        public async Task<string> GetRecurssions()
        {
            var data = await DBHelper.GetRecurrencePostsDataAsync(_configuration);
            var jsondata = new JObject();
            jsondata["recurssions"] = JsonConvert.SerializeObject(data); ;
            return jsondata.ToString();
        }

        [Route("configure")]
        public ActionResult Configure()
        {
            return View();
        }

        [Route("api/GetAllDefaultQuestions")]
        public async Task<List<QuestionsDataEntity>> GetAllDefaultQuestions()
        {
            var questions = await _repository.GetAllDefaultQuestions();
            return questions;
        }

        [HttpGet("Start")]
        public IActionResult Start()
        {
            ViewBag.AzureClientId = _configuration["MicrosoftAppId"];
            return View();
        }
        [HttpGet("api/GetAccessTokenAsync")]
        public async Task<string> GetAccessTokenAsync()
        {
            var accessToken = "";

            var body = $"grant_type=client_credentials&client_id='clientid@tenantid'&client_secret=clientsecret&scope=https://graph.microsoft.com/.default";
            try
            {

                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/common/oauth2/v2.0/token");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = (new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
                HttpResponseMessage response = await httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception(responseBody);

                accessToken = JsonConvert.DeserializeObject<Object>(responseBody).ToString();
                return accessToken;
            }
            catch (Exception)
            {
                //TBD
                throw;
            }
        }

    }
}
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
        private readonly RecurssionDataRepository _recurssiondatarepository;
        public HomeController(QuestionsDataRepository dataRepository,  IConfiguration configuration,ReflectionDataRepository refrepository,RecurssionDataRepository recurssion)
        {
            _repository = dataRepository;
            _configuration = configuration;
            _refrepository = refrepository;
            _recurssiondatarepository = recurssion;
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

        [Route("openReflections")]
        public ActionResult OpenReflections(Guid reflectionid)
        {   
           
            ViewBag.reflectionId = "12e88266-c9f5-49da-ac8a-7bc5c90eb821";
            return View();
        }

        [Route("api/GetReflections/{reflectionid}")]
        public async Task<string> GetReflections(Guid reflectionid)
        {
            var data= await DBHelper.GetViewReflectionsData(reflectionid, _configuration);
            var jsondata = new JObject();
            jsondata["feedback"] = JsonConvert.SerializeObject(data.FeedbackData);
            jsondata["reflection"]= JsonConvert.SerializeObject(data.ReflectionData);
            jsondata["question"] = JsonConvert.SerializeObject(data.Question);
            return jsondata.ToString();
        }
        [Route("api/GetRecurssions")]
        public async Task<string> GetRecurssions()
        {
            var data = await _recurssiondatarepository.GetRecurrionData();
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
            var accessToken ="";

            var body = $"grant_type=client_credentials&client_id=87b25786-8ccc-45d9-8256-c1e502304291@72f988bf-86f1-41af-91ab-2d7cd011db47&client_secret=hP.S3lyLZ5zAFF0qkXlS.-65MV~_0rU8~3&scope=https://graph.microsoft.com/.default";
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
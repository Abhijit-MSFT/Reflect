using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Reflection.Model;
using Reflection.Repositories.QuestionsData;
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

        public HomeController(QuestionsDataRepository dataRepository, IConfiguration configuration)
        {
            _repository = dataRepository;
            _configuration = configuration;
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
        public ActionResult OpenReflections()
        {
            return View();
        }

        [Route("api/GetReflections")]
        public JsonResult GetReflections()
        {
            Responses responsedata = new Responses();
            responsedata.Createdby = " Cara Coleman";
            responsedata.QuestionTitle = "How are you feeling about the test today?";
            responsedata.OptionResponses = new List<OptionResponse>();
            var data = new OptionResponse();
            data.Color = "green";
            data.Width = 100;
            data.Description = "(8) Grace Taylor, Andre Lawson, Mikaela Lee, Aaron Gonzales, Elizabeth Moore, Maya Robinson, Abigail Jackson, Oscar Ward";
            responsedata.OptionResponses.Add(data);
            var data2 = new OptionResponse();
            data2.Color = "light-green";
            data2.Width = 25;
            data2.Description = "(10) Sara Perez, Ashley Schroeder, Brandon Stuart, Michael Peltier, Cora Thomas, Monica Thompson, Mateo Gomez, Michelle Harris, Hannah Jarvis, Olivia Wilson";
            responsedata.OptionResponses.Add(data2);
            var data3 = new OptionResponse();
            data3.Color = "orng";
            data3.Width = 15;
            data3.Description = "(4) Wesley Brooks, Kayla Lewis, Briana Hernandez, Gabriel Diaz";
            responsedata.OptionResponses.Add(data3);
            var data4 = new OptionResponse();
            data4.Color = "red";
            data4.Width = 10;
            data4.Description = "(3) Corey Gray, Nathan Rigby, Isabel Garcia";
            responsedata.OptionResponses.Add(data4);
            var data5 = new OptionResponse();
            data5.Color = "dark-red";
            data5.Width = 5;
            data5.Description = "(2) Sydney Mattos, Natasha Jones";
            responsedata.OptionResponses.Add(data5);
            return Json(responsedata);
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
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
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
using System.IO;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly TelemetryClient _telemetry;

        public HomeController(QuestionsDataRepository dataRepository, IConfiguration configuration,
            ReflectionDataRepository refrepository, IWebHostEnvironment webHostEnvironment, TelemetryClient telemetry)
        {
            _repository = dataRepository;
            _configuration = configuration;
            _refrepository = refrepository;
            _webHostEnvironment = webHostEnvironment;
            _telemetry = telemetry;
        }

        [Route("")]
        public ActionResult Index()
        {
            _telemetry.TrackEvent("Index");
            return View();
        }

        [Route("hello")]
        public ActionResult Hello()
        {
            _telemetry.TrackEvent("Hello");
            return View("Index");
        }

        [Route("manageRecurringPosts")]
        public ActionResult ManageRecurringPosts()
        {
            _telemetry.TrackEvent("ManageRecurringPosts");
            return View();
        }

        [Route("openReflections/{reflectionid}")]
        public ActionResult OpenReflections(Guid reflectionId)
        {
            _telemetry.TrackEvent("OpenReflections");
            ViewBag.reflectionId = reflectionId;
            return View();
        }

        [Route("api/GetReflections/{reflectionid}")]
        public async Task<string> GetReflections(Guid reflectionid)
        {
            _telemetry.TrackEvent("GetReflections");
            try
            {
                var data = await DBHelper.GetViewReflectionsData(reflectionid, _configuration);
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
        [Route("api/GetRecurssions")]
        public async Task<string> GetRecurssions()
        {
            try
            {
                _telemetry.TrackEvent("GetRecurssions");
                var data = await DBHelper.GetRecurrencePostsDataAsync(_configuration);
                var jsondata = new JObject();
                jsondata["recurssions"] = JsonConvert.SerializeObject(data); ;
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/GetAccessTokenAsync")]
        public async Task<string> GetAccessToken()
        {
            _telemetry.TrackEvent("GetAccessToken");

            try
            {
                string tenant = _configuration["Tenant"];
                string appId = _configuration["AppId"];
                string appSecret = _configuration["AppSecret"];
                string response = await POST($"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token",
                                  $"grant_type=client_credentials&client_id={appId}&client_secret={appSecret}"
                                  + "&scope=https%3A%2F%2Fgraph.microsoft.com%2F.default");

                string accessToken = JsonConvert.DeserializeObject<Reflection.Model.TokenResponse>(response).access_token;
                return accessToken;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }
        /// <summary>
        /// Http Post request for retreiving the meta data about the access token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<string> POST(string url, string body)
        {
            _telemetry.TrackEvent("POST");

            try
            {
                HttpClient httpClient = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage response = await httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception(responseBody);
                return responseBody;
            }
            catch (Exception ex)
            {

                _telemetry.TrackException(ex);
                return null;
            }
            
        }

        [HttpGet("ProfilePhoto")]
        public async Task<string> GetPhoto(string token, string userid)
        {
            _telemetry.TrackEvent("GetPhoto");
            try
            {
                string profilePhotoUrl = string.Empty;

                string endpoint = $"{_configuration["UsersEndPoint"]}{userid}/photo/$value";

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var photo = await response.Content.ReadAsStreamAsync();
                                try
                                {
                                    var fileName = userid + ".png";

                                    string imagePath = _webHostEnvironment.WebRootPath + "\\Images\\ProfilePictures\\";

                                    if (!System.IO.Directory.Exists(imagePath))

                                        System.IO.Directory.CreateDirectory(imagePath);

                                    imagePath += fileName;

                                    using (var fileStream = System.IO.File.Create(imagePath))

                                    {
                                        photo.Seek(0, SeekOrigin.Begin);

                                        photo.CopyTo(fileStream);
                                    }

                                    profilePhotoUrl = _configuration["BaseUri"] + "/images/ProfilePictures/" + fileName;


                                }
                                catch (Exception ex)
                                {
                                    _telemetry.TrackException(ex);
                                    return null;
                                }
                            }
                        }
                    }
                }
                return profilePhotoUrl;

            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;

            }
        }


        //[HttpGet("api/GetAccessTokenAsync")]
        //public async Task<string> GetAccessTokenAsync()
        //{
        //    var accessToken = "";

        //    var body = $"grant_type=client_credentials&client_id="+ _configuration["MicrosoftAppId"]+"@"+ _configuration["Tenant"] + "&client_secret="+ _configuration["MicrosoftAppPassword"] + "&scope=https://graph.microsoft.com/.default";
        //    try
        //    {

        //        HttpClient httpClient = new HttpClient();
        //        var request = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/common/oauth2/v2.0/token");
        //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        request.Content = (new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
        //        HttpResponseMessage response = await httpClient.SendAsync(request);
        //        string responseBody = await response.Content.ReadAsStringAsync();

        //        if (!response.IsSuccessStatusCode)
        //            throw new Exception(responseBody);

        //        accessToken = JsonConvert.DeserializeObject<Object>(responseBody).ToString();
        //        return accessToken;
        //    }
        //    catch (Exception ex)
        //    {
        //        //TBD
        //        throw;
        //    }
        //}

    }
}
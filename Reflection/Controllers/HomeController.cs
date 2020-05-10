using Microsoft.AspNetCore.Mvc;
using Reflection.Model;
using System.Collections.Generic;

namespace Microsoft.Teams.Samples.HelloWorld.Web.Controllers
{
    public class HomeController : Controller
    {
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
    }
}
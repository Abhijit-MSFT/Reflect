using Microsoft.AspNetCore.Mvc;


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

        [Route("configure")]
        public ActionResult Configure()
        {
            return View();
        }
    }
}
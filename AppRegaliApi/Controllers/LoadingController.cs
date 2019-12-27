using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppAppartamentiApi.Controllers
{
    public class FacebookLoadingController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Facebook Loading Page";

            return View();
        }
    }
}

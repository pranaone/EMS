using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace ElectionSystem_AD.Controllers.Location
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult Index()
        {
            return View();
        }
    }
}
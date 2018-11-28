using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    public class ProjectDetailsController : Controller
    {
        // GET: ProjectDetails
        public ActionResult ProjectReport(Guid? ProjectGUID)
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;


namespace ProjectManager.Controllers
{
    public class SelectProjectController : Controller
    {
        Repository<ProjectManager.Models.Project> p = new Repository<ProjectManager.Models.Project>();
       
        // GET: SelectProject
        public ActionResult Index()
        {
            var ProjectList = p.GetCollections();
            return View(ProjectList);
        }
        public ActionResult GetPath()
        {
            return View();
        }
        
    }
}
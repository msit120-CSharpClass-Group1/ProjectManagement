using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization.Json;

namespace ProjectManager.Controllers
{
    public class ProjectsController : Controller
    {
        ProjectsRepository projectRepo = new ProjectsRepository();
        // GET: Projects
        public ActionResult Index(int ProjectStatusID=1)
        {
            ViewBag.StatusID = ProjectStatusID;
            var datas = ProjectsRepository.ShowGroupedProject(projectRepo.GetCollections().Where(p=>p.ProjectStatusID==ProjectStatusID));
            return View(datas);
        }
        public ActionResult Projects()
        {
            return PartialView();
        }
        public ActionResult Create()
        {
            return PartialView();
        }
    }
}
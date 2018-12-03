using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    public class PerfomanceController : Controller
    {
        Repository<Project> projectRepo = new Repository<Project>();
        public ActionResult Index()
        {
            ViewBag.InProgress = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 1).ToList();
            ViewBag.Closed = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 3).ToList();

            return View();
        }
    }
}
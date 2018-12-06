using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class PerfomanceController : Controller
    {
        Repository<Project> projectRepo = new Repository<Project>();
        Repository<ProjectMembers> ProjectMembersRepo = new Repository<ProjectMembers>();
        public ActionResult Index()
        {
            ViewBag.InProgress = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 1).ToList();
            ViewBag.Closed = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 2).ToList();
            ViewBag.GroupMember = ProjectMembersRepo.GetCollections().GroupBy(p => p.Project.ProjectName).ToList();      
            return View();
        }
    }
}
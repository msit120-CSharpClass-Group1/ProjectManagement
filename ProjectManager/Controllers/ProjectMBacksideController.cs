using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    public class ProjectMBacksideController : Controller
    {
        // GET: ProjectMBackside
        Repository<ProjectMembers> ProjectMembersRepo = new Repository<ProjectMembers>();
        Repository<Project> ProjectRepo = new Repository<Project>();

        public ActionResult Index()
        {
            return View(ProjectRepo.GetCollections());
        }

        public ActionResult EditMember(Guid? ProjectGUID)
        {
            if (ProjectGUID != null)
            {
                return View(ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID));
            }
            return RedirectToAction("Index", "ProjectMBackside");
        }

        public ActionResult InsertMembers()
        {
            return View();
        }

        public ActionResult UpdateMembers()
        {
            return View();
        }

        public ActionResult DeleteMembers(ProjectMembers _projectmember)
        {
            Guid ProjectGUID = _projectmember.ProjectGUID;
            Guid EmployeeGUID = _projectmember.EmployeeGUID;
            ProjectMembersRepo.Delete(ProjectMembersRepo.Find(EmployeeGUID, ProjectGUID));
            return RedirectToAction("EditMember", ProjectGUID);
        }
    }
}
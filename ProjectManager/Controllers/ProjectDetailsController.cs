using Newtonsoft.Json;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    public class ProjectDetailsController : Controller
    {
        Repository<Project> projectRepo = new Repository<Project>();
        // GET: ProjectDetails

        Repository<Employee> employee = new Repository<Employee>();
        Repository<Department> dep = new Repository<Department>();
        Repository<ProjectMembers> projectMembers = new Repository<ProjectMembers>();

        public ActionResult SaveSession(Guid ProjectGUID)
        {
            Response.Cookies["ProjectGUID"].Value = ProjectGUID.ToString();
            Response.Cookies["ProjectGUID"].Expires = DateTime.Now.AddDays(7);
            return RedirectToAction("Index", "ProjectDetails");
        }

        public ActionResult Index(/*Guid ProjectGUID*/)
        {
            if (Request.Cookies["ProjectGUID"].Value != null)
            {
                Guid indexPJID = new Guid(Request.Cookies["ProjectGUID"].Value.ToString());
                ViewBag.FirstEmpList = employee.GetCollections().ToList();
                ViewBag.ThisProjectMember = projectMembers.GetCollections().Where(p => p.ProjectGUID == indexPJID).ToList();
                return View(dep.GetCollections());
            }
            return RedirectToAction("ProjectReport", "ProjectDetails");
        }

        public ActionResult SelectDep()
        {
            var depGUID = new Guid(Request.QueryString["depid"]);
            var emp = employee.GetCollections().Where(e => e.Department.DepartmentGUID == depGUID);
            return Content(JsonConvert.SerializeObject(emp), "application/json");
        }

        public ActionResult AddProjectMember(Guid memberID)
        {

                ProjectMembers pm = new ProjectMembers();
                pm.ProjectGUID = new Guid(Request.Cookies["ProjectGUID"].Value.ToString());
                pm.EmployeeGUID = memberID;
                projectMembers.Add(pm);
                return Content("html...");          
            //return RedirectToAction("ProjectReport", "ProjectDetails");
        }

        public ActionResult DeleteProjectMember()
        {

            Guid memberID = new Guid(Request.QueryString["memberID"]);
            Guid InvitePJGUID = new Guid(Request.Cookies["ProjectGUID"].Value.ToString());
            projectMembers.Delete(projectMembers.Find(memberID, InvitePJGUID));
            return Content("html...");
            //return RedirectToAction("ProjectReport", "ProjectDetails");
        }

        public ActionResult ReloadTeamCount()
        {
            Guid InvitePJGUID = new Guid(Request.Cookies["ProjectGUID"].Value.ToString());
            var pjmb = projectMembers.GetCollections().Where(p => p.ProjectGUID == InvitePJGUID);
            return Content(JsonConvert.SerializeObject(pjmb), "application/json");
        }
        public ActionResult ProjectReport(Guid? ProjectGUID)
        {
            if (ProjectGUID != null)
            {
                Session["ProjectGUID"] = ProjectGUID;
            }
            return View();
        }
        [HttpGet]
        public ActionResult ProjectEdit()
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");

            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections(), "DepartmentGUID", "DepartmentName");
            ViewBag.Employees = new SelectList(new Repository<Employee>().GetCollections(), "EmployeeGUID", "EmployeeName");
            ViewBag.ProjectStatuses = new SelectList(new Repository<ProjectStatus>().GetCollections(), "ProjectStatusID", "ProjectStatusName");
            ViewBag.ProjectCategories = new SelectList(new Repository<ProjectCategory>().GetCollections(), "ProjectCategoryID", "ProjectCategoryName");
            Guid _projectGUID = new Guid(Session["ProjectGUID"].ToString());            
            return View(projectRepo.Find(_projectGUID));
        }
        [HttpPost]
        public ActionResult ProjectEdit(Project _project)
        {
            var recentProject = projectRepo.Find(_project.ProjectGUID);
            recentProject.ProjectName = _project.ProjectName;
            recentProject.ProjectStatusID = _project.ProjectStatusID;
            recentProject.ProjectCategoryID = _project.ProjectCategoryID;
            recentProject.ProjectSupervisorGUID = _project.ProjectSupervisorGUID;
            recentProject.EstStartDate = _project.EstStartDate;
            recentProject.EstEndDate = _project.EstEndDate;
            recentProject.StartDate = _project.StartDate;
            recentProject.Description = _project.Description;
            recentProject.IsGeneralManagerConcerned = _project.IsGeneralManagerConcerned;

            projectRepo.Update(recentProject);
            return ProjectEdit();
        }

        public ActionResult AssignTaskUI()
        {
            //ViewBag.LoadTask = tasks.GetCollections().Where(t => t.ProjectGUID == SendprojectGUID && t.TaskStatusID == 1).ToList();
            //return View(projectMembers.GetCollections().Where(p => p.ProjectGUID == SendprojectGUID));
            return View();
        }


    }
}
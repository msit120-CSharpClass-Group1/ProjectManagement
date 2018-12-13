using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員,專案經理,處長")]
    public class ProjectsController : Controller
    {
        Repository<Project> projectRepo = new Repository<Project>();
        Repository<Employee> employeeRepo = new Repository<Employee>();
        Repository<Tasks> taskRepo = new Repository<Tasks>();
        // GET: Projects
        public ActionResult Index(int ProjectStatusID=1)
        {
            ViewBag.StatusID = ProjectStatusID;
            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections(), "DepartmentGUID", "DepartmentName");
            ViewBag.Employees = new SelectList(new Repository<Employee>().GetCollections(), "EmployeeGUID", "EmployeeName");
            ViewBag.ProjectStatuses = new SelectList(new Repository<ProjectStatus>().GetCollections(), "ProjectStatusID", "ProjectStatusName");
            ViewBag.ProjectCategories = new SelectList(new Repository<ProjectCategory>().GetCollections(), "ProjectCategoryID", "ProjectCategoryName");
            var projects = projectRepo.GetCollections().OrderBy(p => p.ProjectID).Where(p => p.ProjectStatusID == ProjectStatusID);
            projects.LoadProjectsCompletedRate(taskRepo.GetCollections());  

            var datas = projects.GetGroupedProject();
            return View(datas);
        }
        public ActionResult Projects()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult CreateProject(Project project)
        {
            int lastProjectID = projectRepo.GetCollections().GetLastProjectIntID();
            project.ProjectID = "P" + DateTime.Now.Year.ToString().Substring(2, 2) + (lastProjectID + 1).ToString().Substring(2, 3);
            project.ProjectGUID = Guid.NewGuid();
            project.StartDate = project.EstStartDate;
            project.EndDate = project.EstEndDate;

            project.InChargeDeptGUID = new Guid("87be8dc8-3f70-43ce-847e-46e2d0c1d7a4");
            project.InChargeDeptPMGUID = new Guid("de4fbb32-f667-462f-9e83-fc92a6476c02"); 

            projectRepo.Add(project);
            return RedirectToAction("Index","Projects");
        }
        public ActionResult GetEmployeesByDept(Guid? DeptGUID)
        {
            var emps = employeeRepo.GetCollections()
                .Where(e => e.Department.DepartmentGUID == DeptGUID)
                .Select(e => new { e.EmployeeGUID, e.EmployeeName });
            return Content(JsonConvert.SerializeObject(emps), "application/json");
        }

    }
}
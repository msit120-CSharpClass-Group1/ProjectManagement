using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using static ProjectManager.Models.PermissionBL;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員,專案經理,處長")]
    public class ProjectsController : Controller
    {
        // GET: Projects
        Repository<Project> projectRepo = new Repository<Project>();
        Repository<Employee> employeeRepo = new Repository<Employee>();
        Repository<Tasks> taskRepo = new Repository<Tasks>();
        Repository<Members> memberRepo = new Repository<Members>();
        Repository<JobTitle> jobTitleRepo = new Repository<JobTitle>();

        public ActionResult Index(int ProjectStatusID=1)
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var titleID = jobTitleRepo.Find(new Guid(Request.Cookies["TitleGUID"].Value)).TitleID;

            ViewBag.StatusID = ProjectStatusID;
            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections().OrderBy(d=>d.DepartmentID), "DepartmentGUID", "DepartmentName");
            ViewBag.Employees = new SelectList(new Repository<Employee>().GetCollections(), "EmployeeGUID", "EmployeeName");
            ViewBag.ProjectStatuses = new SelectList(new Repository<ProjectStatus>().GetCollections(), "ProjectStatusID", "ProjectStatusName");
            ViewBag.ProjectCategories = new SelectList(new Repository<ProjectCategory>().GetCollections(), "ProjectCategoryID", "ProjectCategoryName");
            var projects = projectRepo.GetCollections().OrderBy(p => p.ProjectID).Where(p => p.ProjectStatusID == ProjectStatusID);
            switch ((Emp_Title)Enum.Parse(typeof(Emp_Title), titleID.ToString()))
            {
                case Emp_Title.Admin:
                    break;
                case Emp_Title.Minister:
                    break;
                case Emp_Title.PM:
                    projects = projects.Where(p => p.InChargeDeptPMGUID == member.EmployeeGUID);
                    break;
                case Emp_Title.Director:
                    break;
                default:
                    break;
            }

            projects.LoadProjectsCompletedRate(taskRepo.GetCollections());
            projects.LoadProjectSaveTimeRate(taskRepo.GetCollections());

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
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var titleID = jobTitleRepo.Find(new Guid(Request.Cookies["TitleGUID"].Value)).TitleID;
            int lastProjectID = projectRepo.GetCollections().GetLastProjectIntID();
            project.ProjectID = "P" + DateTime.Now.Year.ToString().Substring(2, 2) + (lastProjectID + 1).ToString().Substring(2, 3);
            project.ProjectGUID = Guid.NewGuid();
            project.StartDate = project.EstStartDate;
            project.EndDate = project.EstEndDate;

            project.InChargeDeptGUID = (Guid)member.Employee.DepartmentGUID;    //new Guid("87be8dc8-3f70-43ce-847e-46e2d0c1d7a4");
            project.InChargeDeptPMGUID = (Guid)member.EmployeeGUID;             //new Guid("de4fbb32-f667-462f-9e83-fc92a6476c02"); 

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
        [HttpGet]
        public ActionResult InsertProjectForDemo()
        {
            StoredProcedureForDemo storedProcedure = new StoredProcedureForDemo("InsertProjectForDemo");            
            storedProcedure.Execute();
            return Content("success", "application/json");
        }

    }
}
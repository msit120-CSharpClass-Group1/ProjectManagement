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
            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections(), "DepartmentGUID", "DepartmentName");
            ViewBag.Employees = new SelectList(new Repository<Employee>().GetCollections(), "EmployeeGUID", "EmployeeName");
            ViewBag.ProjectStatuses = new SelectList(new Repository<ProjectStatus>().GetCollections(), "ProjectStatusID", "ProjectStatusName");
            ViewBag.ProjectCategories = new SelectList(new Repository<ProjectCategory>().GetCollections(), "ProjectCategoryID", "ProjectCategoryName");
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
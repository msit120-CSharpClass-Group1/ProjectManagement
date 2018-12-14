using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    public class ProjectsBacksideController : Controller
    {
        // GET: ProjectsBackside
        Repository<Project> projectRepo = new Repository<Project>();

        public ActionResult Index()
        {
            var data = projectRepo.GetCollections().OrderBy(p => p.ProjectID).ToList();
            return View(data);
        }        
        public ActionResult Insert(Project _project)
        {
            if(Request.Form.Count > 0)
            {
                _project.ProjectGUID = Guid.NewGuid();
                _project.ProjectID = "P" + (projectRepo.GetCollections().GetLastProjectIntID() +1);

                projectRepo.Add(_project);
                return RedirectToAction("Index");
            }
            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections(), "DepartmentGUID", "DepartmentName");
            ViewBag.Employees = new SelectList(new Repository<Employee>().GetCollections(), "EmployeeGUID", "EmployeeName");
            ViewBag.ProjectStatuses = new SelectList(new Repository<ProjectStatus>().GetCollections(), "ProjectStatusID", "ProjectStatusName");
            ViewBag.ProjectCategories = new SelectList(new Repository<ProjectCategory>().GetCollections(), "ProjectCategoryID", "ProjectCategoryName");

            return View();            
        }
        [HttpGet]
        public ActionResult Edit(Guid? projectGUID)
        {           
            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections(), "DepartmentGUID", "DepartmentName");
            ViewBag.Employees = new SelectList(new Repository<Employee>().GetCollections(), "EmployeeGUID", "EmployeeName");
            ViewBag.ProjectStatuses = new SelectList(new Repository<ProjectStatus>().GetCollections(), "ProjectStatusID", "ProjectStatusName");
            ViewBag.ProjectCategories = new SelectList(new Repository<ProjectCategory>().GetCollections(), "ProjectCategoryID", "ProjectCategoryName");

            Project project = projectRepo.Find(projectGUID);
            return View(project);
        }
        [HttpPost]
        public ActionResult Edit(Project _project)
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
            recentProject.ProjectBudget = _project.ProjectBudget;

            projectRepo.Update(recentProject);
            return RedirectToAction("Index");
        }
        public ActionResult Delete(Guid? projectGUID)
        {
            Project project = projectRepo.Find(projectGUID);
            projectRepo.Delete(project);
            return RedirectToAction("Index");
        }

    }
}
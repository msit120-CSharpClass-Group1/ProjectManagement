using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員")]
    public class TasksController : Controller
    {
        private Repository<Tasks> tasksRepo = new Repository<Tasks>();
        Repository<Project> projectRepo = new Repository<Project>();
        Repository<ProjectMembers> ProjectMembersRepo = new Repository<ProjectMembers>();

        public ActionResult Index(Guid? projectGUID)
        {
            if (projectGUID == null)
                return RedirectToAction("Index", "ProjectsBackside");
            var data = tasksRepo.GetCollections()
                .Where(t => t.ProjectGUID == projectGUID)
                .OrderBy(t => t.TaskID)
                .GetSortedTasks();
            ViewBag.Project = projectRepo.Find(projectGUID); 
            return View(data);
        }
        public ActionResult TreeGridPartialView_backSide(Guid? projectGUID)
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid _projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            var tasks = tasksRepo.GetCollections().OrderBy(t => t.TaskID)
                .Where(t => t.ProjectGUID == projectGUID).GetSortedTasks();

            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections(), "DepartmentGUID", "DepartmentName");
            ViewBag.Employees = new SelectList(new Repository<Employee>().GetCollections(), "EmployeeGUID", "EmployeeName");
            ViewBag.ProjectStatuses = new SelectList(new Repository<ProjectStatus>().GetCollections(), "ProjectStatusID", "ProjectStatusName");
            ViewBag.ProjectCategories = new SelectList(new Repository<ProjectCategory>().GetCollections(), "ProjectCategoryID", "ProjectCategoryName");
            return PartialView(tasks);
        }
        public ActionResult AsideRightPartialView(Guid? taskGUID)
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid _projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            if (taskGUID == null)
                return RedirectToAction("Index", "ProjectsBackside");

            Tasks task = tasksRepo.Find(taskGUID);
            var leafTasks = tasksRepo.GetCollections().Where(t => t.ProjectGUID == task.ProjectGUID).GetLeafTasks();
            var parentTasks = task.Project.Tasks.Except(leafTasks);
            var projectMembers = ProjectMembersRepo.GetCollections()
                .Where(m => m.ProjectGUID == _projectGUID)
                .Select(m => new { m.EmployeeGUID, m.Employee.EmployeeName }).ToList();

            ViewBag.ParentTasks = new SelectList(parentTasks, "TaskGUID", "TaskName");
            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections(), "DepartmentGUID", "DepartmentName");
            ViewBag.Employees = new SelectList(new Repository<Employee>().GetCollections(), "EmployeeGUID", "EmployeeName");
            ViewBag.TaskStatuses = new SelectList(new Repository<TaskStatus>().GetCollections(), "TaskStatusID", "TaskStatusName");
            ViewBag.ProjectCategories = new SelectList(new Repository<ProjectCategory>().GetCollections(), "ProjectCategoryID", "ProjectCategoryName");            
            ViewBag.ProjectMembers = new SelectList(projectMembers, "EmployeeGUID","EmployeeName");

            return PartialView(task);
        }
        
        [HttpPost]
        public ActionResult Edit(Tasks task)
        {
            Tasks recentTask = tasksRepo.Find(task.TaskGUID);
            recentTask.TaskName = task.TaskName;
            recentTask.ParentTaskGUID = task.ParentTaskGUID;
            recentTask.EmployeeGUID = task.EmployeeGUID;
            recentTask.TaskStatusID = task.TaskStatusID;
            recentTask.EstStartDate = task.EstStartDate;
            recentTask.EstEndDate = task.EstEndDate;
            recentTask.StartDate = task.StartDate;
            recentTask.EndDate = task.EndDate;
            recentTask.IsRead = task.IsRead;
            recentTask.EstWorkTime = recentTask.GetEstWorkTime(System.Web.HttpContext.Current.Application["Holidays"] as HolidaysVM);
            recentTask.WorkTime = recentTask.GetWorkTime(System.Web.HttpContext.Current.Application["Holidays"] as HolidaysVM);

            tasksRepo.Update(recentTask);
            return Json("succes", JsonRequestBehavior.AllowGet);
        }
    }
}
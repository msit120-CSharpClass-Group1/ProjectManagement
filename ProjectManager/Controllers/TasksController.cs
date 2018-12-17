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
        private Repository<Tasks> taskRepo = new Repository<Tasks>();
        private Repository<Project> projectRepo = new Repository<Project>();
        private Repository<ProjectMembers> projectMemberRepo = new Repository<ProjectMembers>();
        private Repository<Document> documentRepo = new Repository<Document>();
        private Repository<TaskDetail> taskDetailRepo = new Repository<TaskDetail>();
        private Repository<TaskResource> resourceRepo = new Repository<TaskResource>();

        public ActionResult Index(Guid? projectGUID)
        {
            if (projectGUID == null)
                return RedirectToAction("Index", "ProjectsBackside");
            var data = taskRepo.GetCollections()
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
            var tasks = taskRepo.GetCollections().OrderBy(t => t.TaskID)
                .Where(t => t.ProjectGUID == projectGUID).GetSortedTasks();

            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections().OrderBy(d => d.DepartmentID), "DepartmentGUID", "DepartmentName");
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

            Tasks task = taskRepo.Find(taskGUID);
            var leafTasks = taskRepo.GetCollections().Where(t => t.ProjectGUID == task.ProjectGUID).GetLeafTasks();
            var parentTasks = task.Project.Tasks.Except(leafTasks);
            var projectMembers = projectMemberRepo.GetCollections()
                .Where(m => m.ProjectGUID == _projectGUID)
                .Select(m => new { m.EmployeeGUID, m.Employee.EmployeeName }).ToList();

            ViewBag.ParentTasks = new SelectList(parentTasks, "TaskGUID", "TaskName");
            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections().OrderBy(d => d.DepartmentID), "DepartmentGUID", "DepartmentName");
            ViewBag.Employees = new SelectList(new Repository<Employee>().GetCollections(), "EmployeeGUID", "EmployeeName");
            ViewBag.TaskStatuses = new SelectList(new Repository<TaskStatus>().GetCollections(), "TaskStatusID", "TaskStatusName");
            ViewBag.ProjectCategories = new SelectList(new Repository<ProjectCategory>().GetCollections(), "ProjectCategoryID", "ProjectCategoryName");            
            ViewBag.ProjectMembers = new SelectList(projectMembers, "EmployeeGUID","EmployeeName");

            return PartialView(task);
        }
        
        [HttpPost]
        public ActionResult Edit(Tasks task)
        {
            Tasks recentTask = taskRepo.Find(task.TaskGUID);
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

            taskRepo.Update(recentTask);
            return Json("succes", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Delete(Guid? taskGUID)
        {            
            Tasks recentTask = taskRepo.Find(taskGUID);
            string errorMsg = "";
            var allTasks = recentTask.GetAllChildTasks().ToList();
            allTasks.Insert(0, recentTask);

            try
            {                
                foreach (var child in allTasks.AsQueryable().Reverse())
                {
                    foreach (var doc in child.Document.ToList())
                    {
                        documentRepo.Delete(documentRepo.Find(doc.DocumentGUID));
                    }
                    foreach (var detail in child.TaskDetail.ToList())
                    {
                        taskDetailRepo.Delete(taskDetailRepo.Find(detail.TaskDetailGUID));
                    }
                    foreach (var resource in child.TaskResource.ToList())
                    {
                        resourceRepo.Delete(resourceRepo.Find(resource.ResourceGUID));
                    }
                    taskRepo.Delete(taskRepo.Find(child.TaskGUID));
                }
                

            }
            catch
            {
                errorMsg = "刪除失敗";
            }

            return Json(errorMsg, JsonRequestBehavior.AllowGet);
        }
    }
}
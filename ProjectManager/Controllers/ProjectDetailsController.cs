using Newtonsoft.Json;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員,專案經理,處長")]
    public class ProjectDetailsController : Controller
    {
        // GET: ProjectDetails
        Repository<Tasks> taskRepo = new Repository<Tasks>();
        Repository<Project> projectRepo = new Repository<Project>();   
        Repository<ProjectMembers> projectMembersRepo = new Repository<ProjectMembers>();
        Repository<TaskStatus> StatusRepo = new Repository<TaskStatus>();


        #region Project Report Chart
        public ActionResult ProjectReport(Guid? ProjectGUID)
        {
            if (ProjectGUID != null)
            {
                Response.Cookies["ProjectGUID"].Value = ProjectGUID.ToString();
                Response.Cookies["ProjectGUID"].Expires = DateTime.Now.AddDays(7);
            }

            return View();
        }
        public ActionResult RootTasksCompletedRate()
        {
            Guid _projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            var _tasks = taskRepo.GetCollections().Where(t => t.ProjectGUID == _projectGUID).OrderBy(t => t.TaskID);
            var rootTasks = _tasks.GetRootTasks();            
            ChartData<SingleColorChartDataset<int>> _data = new ChartData<SingleColorChartDataset<int>>();
            _data.labels.AddRange(rootTasks.Select(t => t.TaskName));
            _data.datasets.Add(new SingleColorChartDataset<int>()
            {
                label = "項目完成度",
                backgroundColor = "#007BFF",
                borderColor = "#007BFF",
                data = rootTasks.GetRootTasksCompletedRate(_tasks).ToList()
            });
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RootTasksEstWorkTimeSum()
        {
            Guid _projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            List<string> colors = new List<string>() { "#007BFF", "#4B0082", "#ADD8E6", "#B0C4DE", "#7744FF", "#CCEEFF" };
            var _tasks = taskRepo.GetCollections().Where(t => t.ProjectGUID == _projectGUID).OrderBy(t => t.TaskID);
            var rootTasks = _tasks.GetRootTasks();

            ChartData<MultiColorChartDataset<int>> _data = new ChartData<MultiColorChartDataset<int>>();
            _data.labels.AddRange(rootTasks.Select(t => t.TaskName));
            _data.datasets.Add(new MultiColorChartDataset<int>()
            {
                label = "dataset",
                backgroundColor = colors,
                borderColor = colors,
                data = rootTasks.GetRootTasksWorkTimeSum(_tasks).ToList()
            });
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProjectMembersEstWorkTimeSum()
        {
            Guid _projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            var members = projectMembersRepo.GetCollections().Where(m => m.ProjectGUID == _projectGUID).Distinct();
            ChartData<SingleColorChartDataset<int>> _data = new ChartData<SingleColorChartDataset<int>>();
            _data.labels.AddRange(members.Select(m => m.Employee.EmployeeName));
            _data.datasets.Add(new SingleColorChartDataset<int>()
            {
                label = "工時總和",
                backgroundColor = "#007BFF",
                borderColor = "#007BFF",
                data = members.GetWorkTimeSumOfProjectMembers(taskRepo.GetCollections().Where(t => t.ProjectGUID == _projectGUID)),
                fill = false,
            });
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TasksByStatus()
        {
            Guid _projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);

            ChartData<SingleColorChartDataset<int>> chartData = new ChartData<SingleColorChartDataset<int>>();

            chartData.labels = StatusRepo.GetCollections().Select(s => s.TaskStatusName).ToList();

            chartData.datasets.Add(new SingleColorChartDataset<int>
            {
                label = "Count",
                backgroundColor = "rgba(91, 155, 213, 0.5)",
                borderColor = "rgba(91, 155, 213, 1)",
                data = StatusRepo.GetCollections().CountTasksByStatus(_projectGUID),
            });

            return Content(JsonConvert.SerializeObject(chartData), "application/json");
        }

        #endregion

        #region Project Edit
        [HttpGet]
        public ActionResult ProjectEdit()
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");

            ViewBag.Departments = new SelectList(new Repository<Department>().GetCollections(), "DepartmentGUID", "DepartmentName");
            ViewBag.Employees = new SelectList(new Repository<Employee>().GetCollections(), "EmployeeGUID", "EmployeeName");
            ViewBag.ProjectStatuses = new SelectList(new Repository<ProjectStatus>().GetCollections(), "ProjectStatusID", "ProjectStatusName");
            ViewBag.ProjectCategories = new SelectList(new Repository<ProjectCategory>().GetCollections(), "ProjectCategoryID", "ProjectCategoryName");
            Guid _projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
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
        #endregion

        #region Project Distribution
        public ActionResult ProjectDistribution()
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid _projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            var tasks = taskRepo.GetCollections().OrderBy(t => t.TaskID)
                .Where(t => t.ProjectGUID == _projectGUID).GetSortedTasks();
            ViewBag.Project = projectRepo.Find(_projectGUID);
            ViewBag.TaskStatuses = new SelectList(new Repository<TaskStatus>().GetCollections(), "TaskStatusID", "TaskStatusName");

            return View(tasks);
        }
        public ActionResult TreeGridPartialView()
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid _projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            var tasks = taskRepo.GetCollections().OrderBy(t => t.TaskID)
                .Where(t => t.ProjectGUID == _projectGUID).GetSortedTasks();
            return PartialView(tasks);
        }        
        public ActionResult AsideRightReadOnlyMode(Guid? taskGUID)
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            var task = taskRepo.Find(taskGUID);
            return PartialView(task);
        }
        public ActionResult AsideRightEditMode(Guid? taskGUID)
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            var task = taskRepo.Find(taskGUID);
            task.AutoWorkTime = task.GetAutoEstWorkTime(System.Web.HttpContext.Current.Application["Holidays"] as HolidaysVM);
            return PartialView(task);
        }
        public ActionResult AsideRightInsertMode(Guid? parentTaskGUID)
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            var task = new Tasks();
            task.ParentTaskGUID = parentTaskGUID;
            task.EstStartDate = DateTime.Now;
            task.EstEndDate = DateTime.Now;            
            task.StartDate = DateTime.Now;
            task.AutoWorkTime = task.GetAutoEstWorkTime(System.Web.HttpContext.Current.Application["Holidays"] as HolidaysVM);
            task.EstWorkTime = task.AutoWorkTime;
            return PartialView(task);
        }
        [HttpPost]
        public ActionResult InsertTask(Tasks task)
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid _projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            task.ProjectGUID = _projectGUID;
            task.TaskGUID = Guid.NewGuid();
            task.StartDate = task.EstStartDate;
            task.EndDate = task.EstEndDate;
            task.TaskStatusID = (int)ProjectManager.Models.TasksBL.Task_Status.Discussing;
            task.AssignedDate = DateTime.Now;
            task.IsRead = false;
            taskRepo.Add(task);
            return Json("success", JsonRequestBehavior.AllowGet);            
        }
        [HttpGet]
        public ActionResult EditTask(Guid? TaskGUID)
        {
            var task = taskRepo.Find(TaskGUID);
            task.AutoWorkTime = task.GetAutoEstWorkTime(System.Web.HttpContext.Current.Application["Holidays"] as HolidaysVM);
            return Content(JsonConvert.SerializeObject(task), "application/json");
        }
        [HttpPost]
        public ActionResult EditTask(Tasks taskModified)
        {
            Tasks recentTask = taskRepo.Find(taskModified.TaskGUID);
            recentTask.TaskName = taskModified.TaskName;            
            recentTask.EstStartDate = taskModified.EstStartDate;
            recentTask.EstEndDate = taskModified.EstEndDate;
            recentTask.EstWorkTime = taskModified.EstWorkTime;
            recentTask.StartDate = taskModified.EstStartDate;
            recentTask.Tag = taskModified.Tag;
            recentTask.Description = taskModified.Description;

            taskRepo.Update(recentTask);
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteTasks(Tasks _task)
        {
            Tasks recentTask = taskRepo.Find(_task.TaskGUID);
            string errorMsg = "";
            var allTasks = recentTask.GetAllChildTasks().ToList();
            allTasks.Insert(0, recentTask);

            if (allTasks.IsAnyResource())
            {
                errorMsg = "欲刪除的工作項目，有費用產生，不可刪除。";
            }
            else
            {
                try
                {
                    foreach (var child in allTasks.AsQueryable().Reverse())
                    {
                        taskRepo.Delete(taskRepo.Find(child.TaskGUID));
                    }
                }
                catch
                {

                }
            }
            return Json(errorMsg, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetChildTaskCount(Guid? taskGUID)
        {
            Tasks recentTask = taskRepo.Find(taskGUID);
            var childTasks = recentTask.GetAllChildTasks();
            return Json(childTasks.Count(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult LoadHolidays(HolidaysVM holidays)
        {
            Response.Cookies["Holidays"].Value = "loaded";            
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application["Holidays"] = holidays;
            System.Web.HttpContext.Current.Application.UnLock();
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAutoWorkTime(Tasks task)
        {
            var autoWorkTime = task.GetAutoEstWorkTime(System.Web.HttpContext.Current.Application["Holidays"] as HolidaysVM);
            return Json(autoWorkTime, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult TaskAcceptance(bool isConfirmed, Guid? taskGuid, int? reviewScore, string reviewDescription)
        {
            Tasks _task = taskRepo.GetCollections().Where(t => t.TaskGUID == taskGuid).FirstOrDefault();
            if (reviewScore != null && isConfirmed)
            {
                if (reviewScore > 100)
                    reviewScore = 100;
                _task.ReviewScore = byte.Parse(reviewScore.ToString());
                _task.ReviewDescription = reviewDescription;
            }
           
            _task.TaskStatusID = isConfirmed ? (int)TasksBL.Task_Status.Completed : (int)TasksBL.Task_Status.InProgress;
            taskRepo.Update(_task);
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult TaskRevivedToInProgress(Guid? taskGuid)
        {
            Tasks _task = taskRepo.GetCollections().Where(t => t.TaskGUID == taskGuid).FirstOrDefault();
            _task.TaskStatusID = (int)TasksBL.Task_Status.InProgress;
            taskRepo.Update(_task);
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EndTask(Guid? taskGuid)
        {
            Tasks _task = taskRepo.GetCollections().Where(t => t.TaskGUID == taskGuid).FirstOrDefault();
            _task.TaskStatusID = (int)TasksBL.Task_Status.Closed;
            taskRepo.Update(_task);
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult InsertTasksForDemo()
        {
            StoredProcedureForDemo storedProcedure = new StoredProcedureForDemo("InsertTasksForDemo");
            storedProcedure.Execute();            
            return Content("success", "application/json");
        }
        #endregion
    }
}
using Newtonsoft.Json;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class ProjectDetailsController : Controller
    {
        Repository<Tasks> taskRepo = new Repository<Tasks>();
        Repository<Project> projectRepo = new Repository<Project>();
        // GET: ProjectDetails

        Repository<Employee> employee = new Repository<Employee>();
        Repository<Department> dep = new Repository<Department>();
        Repository<ProjectMembers> projectMembers = new Repository<ProjectMembers>();
        Repository<Tasks> tasks = new Repository<Tasks>();

        public ActionResult ProjectReport(Guid? ProjectGUID)
        {
            if (ProjectGUID != null)
            {
                Session["ProjectGUID"] = ProjectGUID;
                Response.Cookies["ProjectGUID"].Value = ProjectGUID.ToString();
                Response.Cookies["ProjectGUID"].Expires = DateTime.Now.AddDays(7);
            }

            //if (Session["ProjectGUID"] == null)
            //    return RedirectToAction("Index", "Projects");
            return View();
        }
        public ActionResult RootTasksCompletedRate()
        {
            Guid _projectGUID = new Guid(Session["ProjectGUID"].ToString());
            var _tasks = taskRepo.GetCollections().Where(t => t.ProjectGUID == _projectGUID).OrderBy(t => t.TaskID);
            var rootTasks = _tasks.GetRootTasks();

            ChartData<SingleColorChartDataset> _data = new ChartData<SingleColorChartDataset>();
            _data.labels.AddRange(rootTasks.Select(t => t.TaskName));
            _data.datasets.Add(new SingleColorChartDataset() {
                label="項目完成度",
                backgroundColor= "#007BFF",
                borderColor= "#007BFF",
                data = rootTasks.GetRootTasksCompletedRate(_tasks).ToList()
            });
            return Json(_data,JsonRequestBehavior.AllowGet);
        }
        public ActionResult RootTasksEstWorkTimeSum()
        {
            Guid _projectGUID = new Guid(Session["ProjectGUID"].ToString());
            List<string> colors = new List<string>() { "#007BFF", "#4B0082", "#ADD8E6", "#B0C4DE", "#7744FF", "#CCEEFF" };
            var _tasks = taskRepo.GetCollections().Where(t => t.ProjectGUID == _projectGUID).OrderBy(t => t.TaskID);
            var rootTasks = _tasks.GetRootTasks();

            ChartData<MultiColorChartDataset> _data = new ChartData<MultiColorChartDataset>();
            _data.labels.AddRange(rootTasks.Select(t => t.TaskName));
            _data.datasets.Add(new MultiColorChartDataset()
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
            Guid _projectGUID = new Guid(Session["ProjectGUID"].ToString());
            var members = projectMembers.GetCollections().Where(m => m.ProjectGUID == _projectGUID).Distinct();
            ChartData<SingleColorChartDataset> _data = new ChartData<SingleColorChartDataset>();
            _data.labels.AddRange(members.Select(m => m.Employee.EmployeeName));
            _data.datasets.Add(new SingleColorChartDataset() {
                label = "工時總和",
                backgroundColor = "#007BFF",
                borderColor = "#007BFF",
                data = members.GetWorkTimeSumOfProjectMembers(taskRepo.GetCollections().Where(t => t.ProjectGUID == _projectGUID)),
                fill = false,
            });
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RootTasksResourceSum()
        {
            Guid _projectGUID = new Guid(Session["ProjectGUID"].ToString());
            List<string> colors = new List<string>() { "#007BFF", "#4B0082", "#ADD8E6", "#B0C4DE", "#7744FF", "#CCEEFF" };
            var _tasks = taskRepo.GetCollections().Where(t => t.ProjectGUID == _projectGUID).OrderBy(t => t.TaskID);
            var rootTasks = _tasks.GetRootTasks();
            ChartData<MultiColorChartDataset> _data = new ChartData<MultiColorChartDataset>();
            _data.labels.AddRange(rootTasks.Select(t => t.TaskName));
            _data.datasets.Add(new MultiColorChartDataset()
            {
                label = "dataset",
                backgroundColor = colors,
                borderColor = colors,
                data = rootTasks.GetRootTasksResourceSum(_tasks).ToList()
            });
            return Json(_data, JsonRequestBehavior.AllowGet);
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

        public ActionResult ProjectDistribution()
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid _projectGUID = new Guid(Session["ProjectGUID"].ToString());
            var tasks = taskRepo.GetCollections().OrderBy(t => t.TaskID)
                .Where(t => t.ProjectGUID == _projectGUID).GetSortedTasks();
            ViewBag.Projects = projectRepo.GetCollections().Where(p => p.ProjectGUID == _projectGUID).ToList();
            ViewBag.TaskStatuses = new SelectList(new Repository<TaskStatus>().GetCollections(), "TaskStatusID", "TaskStatusName");

            return View(tasks);
        }
        public ActionResult TreeGridPartialView()
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid _projectGUID = new Guid(Session["ProjectGUID"].ToString());
            var tasks = taskRepo.GetCollections().OrderBy(t => t.TaskID)
                .Where(t => t.ProjectGUID == _projectGUID).GetSortedTasks();
            return PartialView(tasks);
        }

        [HttpPost]
        public ActionResult InsertTask(Tasks task)
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid _projectGUID = new Guid(Session["ProjectGUID"].ToString());
            task.ProjectGUID = _projectGUID;
            task.TaskGUID = Guid.NewGuid();
            task.EstWorkTime = task.GetEstWorkTime(System.Web.HttpContext.Current.Application["Holidays"] as HolidaysVM);
            task.StartDate = task.EstStartDate;
            task.EndDate = task.EstEndDate;
            task.TaskStatusID = (int)ProjectManager.Models.TasksBL.Task_Status.Discussing;
            taskRepo.Add(task);
            return Json("success", JsonRequestBehavior.AllowGet);
            //return RedirectToAction("ProjectDistribution");
        }
        [HttpGet]
        public ActionResult EditTask(Guid? TaskGUID)
        {
            var task = taskRepo.Find(TaskGUID);
            return Content(JsonConvert.SerializeObject(task), "application/json");
        }
        [HttpPost]
        public ActionResult EditTask(Tasks taskModified)
        {
            Tasks recentTask = taskRepo.Find(taskModified.TaskGUID);
            recentTask.TaskName = taskModified.TaskName;            
            recentTask.Tag = taskModified.Tag;
            recentTask.EstStartDate = taskModified.EstStartDate;
            recentTask.EstEndDate = taskModified.EstEndDate;
            recentTask.StartDate = taskModified.EstStartDate;
            recentTask.Description = taskModified.Description;
            recentTask.EstWorkTime = taskModified.GetEstWorkTime(System.Web.HttpContext.Current.Application["Holidays"] as HolidaysVM);
           
            taskRepo.Update(recentTask);            
            return Json("success", JsonRequestBehavior.AllowGet);
            //return RedirectToAction("ProjectDistribution");
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
        public ActionResult GetChildTaskCount(Tasks _task)
        {
            Tasks recentTask = taskRepo.Find(_task.TaskGUID);
            var childTasks = recentTask.GetAllChildTasks();
            return Json(childTasks.Count(),JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult LoadHolidays(HolidaysVM holidays)
        {
            Session["Holidays"] = "loaded";
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application["Holidays"] = holidays;
            System.Web.HttpContext.Current.Application.UnLock();
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult TaskAcceptance( bool isConfirmed,Guid? taskGuid, int? reviewScore)
        {
            Tasks _task = taskRepo.GetCollections().Where(t => t.TaskGUID == taskGuid).FirstOrDefault();
            if (reviewScore != null)
            {
                _task.ReviewScore = byte.Parse(reviewScore.ToString());
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
            _task.TaskStatusID = (int)TasksBL.Task_Status.Ended;
            taskRepo.Update(_task);
            return Json("success", JsonRequestBehavior.AllowGet);
        }
    }
}
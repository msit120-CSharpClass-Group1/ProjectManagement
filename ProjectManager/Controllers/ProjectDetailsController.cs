using Newtonsoft.Json;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    public class ProjectDetailsController : Controller
    {
        Repository<Tasks> taskRepo = new Repository<Tasks>();
        Repository<Project> projectRepo = new Repository<Project>();
        // GET: ProjectDetails

        Repository<Employee> employee = new Repository<Employee>();
        Repository<Department> dep = new Repository<Department>();
        Repository<ProjectMembers> projectMembers = new Repository<ProjectMembers>();
        Repository<Tasks> tasks = new Repository<Tasks>();

        public ActionResult Index(/*Guid ProjectGUID*/)
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid indexPJID = new Guid(Session["ProjectGUID"].ToString());
            ViewBag.FirstEmpList = employee.GetCollections().ToList();
            ViewBag.ThisProjectMember = projectMembers.GetCollections().Where(p => p.ProjectGUID == indexPJID).ToList();
            return View(dep.GetCollections());
        }

        public ActionResult SelectDep()
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            var depGUID = new Guid(Request.QueryString["depid"]);
            var emp = employee.GetCollections().Where(e => e.Department.DepartmentGUID == depGUID);
            return Content(JsonConvert.SerializeObject(emp), "application/json");
        }

        public ActionResult AddProjectMember(Guid memberID)
        {

            ProjectMembers pm = new ProjectMembers();
            pm.ProjectGUID = new Guid(Session["ProjectGUID"].ToString());
            pm.EmployeeGUID = memberID;
            projectMembers.Add(pm);
            return RedirectToAction("Index", "ProjectDetails");
        }

        public ActionResult DeleteProjectMember()
        {
            Guid memberID = new Guid(Request.QueryString["memberID"]);
            Guid InvitePJGUID = new Guid(Session["ProjectGUID"].ToString());
            projectMembers.Delete(projectMembers.Find(memberID, InvitePJGUID));
            return RedirectToAction("Index", "ProjectDetails");
        }

        public ActionResult ReloadTeamCount()
        {
            Guid InvitePJGUID = new Guid(Session["ProjectGUID"].ToString());
            var pjmb = projectMembers.GetCollections().Where(p => p.ProjectGUID == InvitePJGUID);
            return Content(JsonConvert.SerializeObject(pjmb), "application/json");
        }

        public ActionResult TaskExist(Guid? memberGUID)
        {
            Guid projectGUID = new Guid(Session["ProjectGUID"].ToString());
            var q = tasks.GetCollections().Where(t => t.EmployeeGUID == memberGUID && t.ProjectGUID == projectGUID).Select(t=>t.EmployeeGUID).FirstOrDefault();
            if (q!=null)
            {
                return Content("HasTask");
            }
            return Content("NoTask");
        }

        public ActionResult ProjectReport(Guid? ProjectGUID)
        {
            if (ProjectGUID != null)
            {
                Session["ProjectGUID"] = ProjectGUID;
                Response.Cookies["ProjectGUID"].Value = ProjectGUID.ToString();
                Response.Cookies["ProjectGUID"].Expires = DateTime.Now.AddDays(7);
            }

            return View();
        }
        public ActionResult RootTasksCompletedRate()
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid _projectGUID = new Guid(Session["ProjectGUID"].ToString());
            var _tasks = taskRepo.GetCollections().Where(t => t.ProjectGUID == _projectGUID).OrderBy(t => t.TaskID);
            var rootTasks = _tasks.GetRootTasks();

            ChartData<BarChartDataset> _data = new ChartData<BarChartDataset>();
            _data.labels.AddRange(rootTasks.Select(t => t.TaskName));
            _data.datasets.Add(new BarChartDataset() {
                label="項目完成度",
                backgroundColor= "#007BFF",
                borderColor= "#007BFF",
                data = rootTasks.GetRootTasksCompletedRate(_tasks).ToList()
            });
            return Json(_data,JsonRequestBehavior.AllowGet);
        }
        public ActionResult RootTasksEstWorkTimeSum()
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid _projectGUID = new Guid(Session["ProjectGUID"].ToString());
            List<string> colors = new List<string>() { "#007BFF", "#4B0082", "#ADD8E6", "#B0C4DE", "#7744FF", "#CCEEFF" };
            var _tasks = taskRepo.GetCollections().Where(t => t.ProjectGUID == _projectGUID).OrderBy(t => t.TaskID);
            var rootTasks = _tasks.GetRootTasks();
            ChartData<PieChartDataset> _data = new ChartData<PieChartDataset>();
            _data.labels.AddRange(rootTasks.Select(t => t.TaskName));
            _data.datasets.Add(new PieChartDataset()
            {
                label = "dataset",
                backgroundColor = colors,
                borderColor = colors,
                data = rootTasks.GetRootTasksWorkTimeSum(_tasks).ToList()
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

        public ActionResult AssignTask()
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid SendprojectGUID = new Guid(Session["ProjectGUID"].ToString());
            ViewBag.LoadTask = tasks.GetCollections().Where(t => t.TaskStatusID == 2 && t.ProjectGUID == SendprojectGUID).GetLeafTasks();
            ViewBag.Workload = tasks.GetCollections().GetTeamWorkLoad();
            return View(projectMembers.GetCollections().Where(p => p.ProjectGUID == SendprojectGUID));
        }

        public ActionResult EditTaskM()
        {
            if (Request.Form["TotalRow"] != "")
            {
                var TotalRow = Convert.ToInt32(Request.Form["TotalRow"]);
                for (int i = 0; i < TotalRow; i++)
                {
                    var EmpGUID = new Guid(Request.Form["EmployeeGUID" + i]);
                    var TaskGUID = new Guid(Request.Form["TaskGUID" + i]);
                    Tasks _tasks = tasks.Find(TaskGUID);
                    _tasks.EmployeeGUID = EmpGUID;
                    tasks.Update(_tasks);
                }
            }
            return RedirectToAction("AssignTask");
        }

        public ActionResult ReloadTaskList()
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid SendprojectGUID = new Guid(Session["ProjectGUID"].ToString());
            var taskList = tasks.GetCollections().Where(t => t.ProjectGUID == SendprojectGUID && t.TaskStatusID == 2).GetLeafTasks().ToList();
            return Content(JsonConvert.SerializeObject(taskList), "application/json");
        }

        public ActionResult LeaveMessageTag()
        {
            var message = Request.Form["text"];
            Guid TaskGUID = new Guid(Request.Form["TaskGUID"].ToString());
            Tasks _tasks = tasks.Find(TaskGUID);
            _tasks.Tag = message;
            tasks.Update(_tasks);
            return RedirectToAction("AssignTask");
        }

        public ActionResult GetTaskDesc(Guid TaskGUID)
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            var TaskName = tasks.GetCollections().Where(t => t.TaskGUID == TaskGUID).FirstOrDefault().Description;
            return Content(TaskName);
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
            taskRepo.Add(task);
            return RedirectToAction("ProjectDistribution");
        }
        [HttpGet]
        public ActionResult EditTask(Guid? TaskGUID)
        {
            var task = taskRepo.Find(TaskGUID);
            return Content(JsonConvert.SerializeObject(task), "application/json");
        }
        [HttpPost]
        public ActionResult EditTask(Tasks _task)
        {
            Tasks recentTask = taskRepo.Find(_task.TaskGUID);
            recentTask.TaskName = _task.TaskName;
            recentTask.TaskStatusID = _task.TaskStatusID;
            recentTask.Tag = _task.Tag;
            recentTask.EstStartDate = _task.EstStartDate;
            recentTask.EstEndDate = _task.EstEndDate;
            recentTask.StartDate = _task.StartDate;
            recentTask.EndDate = _task.EndDate;
            recentTask.Description = _task.Description;

            taskRepo.Update(recentTask);
            return RedirectToAction("ProjectDistribution");
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
                catch (Exception ex)
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
    }
}
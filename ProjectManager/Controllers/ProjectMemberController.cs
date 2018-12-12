using Newtonsoft.Json;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員,專案經理,處長")]
    public class ProjectMemberController : Controller
    {
        Repository<Employee> employee = new Repository<Employee>();
        Repository<Department> dep = new Repository<Department>();
        Repository<ProjectMembers> projectMembers = new Repository<ProjectMembers>();
        Repository<Tasks> tasks = new Repository<Tasks>();
        // GET: ProjectMember
        public ActionResult Index()
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
            //return Json(JsonConvert.SerializeObject(emp), "application/json");
            //return Json(JsonConvert.SerializeObject(emp), "application/json");
            //return Json(emp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddProjectMember(Guid memberID)
        {
            ProjectMembers pm = new ProjectMembers();
            pm.ProjectGUID = new Guid(Session["ProjectGUID"].ToString());
            pm.EmployeeGUID = memberID;
            projectMembers.Add(pm);
            return RedirectToAction("Index", "ProjectMember");
        }

        public ActionResult DeleteProjectMember()
        {
            Guid memberID = new Guid(Request.QueryString["memberID"]);
            Guid InvitePJGUID = new Guid(Session["ProjectGUID"].ToString());
            projectMembers.Delete(projectMembers.Find(memberID, InvitePJGUID));
            return RedirectToAction("Index", "ProjectMember");
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
            var q = tasks.GetCollections().Where(t => t.EmployeeGUID == memberGUID && t.ProjectGUID == projectGUID && t.TaskStatusID == 2).Select(t => t.EmployeeGUID).FirstOrDefault();
            if (q != null)
            {
                return Content("HasTask");
            }
            return Content("NoTask");
        }

        public ActionResult AssignTask()
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid SendprojectGUID = new Guid(Session["ProjectGUID"].ToString());
            ViewBag.LoadTask = tasks.GetCollections().Where(t => t.TaskStatusID == 1 && t.ProjectGUID == SendprojectGUID).GetLeafTasks();
            ViewBag.Workload = tasks.GetCollections().GetLeafTasks().GetTeamWorkLoad();
            return View(projectMembers.GetCollections().Where(p => p.ProjectGUID == SendprojectGUID));
        }

        public ActionResult EditTaskM()
        {
            if (Request.Form["TotalRow"] != "")
            {
                var FirstRow = Convert.ToInt32(Request.Form["FirstRow"]);
                var LastRow = Convert.ToInt32(Request.Form["LastRow"]);

                for (int i = FirstRow; i <= LastRow; i++)
                {
                    if (Request.Form["EmployeeGUID" + i] != null && Request.Form["TaskGUID" + i] != null)
                    {
                        var EmpGUID = new Guid(Request.Form["EmployeeGUID" + i]);
                        var TaskGUID = new Guid(Request.Form["TaskGUID" + i]);
                        Tasks _tasks = tasks.Find(TaskGUID);
                        _tasks.EmployeeGUID = EmpGUID;
                        _tasks.AssignedDate = DateTime.Now;
                        _tasks.TaskStatusID = 2;
                        tasks.Update(_tasks);
                    }
                }
            }
            return RedirectToAction("AssignTask");
        }
        public ActionResult ReloadTaskList()
        {
            if (Session["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid SendprojectGUID = new Guid(Session["ProjectGUID"].ToString());
            var taskList = tasks.GetCollections().Where(t => t.ProjectGUID == SendprojectGUID && t.TaskStatusID == 1).GetLeafTasks().ToList();
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
    }
}
﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid indexPJID = new Guid(Request.Cookies["ProjectGUID"].Value);
            ViewBag.FirstEmpList = employee.GetCollections().ToList();
            ViewBag.ThisProjectMember = projectMembers.GetCollections().Where(p => p.ProjectGUID == indexPJID).ToList();           
            return View(dep.GetCollections());
        }
        public ActionResult SelectDep(Guid depid)
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            var emp = employee.GetCollections().Where(e => e.DepartmentGUID == depid)
                .Select(e=>new { e.DepartmentGUID,e.EmployeeGUID,e.EmployeeName,e.TitleGUID});
            return Content(JsonConvert.SerializeObject(emp), "application/json");
        }

        public ActionResult AddProjectMember(Guid memberID)
        {
            ProjectMembers pm = new ProjectMembers();
            pm.ProjectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            pm.EmployeeGUID = memberID;
            pm.IsRead = false;
            pm.InvideDate = DateTime.Now;
            projectMembers.Add(pm);
            return RedirectToAction("Index", "ProjectMember");
        }

        public ActionResult DeleteProjectMember()
        {
            Guid memberID = new Guid(Request.QueryString["memberID"]);
            Guid InvitePJGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            projectMembers.Delete(projectMembers.Find(memberID, InvitePJGUID));
            return RedirectToAction("Index", "ProjectMember");
        }

        public ActionResult ReloadTeamCount()
        {
            Guid InvitePJGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            var pjmb = projectMembers.GetCollections().Where(p => p.ProjectGUID == InvitePJGUID)
                .Select(p => new { p.EmployeeGUID, p.Employee.TitleGUID, p.Employee.EmployeeName });
            return Content(JsonConvert.SerializeObject(pjmb), "application/json");
        }

        public ActionResult TaskExist(Guid? memberGUID)
        {
            Guid projectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            var q = tasks.GetCollections().Where(t => t.EmployeeGUID == memberGUID && t.ProjectGUID == projectGUID && t.TaskStatusID == 2).Select(t => t.EmployeeGUID).FirstOrDefault();
            if (q != null)
            {
                return Content("HasTask");
            }
            return Content("NoTask");
        }

        public ActionResult AssignTask()
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid SendprojectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            var q = tasks.GetCollections().Where(t => t.ProjectGUID == SendprojectGUID).GetLeafTasks();
            ViewBag.LoadTask = q.Where(p => p.TaskStatusID == 1);
            ViewBag.Workload = tasks.GetCollections().GetLeafTasks().GetTeamWorkLoad();
            return View(projectMembers.GetCollections().Where(p => p.ProjectGUID == SendprojectGUID));
        }

        public ActionResult EditTaskM()
        {
            try
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
                            _tasks.IsRead = false;
                            tasks.Update(_tasks);
                        }
                    }
                }
            }
            catch { }
            return RedirectToAction("AssignTask");
        }
        public ActionResult ReloadTaskList()
        {
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            Guid SendprojectGUID = new Guid(Request.Cookies["ProjectGUID"].Value);
            var q = tasks.GetCollections().Where(t => t.ProjectGUID == SendprojectGUID).GetLeafTasks().ToList();
            var taskList = q.Where(t => t.TaskStatusID == 1);
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
            if (Request.Cookies["ProjectGUID"] == null)
                return RedirectToAction("Index", "Projects");
            var TaskName = tasks.GetCollections().Where(t => t.TaskGUID == TaskGUID).FirstOrDefault().Description;
            return Content(TaskName);
        }

        public ActionResult GetProjectMemberTasks(Guid EmployeeGUID)
        {
            var memberTask = tasks.GetCollections().Where(t => t.EmployeeGUID == EmployeeGUID && t.TaskStatusID==2 &&  t.ProjectGUID == new Guid(Request.Cookies["ProjectGUID"].Value));
            return Content(JsonConvert.SerializeObject(memberTask), "application/json");
        }

        public ActionResult CancelTask(Guid TaskGUID)
        {
            Tasks _tasks = tasks.Find(TaskGUID);
            _tasks.EmployeeGUID = null;
            _tasks.AssignedDate = null;
            _tasks.TaskStatusID = 1;
            _tasks.IsRead = true;
            tasks.Update(_tasks);
            return Content("已退回分配工作項目清單");
        }
    }
}
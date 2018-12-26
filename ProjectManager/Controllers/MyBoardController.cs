using Newtonsoft.Json;
using ProjectManager.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ProjectManager.Controllers
{
    [Authorize]
    public class MyBoardController : Controller
    {
        // GET: MyBoard
        Repository<ProjectManager.Models.TaskDetail> td = new Repository<ProjectManager.Models.TaskDetail>();
        Repository<ProjectManager.Models.TaskStatus> s = new Repository<ProjectManager.Models.TaskStatus>();
        Repository<ProjectManager.Models.Tasks> t = new Repository<ProjectManager.Models.Tasks>();
        Repository<ProjectManager.Models.Project> p = new Repository<ProjectManager.Models.Project>();
        Repository<ProjectManager.Models.Members> m = new Repository<ProjectManager.Models.Members>();
        Repository<ProjectManager.Models.Employee> e = new Repository<ProjectManager.Models.Employee>();
        Repository<ProjectManager.Models.ProjectMembers> pm = new Repository<ProjectManager.Models.ProjectMembers>();

        public ActionResult Index(Guid id)
        {
            BoardVM VM = new BoardVM();
            var PID = Request.Cookies["PID"].Value;
            var q = from parentTask in t.GetCollections()
                    join childrenTask in t.GetCollections() on parentTask.TaskGUID equals childrenTask.ParentTaskGUID
                    select childrenTask;
            VM.TaskStatus = s.GetCollections().Where(x => x.TaskStatusID != 1);
            VM.Tasks = q.Where(x => x.ProjectGUID.ToString() == PID && x.EmployeeGUID == id).OrderBy(x=>x.EstEndDate).ToList();
            VM.Project = p.GetCollections().Where(x => x.ProjectGUID.ToString() == PID).ToList();
            VM.TaskDetail = td.GetCollections();
            ViewBag.UserName = e.Find(id).EmployeeName+ "的看板";
            q.ToList();
            var _pm = pm.GetCollections().Where(n => n.EmployeeGUID == id && n.ProjectGUID.ToString() == PID).ToList();
            foreach (var _p in _pm)
            {
                if (!(bool)_p.IsRead)
                {
                    _p.IsRead = true;
                    pm.Update(_p);
                }
            }
            foreach (var task in VM.Tasks)
            {
                if(!(bool)task.IsRead)
                {
                    task.IsRead = true;
                    t.Update(task);
                }
            }
            return View(VM);
        }
        public ActionResult GetEmployee(Guid id)
        {
            var EmployeeGuid = m.Find(new Guid(Request.Cookies["MemberGUID"].Value)).EmployeeGUID;

            if (id == EmployeeGuid)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }

        }
        public ActionResult GetDetailTask(Guid id)
        {
            BoardVM VM = new BoardVM();
            VM.TaskDetail = td.GetCollections().Where(x => x.TaskGUID == id && x.TaskDetailStatusID == 2).ToList();
            return Json(VM.TaskDetail.Count());

        }
        public ActionResult GetCard(Guid id)
        {
            BoardVM VM = new BoardVM();
            VM.Task = t.Find(id);
            return Content(JsonConvert.SerializeObject(VM), "application/Json");

        }
        public ActionResult GetDetail(Guid id)
        {

            BoardVM VM = new BoardVM();
            VM.TaskDetail = td.GetCollections().Where(n => n.TaskGUID == id).ToList();
            return Json(VM.TaskDetail.Select(n => new { n.TaskDetailName, n.TaskDetailGUID, n.TaskDetailStatusID }).ToList());
        }


        public ActionResult EditTaskStatusID(Guid id, int TaskStatusID, int? WorkTime)
        {

            BoardVM VM = new BoardVM();
            VM.Task = t.Find(id);
            VM.Task.TaskStatusID = TaskStatusID;
            if (TaskStatusID == 3)
            {
                VM.Task.EndDate = DateTime.Now;
                VM.Task.WorkTime = WorkTime;
            }
            t.Update(VM.Task);
            return Json(true);
        }
     
        public ActionResult InsertDetailTask(TaskDetail taskDetail)
        {
            if (taskDetail != null)
            {
                taskDetail.TaskDetailGUID = Guid.NewGuid();
                taskDetail.StartDate = DateTime.Now;
                taskDetail.EndDate = DateTime.Now;
                taskDetail.EmployeeGUID = m.Find(new Guid(Request.Cookies["MemberGUID"].Value)).EmployeeGUID;
                td.Add(taskDetail);
            }
            return Json(taskDetail.TaskDetailGUID);
        }
        public ActionResult EditDetailStatus(Guid id, int TaskDetailStatusID)
        {
            BoardVM VM = new BoardVM();
            VM.TaskDetails = td.Find(id);
            VM.TaskDetails.TaskDetailStatusID = TaskDetailStatusID;
            td.Update(VM.TaskDetails);
            return Json(true);
        }

        public ActionResult EditTaskDatail(Guid id, string TaskDetailName)
        {
            BoardVM VM = new BoardVM();
            VM.TaskDetails = td.Find(id);
            VM.TaskDetails.TaskDetailName = TaskDetailName;
            td.Update(VM.TaskDetails);
            return Json(true);
        }

        public ActionResult DeleteTaskDatail(Guid id, Guid cardID)
        {
            BoardVM VM = new BoardVM();
            VM.TaskDetails = td.Find(id);
            td.Delete(VM.TaskDetails);
            return Json(td.GetCollections().Where(x => x.TaskGUID == cardID).Count());
        }

        public ActionResult ExpiredTask(Guid id)
        {
            BoardVM VM = new BoardVM();
            VM.Task = t.Find(id);
            return Json(VM.Task.EstEndDate);
        }

    }
}
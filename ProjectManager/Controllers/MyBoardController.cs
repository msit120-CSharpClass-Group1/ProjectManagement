
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
    public class MyBoardController : Controller
    {
        // GET: MyBoard
        Repository<ProjectManager.Models.TaskDetail> td = new Repository<ProjectManager.Models.TaskDetail>();
        Repository<ProjectManager.Models.TaskStatus> s = new Repository<ProjectManager.Models.TaskStatus>();
        Repository<ProjectManager.Models.Tasks> t = new Repository<ProjectManager.Models.Tasks>();
        Repository<ProjectManager.Models.Project> p = new Repository<ProjectManager.Models.Project>();
        public ActionResult Index(Guid id)
        {
            BoardVM VM = new BoardVM();
            var PID = Request.Cookies["PID"].Value;
            VM.TaskStatus = s.GetCollections();
            VM.Tasks = t.GetCollections().Where(x => x.ProjectGUID.ToString() == PID && x.EmployeeGUID == id).ToList();
            VM.Project = p.GetCollections().Where(x => x.ProjectGUID.ToString() == PID).ToList();
            VM.TaskDetail = td.GetCollections();

           
           

            var q = from parentTask in t.GetCollections()
                    join childrenTask in t.GetCollections() on parentTask.TaskGUID equals childrenTask.ParentTaskGUID
                    select childrenTask;

            return View(VM);
        }
        public ActionResult GetCard(Guid id)
        {
            BoardVM VM = new BoardVM();
            VM.Task = t.Find(id);
            return Content(JsonConvert.SerializeObject(VM),"application/Json");

        }
        public ActionResult GetDetail(Guid id)
        {
            BoardVM VM = new BoardVM();
            VM.TaskDetail = td.GetCollections().Where(n => n.TaskGUID == id).ToList();
            return Content(JsonConvert.SerializeObject(VM.TaskDetail.Select(n=>n.TaskDetailName).ToList()), "application/Json");

        }

        public ActionResult InsertDetailTask(TaskDetail taskDetail)
        {
            if (taskDetail!=null)
            {
                taskDetail.TaskDetailGUID = Guid.NewGuid();
                taskDetail.TaskDetailStatusID = 1;
                taskDetail.StartDate = DateTime.Now;
                taskDetail.EndDate = DateTime.Now;
                td.Add(taskDetail);
            }
            return Json(td.GetCollections().Where(x=>x.TaskGUID==new Guid (Request.Form["TaskGUID"])).Count());
        }
    }
}
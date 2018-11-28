
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
            return View(VM);
        }
       
        public ActionResult GetCard(Guid id)
        {
            BoardVM VM = new BoardVM();
            VM.Task = t.Find(id);
            return Content(JsonConvert.SerializeObject(VM),"application/Json");

        }
    }
}
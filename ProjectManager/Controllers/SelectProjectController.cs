using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;


namespace ProjectManager.Controllers
{
    [Authorize]
    public class SelectProjectController : Controller
    {
        Repository<ProjectManager.Models.ProjectMembers> pm = new Repository<ProjectManager.Models.ProjectMembers>();
        Repository<ProjectManager.Models.Members> m = new Repository<ProjectManager.Models.Members>();
        Repository<ProjectManager.Models.Project> p = new Repository<ProjectManager.Models.Project>();
        Repository<ProjectManager.Models.Tasks> t = new Repository<ProjectManager.Models.Tasks>();

        // GET: SelectProject
        public ActionResult Index()
        {
            BoardVM VM = new BoardVM();
            var members = m.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            VM.ProjectMember = pm.GetCollections().Where(x => x.EmployeeGUID == members.EmployeeGUID && x.Project.ProjectStatusID == 1).ToList();
            return View(VM);
        }

        public ActionResult AllTasks()
        {
            BoardVM VM = new BoardVM();
            var members = m.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var q = from parentTask in t.GetCollections();
                    join childrenTask in t.GetCollections() on parentTask.TaskGUID equals childrenTask.ParentTaskGUID
                    select childrenTask;

            VM.Tasks = q.Where(x => x.EmployeeGUID == members.EmployeeGUID).ToList();
            return PartialView(VM);
        }

    }
}
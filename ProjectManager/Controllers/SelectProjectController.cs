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

        // GET: SelectProject
        public ActionResult Index()
        {
            BoardVM VM = new BoardVM();
            var members = m.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            //VM.GetProjectDepartment = pm.GetCollections().Where(x => x.EmployeeGUID == members.EmployeeGUID && x.Project.ProjectStatusID == 1).GroupBy(x=>x.Project.Department.DepartmentName).Select(g=>new )
            VM.ProjectMember = pm.GetCollections().Where(x => x.EmployeeGUID == members.EmployeeGUID && x.Project.ProjectStatusID == 1).ToList();
            //var ProjectList = pm.GetCollections().Where(x=>x.EmployeeGUID==members.EmployeeGUID && x.Project.ProjectStatusID == 1).ToList();
            return View(VM);
        }

        
    }
}
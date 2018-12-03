using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;


namespace ProjectManager.Controllers
{
    public class SelectProjectController : Controller
    {
        Repository<ProjectManager.Models.ProjectMembers> pm = new Repository<ProjectManager.Models.ProjectMembers>();
        Repository<ProjectManager.Models.Members> m = new Repository<ProjectManager.Models.Members>();

        // GET: SelectProject
        public ActionResult Index()
        {
            var members = m.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var ProjectList = pm.GetCollections().Where(x=>x.EmployeeGUID==members.EmployeeGUID);
            return View(ProjectList);
        }

        
    }
}
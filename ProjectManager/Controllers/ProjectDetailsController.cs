using Newtonsoft.Json;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    public class ProjectDetailsController : Controller
    {
        // GET: ProjectDetails
        Repository<Employee> employee = new Repository<Employee>();
        Repository<Department> dep = new Repository<Department>();
        Repository<ProjectMembers> projectMembers = new Repository<ProjectMembers>();

        public ActionResult Index(Guid ProjectGUID)
        {
            Session["ProjectGUID"] = ProjectGUID;
            Guid indexPJID = new Guid(Session["ProjectGUID"].ToString());
            ViewBag.FirstEmpList = employee.GetCollections().ToList();
            ViewBag.ThisProjectMember = projectMembers.GetCollections().Where(p => p.ProjectGUID == indexPJID).ToList();     
            return View(dep.GetCollections());          
        }

        public ActionResult SelectDep()
        {
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
            return Content("html...");
        }

        public ActionResult DeleteProjectMember()
        {
            Guid memberID = new Guid(Request.QueryString["memberID"]);
            Guid InvitePJGUID = new Guid(Session["ProjectGUID"].ToString());
            projectMembers.Delete(projectMembers.Find(memberID, InvitePJGUID));
            return Content("html...");
        }

        public ActionResult ReloadTeamCount()
        {
            Guid InvitePJGUID = new Guid(Session["ProjectGUID"].ToString());
            var pjmb = projectMembers.GetCollections().Where(p => p.ProjectGUID == InvitePJGUID);
            return Content(JsonConvert.SerializeObject(pjmb), "application/json");
        }
    }
}
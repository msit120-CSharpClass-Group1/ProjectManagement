using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        IRepository<Tasks> tasks = new Repository<Tasks>();
        IRepository<Members> members = new Repository<Members>();
        // GET: Notification
        public ActionResult Index()
        {
            var _members = members.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var _tasks = tasks.GetCollections()
                .Where(n => n.EmployeeGUID == _members.EmployeeGUID)
                .OrderByDescending(n => n.AssignedDate).Take(5);
            return View(_tasks);
        }
        public ActionResult Load()
        {
            var _members =members.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var _tasks =tasks.GetCollections()
                .Where(n => n.EmployeeGUID == _members.EmployeeGUID)
                .Select(n=>new { n.TaskName,n.AssignedDate,n.IsRead,n.EmployeeGUID,n.Project.ProjectName,n.ProjectGUID,n.Project.Employee1.EmployeeName})
                .OrderByDescending(n=>n.AssignedDate).Take(5);
            return Json(_tasks);
        }
    }
}
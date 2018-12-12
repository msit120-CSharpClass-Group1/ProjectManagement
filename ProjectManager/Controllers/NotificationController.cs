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
            return View();
        }
        public ActionResult Load()
        {
            var _members =members.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var _tasks =tasks.GetCollections().Where(n => n.EmployeeGUID == _members.EmployeeGUID).Select(n=>new { n.TaskName,n.AssignedDate,n.IsRead}).OrderByDescending(n=>n.AssignedDate).Take(5);
            return Json(_tasks);
        }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using ProjectManager.Models.BLL;

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
            var _notifications = _members.GetNotifications().OrderByDescending(n => n.NotificationDate);
            return View(_notifications);
        }
        public ActionResult Load()
        {
            var _members =members.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var _notifications = _members.GetNotifications().OrderByDescending(n=>n.NotificationDate).Take(5);
            return Json(_notifications);
        }
    }
}
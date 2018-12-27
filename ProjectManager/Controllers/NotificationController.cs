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
        Repository<Members> members = new Repository<Members>();
        Repository<Tasks> tasks = new Repository<Tasks>();
        Repository<ProjectMembers> projectMembers = new Repository<ProjectMembers>();
        Repository<Calendar> calendars = new Repository<Calendar>();
        // GET: Notification
        public ActionResult Index()
        {
            var _members = members.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var _notifications = _members.GetNotifications(tasks.GetCollections().ToList(), projectMembers.GetCollections().ToList(), calendars.GetCollections().ToList()).OrderByDescending(n => n.NotificationDate);
            return View(_notifications);
        }
        public ActionResult Load()
        {
            var _members =members.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var _notifications = _members.GetNotifications(tasks.GetCollections().ToList(), projectMembers.GetCollections().ToList(), calendars.GetCollections().ToList()).OrderByDescending(n=>n.NotificationDate).ToList().Take(5);
            return Json(_notifications);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using Newtonsoft.Json;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private Repository<Calendar> CalRepo = new Repository<Calendar>();
        private Repository<Project> ProjectRepo = new Repository<Project>();
        private Repository<ProjectMembers> ProjectMemberRepo = new Repository<ProjectMembers>();
        private Repository<Members> MembersRepo = new Repository<Members>();
        
        // GET: Calendar
        public ActionResult Index()
        {
            Guid memberGUID = new Guid(Request.Cookies["MemberGUID"].Value);
            var events = CalRepo.GetCollections().Where(n => n.MemberGUID == memberGUID).ToList();
            foreach (var _events in events)
            {
                if (!(bool)_events.IsRead)
                {
                    _events.IsRead = true;
                    CalRepo.Update(_events);
                }
            }
            return View(ProjectRepo.GetCollections());
        }

        public ActionResult GetEvents()
        {
            Guid memberGUID = new Guid(Request.Cookies["MemberGUID"].Value);
            var events = CalRepo.GetCollections().Where(n => n.MemberGUID == memberGUID).ToList();
            return Content(JsonConvert.SerializeObject(events), "application/json");
        }

        [HttpPost]
        public JsonResult SaveEvent(Calendar e)
        {
            var status = false;
         
                if (e.EventID > 0)
                {
                    //Update the event
                    var v = CalRepo.GetCollections().Where(a => a.EventID == e.EventID).FirstOrDefault();
                    if (v != null)
                    {                                            
                        //v.CalendarGUID = e.CalendarGUID;
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.EndDay = e.EndDay;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                        CalRepo.Update(v);
                    }
                }
                else
                {
                   CalRepo.Add(e);
                }

            status = true;

            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            var v = CalRepo.GetCollections().Where(a => a.EventID == eventID).FirstOrDefault();
            
            if (v != null)
            {
                CalRepo.Delete(v);                
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        public ActionResult SaveEventToAll(Project _project, Calendar _calendar)
        {
            var ProjectMember = ProjectMemberRepo.GetCollections().Where(p => p.ProjectGUID == _project.ProjectGUID);
            foreach (var item in ProjectMember)
            {               
                if(item.Employee.Members.Count()!=0)
                {
                    Calendar calendar = new Calendar();
                    calendar.MemberGUID = item.Employee.Members.Select(p => p.MemberGUID).Single();
                    calendar.CalendarGUID = Guid.NewGuid();
                    calendar.Subject = _calendar.Subject;
                    calendar.Start = _calendar.Start;
                    calendar.EndDay = _calendar.EndDay;
                    calendar.Description = _calendar.Description;
                    calendar.IsFullDay = _calendar.IsFullDay;
                    calendar.ThemeColor = _calendar.ThemeColor;
                    calendar.CreateDate = DateTime.Now;
                    calendar.IsRead = false;
                    calendar.CategoryID = 2;
                    CalRepo.Add(calendar);
                }
            }                  
            return Content("活動已新增");
        }
    }
}
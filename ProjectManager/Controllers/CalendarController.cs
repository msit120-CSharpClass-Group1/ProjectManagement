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
        // GET: Calendar
        public ActionResult Index()
        {
            return View();
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

    }
}
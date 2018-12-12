using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    //[Authorize(Roles = "管理員,工程師")]
    [Authorize]
    public class HomeController : Controller
    {
        Repository<Tasks> taskRepo = new Repository<Tasks>();
        Repository<TaskStatus> taskStatusRepo = new Repository<TaskStatus>();
        Repository<Members> memberRepo = new Repository<Members>();
        
        
        public ActionResult Index()
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var tasksInProgress = taskRepo.GetCollections().Where(t => t.EmployeeGUID == member.EmployeeGUID)
                .Where(t=>t.Project.ProjectStatusID == (int)ProjectsBL.Project_Status.InProgress);

            ViewBag.UserGUID = member.EmployeeGUID;
            ViewBag.UserName = member.Employee.EmployeeName;
            ViewBag.TasksSum = tasksInProgress.Count();

            List<DisplayTaskStatusCountVM> statusCounts = new List<DisplayTaskStatusCountVM>() {
                new DisplayTaskStatusCountVM()
                {
                    TaskStatusName = taskStatusRepo.GetCollections().Where(s=>s.TaskStatusID==(int)TasksBL.Task_Status.InProgress).Select(s=>s.TaskStatusName).FirstOrDefault(),
                    Count = tasksInProgress.Where(t => t.TaskStatus.TaskStatusID == (int)TasksBL.Task_Status.InProgress).Count()
                },
                new DisplayTaskStatusCountVM()
                {
                    TaskStatusName = taskStatusRepo.GetCollections().Where(s=>s.TaskStatusID==(int)TasksBL.Task_Status.WaitForConfirmed).Select(s=>s.TaskStatusName).FirstOrDefault(),
                    Count = tasksInProgress.Where(t => t.TaskStatus.TaskStatusID == (int)TasksBL.Task_Status.WaitForConfirmed).Count()
                },
                new DisplayTaskStatusCountVM()
                {
                    TaskStatusName = taskStatusRepo.GetCollections().Where(s=>s.TaskStatusID==(int)TasksBL.Task_Status.Completed).Select(s=>s.TaskStatusName).FirstOrDefault(),
                    Count = tasksInProgress.Where(t => t.TaskStatus.TaskStatusID == (int)TasksBL.Task_Status.Completed).Count()
                },
                new DisplayTaskStatusCountVM()
                {
                    TaskStatusName = taskStatusRepo.GetCollections().Where(s=>s.TaskStatusID==(int)TasksBL.Task_Status.Closed).Select(s=>s.TaskStatusName).FirstOrDefault(),
                    Count = tasksInProgress.Where(t => t.TaskStatus.TaskStatusID == (int)TasksBL.Task_Status.Closed).Count()
                }
            };
            ViewBag.TaskStatusCounts = statusCounts;

            return View(tasksInProgress.GetTasksGroupByStatus());
        }
        [HttpPost]
        public ActionResult SetCookiesForMyBoard(Guid? projectGUID)
        {
            var userGUID = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value)).EmployeeGUID;
            Response.Cookies["PID"].Value = projectGUID.ToString();
            return Json(userGUID, JsonRequestBehavior.AllowGet);
        }
    }
}
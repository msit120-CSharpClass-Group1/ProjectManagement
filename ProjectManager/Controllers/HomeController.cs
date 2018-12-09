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
        Repository<Members> memberRepo = new Repository<Members>();
        
        public ActionResult Index()
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var tasksInProgress = taskRepo.GetCollections().Where(t => t.EmployeeGUID == member.EmployeeGUID)
                .Where(t=>t.Project.ProjectStatusID == (int)ProjectsBL.Project_Status.InProgress);

            ViewBag.UserName = member.Employee.EmployeeName;
            ViewBag.TasksSum = tasksInProgress.Count();            
            ViewBag.InProgressCount = tasksInProgress.Where(t => t.TaskStatus.TaskStatusID == (int)TasksBL.Task_Status.InProgress).Count();
            ViewBag.WaitForConfirmedCount = tasksInProgress.Where(t => t.TaskStatus.TaskStatusID == (int)TasksBL.Task_Status.WaitForConfirmed).Count();
            ViewBag.CompletedCount = tasksInProgress.Where(t => t.TaskStatus.TaskStatusID == (int)TasksBL.Task_Status.Completed).Count();
            ViewBag.EndedCount = tasksInProgress.Where(t => t.TaskStatus.TaskStatusID == (int)TasksBL.Task_Status.Ended).Count();

            return View(tasksInProgress.GetTasksGroupByStatus());
        }
    }
}
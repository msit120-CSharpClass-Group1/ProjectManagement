using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ProjectManager.Models.PermissionBL;

namespace ProjectManager.Controllers
{
    //[Authorize(Roles = "管理員,工程師")]
    [Authorize]
    public class HomeController : Controller
    {
        Repository<Tasks> taskRepo = new Repository<Tasks>();
        Repository<TaskStatus> taskStatusRepo = new Repository<TaskStatus>();
        Repository<Members> memberRepo = new Repository<Members>();
        Repository<JobTitle> jobTitleRepo = new Repository<JobTitle>();
        Repository<WidgetDetail> widgetDetailRepo = new Repository<WidgetDetail>();
        Repository<Widgets> widgetRepo = new Repository<Widgets>();
        Repository<Dashboard> dashboardRepo = new Repository<Dashboard>();        

        public ActionResult Index()
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var titleID = jobTitleRepo.Find(new Guid(Request.Cookies["TitleGUID"].Value)).TitleID;
            var tasksInProgress = taskRepo.GetCollections().Where(t => t.EmployeeGUID == member.EmployeeGUID)
                .Where(t => t.Project.ProjectStatusID == (int)ProjectsBL.Project_Status.InProgress);

            ViewBag.UserGUID = member.EmployeeGUID;
            ViewBag.UserName = member.Employee.EmployeeName;
            ViewBag.TasksSum = tasksInProgress.Count();
            
            //Dictionary<string, int> taskStatusDic = new Dictionary<string, int>();
            //for (int i = 2; i <=5 ; i++)
            //{
            //    taskStatusDic.Add(
            //    taskStatusRepo.GetCollections().Where(s => s.TaskStatusID == i).Select(s => s.TaskStatusName).FirstOrDefault(),
            //    tasksInProgress.Where(t => t.TaskStatus.TaskStatusID == i).Count());
            //}
            
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

            switch ((Emp_Title)Enum.Parse(typeof(Emp_Title), titleID.ToString()))
            {
                case Emp_Title.Admin:
                    ViewBag.Widgets = widgetRepo.GetCollections().Where(w => w.AdminPermit == true).ToList();
                    break;
                case Emp_Title.PG:
                    ViewBag.Widgets = widgetRepo.GetCollections().Where(w => w.ProgrammerPermit == true).ToList();
                    break;
                case Emp_Title.Minister:
                    ViewBag.Widgets = widgetRepo.GetCollections().Where(w => w.MinisterPermit == true).ToList();
                    break;
                case Emp_Title.PM:
                    ViewBag.Widgets = widgetRepo.GetCollections().Where(w => w.ProjectManagerPermit == true).ToList();
                    break;
                case Emp_Title.Director:
                    ViewBag.Widgets = widgetRepo.GetCollections().Where(w => w.DirectorPermit == true).ToList();
                    break;
                default:
                    ViewBag.Widgets = null;
                    break;
            }


            return View(tasksInProgress.GetTasksGroupByStatus());
        }
        public ActionResult GetMyTasksWidgetPV()
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var tasksInProgress = taskRepo.GetCollections().Where(t => t.EmployeeGUID == member.EmployeeGUID)
                .Where(t => t.Project.ProjectStatusID == (int)ProjectsBL.Project_Status.InProgress);
            ViewBag.UserName = member.Employee.EmployeeName;
            ViewBag.TasksSum = tasksInProgress.Count();

            return PartialView(tasksInProgress.GetTasksGroupByStatus());
        }
        public ActionResult GetBurndownChartPV()
        {
            return PartialView();
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
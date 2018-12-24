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
        Repository<Employee> employeeRepo = new Repository<Employee>();
        Repository<ProjectMembers> projectMemberRepo = new Repository<ProjectMembers>();
        Repository<Project> projectRepo = new Repository<Project>();
        Repository<JobTitle> jobTitleRepo = new Repository<JobTitle>();
        Repository<WidgetDetail> widgetDetailRepo = new Repository<WidgetDetail>();
        Repository<Widgets> widgetRepo = new Repository<Widgets>();
        Repository<Dashboard> dashboardRepo = new Repository<Dashboard>();
        Repository<ProjectStatus> projectStatusRepo = new Repository<ProjectStatus>();

        public ActionResult Index()
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var titleID = jobTitleRepo.Find(new Guid(Request.Cookies["TitleGUID"].Value)).TitleID;
            var tasksInProgress = taskRepo.GetCollections().Where(t => t.EmployeeGUID == member.EmployeeGUID)
                .Where(t => t.Project.ProjectStatusID == (int)ProjectsBL.Project_Status.InProgress);

            ViewBag.UserGUID = member.EmployeeGUID;

            //Dictionary<string, int> taskStatusDic = new Dictionary<string, int>();
            //for (int i = 2; i <=5 ; i++)
            //{
            //    taskStatusDic.Add(
            //    taskStatusRepo.GetCollections().Where(s => s.TaskStatusID == i).Select(s => s.TaskStatusName).FirstOrDefault(),
            //    tasksInProgress.Where(t => t.TaskStatus.TaskStatusID == i).Count());
            //}
            

            switch ((Emp_Title)Enum.Parse(typeof(Emp_Title), titleID.ToString()))
            {
                case Emp_Title.Admin:
                    var _projectlist0 = projectMemberRepo.GetCollections().Where(n => n.EmployeeGUID == member.EmployeeGUID).Select(n => new {n.ProjectGUID,n.Project.ProjectName });
                    ViewBag.ProjectList = new SelectList(_projectlist0, "ProjectGUID", "ProjectName");
                    ViewBag.Widgets = widgetRepo.GetCollections().Where(w => w.AdminPermit == true).ToList();
                    break;
                case Emp_Title.PG:
                    var _projectlist1 = projectMemberRepo.GetCollections().Where(n => n.EmployeeGUID == member.EmployeeGUID).Select(n => new { n.ProjectGUID, n.Project.ProjectName });
                    ViewBag.ProjectList = new SelectList(_projectlist1, "ProjectGUID", "ProjectName");
                    ViewBag.Widgets = widgetRepo.GetCollections().Where(w => w.ProgrammerPermit == true).ToList();
                    break;
                case Emp_Title.Minister:
                    var _projectlist3 = projectRepo.GetCollections().Select(n => new { n.ProjectGUID, n.ProjectName });
                    ViewBag.ProjectList = new SelectList(_projectlist3, "ProjectGUID", "ProjectName");
                    ViewBag.Widgets = widgetRepo.GetCollections().Where(w => w.MinisterPermit == true).ToList();
                    break;
                case Emp_Title.PM:
                    var _projectlist5 = projectRepo.GetCollections().Where(n => n.InChargeDeptPMGUID == member.EmployeeGUID).Select(n => new { n.ProjectGUID, n.ProjectName });
                    ViewBag.ProjectList = new SelectList(_projectlist5, "ProjectGUID", "ProjectName");
                    ViewBag.Widgets = widgetRepo.GetCollections().Where(w => w.ProjectManagerPermit == true).ToList();
                    break;
                case Emp_Title.Director:
                    var _projectlist4 = projectRepo.GetCollections().Select(n => new { n.ProjectGUID, n.ProjectName });
                    ViewBag.ProjectList = new SelectList(_projectlist4, "ProjectGUID", "ProjectName");
                    ViewBag.Widgets = widgetRepo.GetCollections().Where(w => w.DirectorPermit == true).ToList();
                    break;
                default:
                    ViewBag.Widgets = null;
                    break;
            }


            return View(tasksInProgress.GetTasksGroupByStatus());
        }
        public ActionResult GetLeftDivPartialView()
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var titleID = jobTitleRepo.Find(new Guid(Request.Cookies["TitleGUID"].Value)).TitleID;
            string actionName = "";
            switch ((Emp_Title)Enum.Parse(typeof(Emp_Title), titleID.ToString()))
            {
                case Emp_Title.Admin:
                    actionName ="GetAllProjectsPV";
                    break;
                case Emp_Title.PG:
                    actionName = "GetMyTasksPV"; 
                    break;
                case Emp_Title.Minister:
                    actionName = "GetAllProjectsPV";
                    break;
                case Emp_Title.PM:
                    actionName = "GetMyProjectsPV"; 
                    break;
                case Emp_Title.Director:
                    actionName = "GetAllProjectsPV";
                    break;
                default:
                    actionName = "GetAllProjectsPV";
                    break;
            }
            return Json(actionName, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMyTasksPV()
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var tasksInProgress = taskRepo.GetCollections().Where(t => t.EmployeeGUID == member.EmployeeGUID)
                .Where(t => t.Project.ProjectStatusID == (int)ProjectsBL.Project_Status.InProgress);
            ViewBag.UserName = member.Employee.EmployeeName;
            ViewBag.TasksSum = tasksInProgress.Count();

            return PartialView(tasksInProgress.GetTasksGroupByStatus());
        }
        public ActionResult GetMyProjectsPV(Guid? projectGUID)
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var _tasks = taskRepo.GetCollections().Where(t => t.ProjectGUID == projectGUID);
            var datas = projectMemberRepo.GetCollections()
                .Where(m => m.ProjectGUID == projectGUID)
                .GetProjectMemberTaskCompletedRate(_tasks);

            return PartialView(datas.ToList());
        }
        public ActionResult GetAllProjectsPV()
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var datas = projectRepo.GetCollections().GetProjectsGroupedByPM(taskRepo.GetCollections());
            return PartialView(datas.ToList());
        }

        public ActionResult GetTitleCardsPV()
        {
            var member = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value));
            var titleID = jobTitleRepo.Find(new Guid(Request.Cookies["TitleGUID"].Value)).TitleID;
            if (titleID == (int)Emp_Title.Minister || titleID == (int)Emp_Title.Director)
            {
                string projectID_thisYear = "P" + DateTime.Now.Year.ToString().Substring(2, 2);
                var projects = projectRepo.GetCollections().Where(p=>p.ProjectID.StartsWith(projectID_thisYear));
                List<DisplayProjectStatusCountVM> statusCounts = new List<DisplayProjectStatusCountVM>() {
                    new DisplayProjectStatusCountVM()
                    {
                        ProjectStatusName = projectStatusRepo.GetCollections().Where(s=>s.ProjectStatusID == (int)ProjectsBL.Project_Status.InProgress).Select(s=>s.ProjectStatusName).First(),
                        Count = projects.Where(p=>p.ProjectStatusID == (int)ProjectsBL.Project_Status.InProgress).Count()
                    },                    
                    new DisplayProjectStatusCountVM()
                    {
                        ProjectStatusName = projectStatusRepo.GetCollections().Where(s=>s.ProjectStatusID == (int)ProjectsBL.Project_Status.WaitForConfirmed).Select(s=>s.ProjectStatusName).First(),
                        Count = projects.Where(p=>p.ProjectStatusID == (int)ProjectsBL.Project_Status.WaitForConfirmed).Count()
                    },
                    new DisplayProjectStatusCountVM()
                    {
                        ProjectStatusName = projectStatusRepo.GetCollections().Where(s=>s.ProjectStatusID == (int)ProjectsBL.Project_Status.Completed).Select(s=>s.ProjectStatusName).First(),
                        Count = projects.Where(p=>p.ProjectStatusID == (int)ProjectsBL.Project_Status.Completed).Count()
                    }
                };
                return PartialView("ProjectCardsPV", statusCounts);
            }
            else
            {
                var tasksInProgress = taskRepo.GetCollections().Where(t => t.EmployeeGUID == member.EmployeeGUID)
                        .Where(t => t.Project.ProjectStatusID == (int)ProjectsBL.Project_Status.InProgress);
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

                return PartialView("TaskCardsPV", statusCounts);
            }

        }
        [HttpPost]
        public ActionResult SetCookiesForMyBoard(Guid? projectGUID)
        {
            var userGUID = memberRepo.Find(new Guid(Request.Cookies["MemberGUID"].Value)).EmployeeGUID;
            Response.Cookies["PID"].Value = projectGUID.ToString();
            return Json(userGUID, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetGantt(Guid id)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var tasks = taskRepo.GetCollections().Where(n => n.ProjectGUID == id).GetLeafTasks().Select(n => 
            new { actualStart = (n.StartDate.Value.ToUniversalTime()- origin).TotalMilliseconds,
                actualEnd = (n.EndDate.Value.ToUniversalTime() - origin).TotalMilliseconds,
                name = n.TaskName}).OrderBy(n=>n.actualStart).ToList();
            return Json(tasks, JsonRequestBehavior.AllowGet);
        }

    }
}
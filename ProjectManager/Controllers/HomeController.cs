﻿using ProjectManager.Models;
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


        public ActionResult GetGantt(Guid id)
        {
            var tasks = taskRepo.GetCollections().Where(n => n.ProjectGUID == id).GetLeafTasks().Select(n=>new { n.StartDate,n.EndDate,n.TaskName}).ToList();
            return Json(tasks, JsonRequestBehavior.AllowGet);
        }

    }
}
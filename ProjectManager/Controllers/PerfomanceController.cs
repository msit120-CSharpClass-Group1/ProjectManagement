using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    public class PerfomanceController : Controller
    {
        Repository<Project> projectRepo = new Repository<Project>();
        Repository<ProjectMembers> ProjectMembersRepo = new Repository<ProjectMembers>();
        public ActionResult Index()
        {
            //ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
            //vm.ProjectGUID = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 1).Select(p => p.ProjectGUID);
            //vm.ProjectName = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 1).Select(p => p.ProjectName);
            //vm.InProgressProject = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 1).ToList();
            //vm.ClosedProject = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 2).Select(p => p.ProjectGUID);
            //vm.ClosedProjectStatusName = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 2).Select(p => p.ProjectStatus.ProjectStatusName);
            //vm.GroupMember = ProjectMembersRepo.GetCollections().GroupBy(p => p.Project.ProjectName).ToList();
            //vm.GetPMscore = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID != null).GetTeamPMAvgScore();
            //vm.GetMemberCount = ProjectMembersRepo.GetCollections().GetTeamMemberCount();
            ViewBag.InProgress = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 1).ToList();
            ViewBag.Closed = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 2).ToList();
            ViewBag.GroupMember = ProjectMembersRepo.GetCollections().GroupBy(p => p.Project.ProjectName).ToList();
            ViewBag.GetPMscore = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID != null).GetTeamPMAvgScore();
            ViewBag.GetMemberCount = ProjectMembersRepo.GetCollections().GetTeamMemberCount();     
            return View();
        }
        public ActionResult Score(Guid? projectGUID)
        {
            //test
            //ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
            //vm.GroupMemberCount = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == projectGUID).GroupBy(p => p.Employee.EmployeeName).Select (p=>p.Count());
            //return View(vm);
            ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
            vm.GroupMember = ProjectMembersRepo.GetCollections().GroupMembers();
            vm.GetMemberCount = ProjectMembersRepo.GetCollections().GetTeamMemberCount();
            return View(vm);
        }
        public ActionResult Watch()
        {
            return View();
        }

    }
}
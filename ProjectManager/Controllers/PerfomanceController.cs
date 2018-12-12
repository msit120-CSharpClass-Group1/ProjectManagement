using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class PerfomanceController : Controller
    {
        Repository<Project> projectRepo = new Repository<Project>();
        Repository<ProjectMembers> ProjectMembersRepo = new Repository<ProjectMembers>();
        Repository<Tasks> taskRepo = new Repository<Tasks>();
        Repository<Employee> empRepo = new Repository<Employee>();

        public ActionResult Index()
        {
            ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
            vm.InProgressProject = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 1).ToList();        
            vm.ClosedProject = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 2 ).ToList();
            vm.GetPMscore = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID != null).GetTeamPMAvgScore();
            vm.GetMemberCount = ProjectMembersRepo.GetCollections().GetTeamMemberCount();
            vm.GetTaskAVGScore = taskRepo.GetCollections().Where(p => p.Project.ProjectStatusID == 2).GetTaskAVGScore();
            return View(vm);
        }
        public ActionResult ScoreByMySelf(Guid? ProjectGUID)
        {
            return View();
        }
        public ActionResult ScoreByPM(Guid? ProjectGUID)
        {
            if (ProjectGUID != null)
            {
                ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
                vm.ProjectMembers = ProjectMembersRepo.GetCollections().Where(p => p.Project.ProjectStatusID == 1 && p.PMscore == null && p.ProjectGUID == ProjectGUID);
                return View(vm);
            }
            return RedirectToAction("Index", "Perfomance");
        }
        public ActionResult Watch(Guid? ProjectGUID)
        {
            if (ProjectGUID != null)
            {
                ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
                vm.TeamMember = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID);
                vm.GroupMembersScore = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GroupMembersScore();
                vm.GetHighestMember = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetHighestMember();
                vm.GetPMscore = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetTeamPMAvgScore();
                vm.GetMemberCount = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetTeamMemberCount();
                vm.GetAboveAVGMember = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetAboveAVGMember();
                vm.GetUnderAVGMember = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetUnderAVGMember(); 
                vm.GetLowestMember = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetLowestMember();
                vm.GetNoneScore = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetNoneScore();

                return View(vm);
            }
            return RedirectToAction("Index", "Perfomance");
        }
        public ActionResult TaskScoreAVGReview(Guid? ProjectGUID)
        {
            if (ProjectGUID != null)
            {
                ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
                vm.GroupMemberTaskScore = taskRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID && p.EmployeeGUID != null).GetLeafTasks()
                    .GroupBy(g => g.Employee.EmployeeName)
                    .Select(g => new Group<string, Tasks> { Key = g.Key, value = g, Avg = g.Average(p => p.ReviewScore) });             
                return View(vm);          
            }
            return RedirectToAction("Index", "Perfomance");
        }
    }
}
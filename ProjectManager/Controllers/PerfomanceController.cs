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
  
        public ActionResult Index()
        {
            ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
            vm.InProgressProject = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 1).ToList();        
            vm.ClosedProject = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 2 ).ToList();
            vm.GetPMscore = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID != null).GetTeamPMAvgScore();
            vm.GetMemberCount = ProjectMembersRepo.GetCollections().GetTeamMemberCount();                  
            return View(vm);
        }
        public ActionResult Score(Guid? ProjectGUID)
        {
            return View();
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
            ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
            vm.GroupMemberTaskScore = taskRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID && p.EmployeeGUID!=null)
                .GroupBy(g => g.Employee.EmployeeName)
                .Select(g => new Group<string, Tasks> { Key = g.Key,value=g,Avg = g.Average(p => p.ReviewScore) });            
            return View(vm);
        }
    }
}
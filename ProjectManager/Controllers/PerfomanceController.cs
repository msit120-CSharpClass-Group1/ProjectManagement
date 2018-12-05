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
            ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
            vm.InProgressProject = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 1).ToList();           
            vm.ClosedProject = projectRepo.GetCollections().Where(p => p.ProjectStatusID == 2).ToList();
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
                return View(vm);
            }
            return RedirectToAction("Index", "Perfomance");
        }

    }
}
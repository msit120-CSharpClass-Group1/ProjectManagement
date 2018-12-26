using Newtonsoft.Json;
using ProjectManager.Models;
using ProjectManager.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "工程師,專案經理,管理員")]
    public class PerfomanceController : Controller
    {
        Repository<Project> projectRepo = new Repository<Project>();
        Repository<ProjectMembers> ProjectMembersRepo = new Repository<ProjectMembers>();
        Repository<Tasks> taskRepo = new Repository<Tasks>();
        Repository<Employee> empRepo = new Repository<Employee>();
        Repository<Members> memberRepo = new Repository<Members>();
       
        public ActionResult Index()
        {
            var memberGUID = new Guid(Request.Cookies["MemberGUID"].Value);
            var character = memberRepo.GetCollections().Where(m => m.MemberGUID == memberGUID).Select(m => m.EmployeeGUID).FirstOrDefault();
            ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
            var collections = ProjectMembersRepo.GetCollections();
            vm.InProgressProject = collections.Where(p => p.EmployeeGUID == character && p.Project.ProjectStatusID == 1);
            vm.ClosedProject = collections.Where(p => p.EmployeeGUID == character && p.Project.ProjectStatusID == 2).ToList();
            vm.GetPMscore = collections.Where(p => p.ProjectGUID != null).GetTeamPMAvgScore();
            vm.GetMemberCount = collections.GetTeamMemberCount();
            vm.GetTaskAVGScore = taskRepo.GetCollections().Where(p => p.Project.ProjectStatusID == 2).GetTaskAVGScore();
            return View(vm);
        }
        public ActionResult ScoreByPM(Guid? ProjectGUID)
        {
            if (ProjectGUID != null)
            {
                ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
                vm.ProjectMembers = ProjectMembersRepo.GetCollections().Where(p => p.Project.ProjectStatusID == 1 && p.PMscore == null && p.ProjectGUID == ProjectGUID && p.Employee.JobTitle.TitleName!="專案經理");
                vm.ProjectGUIDShow = ProjectGUID;
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
                vm.GroupMembersScore = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID && p.Employee.JobTitle.TitleName!="專案經理").GroupMembersScore();
                vm.GetHighestMember = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetHighestMember();
                vm.GetPMscore = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetTeamPMAvgScore();
                vm.GetMemberCount = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetTeamMemberCount();
                vm.GetAboveAVGMember = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetAboveAVGMember();
                vm.GetUnderAVGMember = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetUnderAVGMember(); 
                vm.GetLowestMember = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID).GetLowestMember();
                vm.GetNoneScore = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID && p.Employee.JobTitle.TitleName != "專案經理").GetNoneScore();
                vm.ProjectGUIDShow = ProjectGUID;
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

        public ActionResult EditPMScore(ProjectMembers _projectMember)
        {
            var pm = ProjectMembersRepo.Find(_projectMember.EmployeeGUID, _projectMember.ProjectGUID);
            pm.PMscore = _projectMember.PMscore;
            pm.PMScoreDate = DateTime.Now;
            ProjectMembersRepo.Update(pm);
            return Content("success");
        }

        public ActionResult ChoseProject()
        {
            var memberGUID = new Guid(Request.Cookies["MemberGUID"].Value);
            var character = memberRepo.GetCollections().Where(m => m.MemberGUID == memberGUID).Select(m => m.EmployeeGUID).FirstOrDefault();
            ProjectMemberScoreVM vm = new ProjectMemberScoreVM();
            vm.ProjectMembers = ProjectMembersRepo.GetCollections().Where(p => p.EmployeeGUID == character && p.Project.ProjectStatusID ==1);
            vm.EmployeeGUID = character;
            var test = vm.ProjectMembers.Select(t => t.ProjectGUID).FirstOrDefault();
            //if ()
            //{
                return View(vm);
            //}
            //return RedirectToAction("Index", "Projects");
        }
        public ActionResult ScoreByMySelf(Guid? ProjectGUID)
        {
            ProjectMemberScoreVM vm = new ProjectMemberScoreVM();            
            return View();
        }
        public ActionResult EditSelfScore(ProjectMembers _projectMember)
        {
            var pm = ProjectMembersRepo.Find(_projectMember.EmployeeGUID, _projectMember.ProjectGUID);
            pm.Selfscore = _projectMember.Selfscore;
            pm.SelfScoreDate = DateTime.Now;
            ProjectMembersRepo.Update(pm);
            return Content("success");
        }

        public ActionResult OverAllReview()//總覽介面(含評分時間、誰評、是哪個專案、可篩選人)
        {
            return View();
        }

        #region ScoreChart
        public ActionResult WatchChart(ProjectMembers _projectMember)
        {
            ChartData<SingleColorChartDataset<int>> chartData = new ChartData<SingleColorChartDataset<int>>();

            chartData.labels = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == _projectMember.ProjectGUID && p.Employee.JobTitle.TitleName != "專案經理").Select(p => p.Employee.EmployeeName).ToList();

            chartData.datasets.Add(new SingleColorChartDataset<int>
            {
                label = "專案經理評分",
                backgroundColor = "rgba(91, 155, 213, 0.5)",
                borderColor = "rgba(91, 155, 213, 1)",
                data = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == _projectMember.ProjectGUID && p.Employee.JobTitle.TitleName != "專案經理" && p.PMscore != null).Select(p =>(int)p.PMscore).ToList()
            });
            return Content(JsonConvert.SerializeObject(chartData),"application/json");
        }

        public ActionResult WatchChartSelfScore(ProjectMembers _projectMember)
        {
            ChartData<SingleColorChartDataset<int>> chartData = new ChartData<SingleColorChartDataset<int>>();

            chartData.labels = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == _projectMember.ProjectGUID && p.Employee.JobTitle.TitleName != "專案經理").Select(p => p.Employee.EmployeeName).ToList();

            chartData.datasets.Add(new SingleColorChartDataset<int>
            {
                label = "自評分",
                backgroundColor = "rgba(91, 155, 213, 0.5)",
                borderColor = "rgba(91, 155, 213, 1)",
                data = ProjectMembersRepo.GetCollections().Where(p => p.ProjectGUID == _projectMember.ProjectGUID && p.Employee.JobTitle.TitleName != "專案經理" && p.Selfscore != null).Select(p => (int)p.Selfscore).ToList()
            });
            return Content(JsonConvert.SerializeObject(chartData), "application/json");
        }
        #endregion
    }
}
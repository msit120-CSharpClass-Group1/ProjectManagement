using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class SelectMBoardController : Controller
    {
        Repository<ProjectManager.Models.ProjectMembers> pm = new Repository<ProjectManager.Models.ProjectMembers>();
        Repository<ProjectManager.Models.Project> p = new Repository<ProjectManager.Models.Project>();
        Repository<ProjectManager.Models.Tasks> t = new Repository<Tasks>();
        // GET: SelectMBoard
        public ActionResult Index(Guid id)
        {
            List<SelectMBoardVM> model = new List<SelectMBoardVM>();
            foreach (var pm in pm.GetCollections().Where(x => x.ProjectGUID == id))
            {
                int total = pm.Project.Tasks.Where(t=>t.EmployeeGUID == pm.EmployeeGUID).Count();
                int completed = pm.Project.Tasks
                    .Where(t => t.EmployeeGUID == pm.EmployeeGUID)
                    .Where(t => t.TaskStatusID == (int)TasksBL.Task_Status.Completed).Count();
                int completedRate = (total == 0)? 0 : completed * 100 / total;

                model.Add(new SelectMBoardVM() {
                    EmployeeGuid = pm.EmployeeGUID,
                    EmployeeName = pm.Employee.EmployeeName,
                    InchargeTaskCount = total,
                    CompletedTaskCount = completed,
                    CompletedRate = completedRate
                });
            }

            Response.Cookies["PID"].Value = id.ToString();
            return View(model);
          
        }
    }
}
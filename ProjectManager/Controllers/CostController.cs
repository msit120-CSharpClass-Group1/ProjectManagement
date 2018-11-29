using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    public class CostController : Controller
    {
        Repository<TaskResource> ResourceRepo = new Repository<TaskResource>();
        Repository<ResourceCategory> ExpCatRepo = new Repository<ResourceCategory>();
        Repository<Department> DptRepo = new Repository<Department>();
        Repository<Project> ProjectRepo = new Repository<Project>();
        Repository<Tasks> TaskRepo = new Repository<Tasks>();
        ProjectManagementEntities dbContext = new ProjectManagementEntities();

        // GET: Cost
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExpList()
        {
            var Departments = DptRepo.GetCollections();

            var q = (from d in dbContext.Department
                    join p in dbContext.Project on d.DepartmentGUID equals p.RequiredDeptGUID
                    select d).Distinct();

            ViewBag.Departments = q.ToList();

            ViewBag.ExpCats = new SelectList(ExpCatRepo.GetCollections(), "CategoryID", "CategoryName");

            return View();
        }

        public ActionResult GetProjectListByDptID(Guid DepartmentID)
        {
            List<Project> ProjectList = ProjectRepo.GetCollections().Where(p => p.RequiredDeptGUID == DepartmentID).Select(p => new Project { ProjectGUID = p.ProjectGUID, ProjectID = p.ProjectID, ProjectName = p.ProjectName }).ToList();

            return Json(ProjectList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTaskListByProjectGuid(Guid ProjectGuid)
        {
            List<Tasks> TaskList = TaskRepo.GetCollections().Where(t => t.ProjectGUID == ProjectGuid).Select(t => new Tasks { TaskGUID = t.TaskGUID, TaskName = t.TaskName }).ToList();

            return Json(TaskList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTaskResources(Guid id)
        {
            var q = from p in dbContext.Project
                    join t in dbContext.Tasks on p.ProjectGUID equals t.ProjectGUID
                    join tr in dbContext.TaskResource on t.TaskGUID equals tr.TaskGUID
                    join c in dbContext.ResourceCategory on tr.CategoryID equals c.CategoryID
                    where p.ProjectGUID == id
                    select new DisplayResource { ProjectGUID = p.ProjectGUID, ProjectName = p.ProjectName, TaskGUID = t.TaskGUID, TaskName = t.TaskName, ResourceGUID = tr.ResourceGUID, ResourceID = tr.ResourceID, ResourceName = tr.ResourceName, CategoryID = c.CategoryID, CategoryName = c.CategoryName, Quantity = tr.Quantity, Unit = tr.Unit, UnitPrice = tr.UnitPrice, SubTotal = (tr.UnitPrice * tr.Quantity), Date = tr.Date, Description = tr.Description };

            var DisplayList = q.ToList();

            var Departments = DptRepo.GetCollections();

            return PartialView(DisplayList);
        }

        public ActionResult AddTaskResource(TaskResource resource)
        {
            resource.ResourceGUID = Guid.NewGuid();
            ResourceRepo.Add(resource);

            return RedirectToAction("ExpList");
        }

        public ActionResult UpdateTaskResource(TaskResource resource)
        {
            ResourceRepo.Update(resource);

            return RedirectToAction("ExpList");
        }

        public ActionResult DeleteTaskResource(Guid id)
        {            
            ResourceRepo.Delete(ResourceRepo.Find(id));
            return RedirectToAction("ExpList");
        }


        public ActionResult ExpCatMgr()
        {
            return View(ExpCatRepo.GetCollections());
        }

        public ActionResult AddCat(ResourceCategory cat)
        {
            ExpCatRepo.Add(cat);
            return RedirectToAction("ExpCatMgr");
        }

        public ActionResult UpdateCat(ResourceCategory cat)
        {
            ExpCatRepo.Update(cat);
            return RedirectToAction("ExpCatMgr");
        }

        public ActionResult DeleteCat(int? id)
        {
            ExpCatRepo.Delete(ExpCatRepo.Find(id));
            return RedirectToAction("ExpCatMgr");
        }
    }
}
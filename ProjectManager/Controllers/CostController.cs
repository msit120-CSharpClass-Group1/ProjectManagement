using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using PagedList;
using PagedList.Mvc;

namespace ProjectManager.Controllers
{
    public class CostController : Controller
    {
        Repository<ResourceCategory> ResourceCatRepo = new Repository<ResourceCategory>();
        Repository<TaskResource> ResourceRepo = new Repository<TaskResource>();
        Repository<Department> DptRepo = new Repository<Department>();
        Repository<Project> ProjectRepo = new Repository<Project>();
        Repository<Tasks> TaskRepo = new Repository<Tasks>();

        // GET: Cost
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExpList()
        {
            var Departments = DptRepo.GetCollections();

            var q = (from d in DptRepo.GetCollections()
                     join p in ProjectRepo.GetCollections() on d.DepartmentGUID equals p.RequiredDeptGUID
                     select d).Distinct();

            ViewBag.Departments = q.ToList();

            ViewBag.ExpCats = new SelectList(ResourceCatRepo.GetCollections(), "CategoryID", "CategoryName");

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

        public ActionResult GetTaskResources(Guid id,int? page)
        {
            var q = from p in ProjectRepo.GetCollections()
                    join t in TaskRepo.GetCollections() on p.ProjectGUID equals t.ProjectGUID
                    join tr in ResourceRepo.GetCollections() on t.TaskGUID equals tr.TaskGUID
                    join c in ResourceCatRepo.GetCollections() on tr.CategoryID equals c.CategoryID
                    where p.ProjectGUID == id
                    select new ProjectResourceVM
                    {
                        ProjectGUID = p.ProjectGUID,
                        ProjectName = p.ProjectName,
                        TaskGUID = t.TaskGUID,
                        TaskName = t.TaskName,
                        ResourceGUID = tr.ResourceGUID,
                        ResourceID = tr.ResourceID,
                        ResourceName = tr.ResourceName,
                        CategoryID = c.CategoryID,
                        CategoryName = c.CategoryName,
                        Quantity = tr.Quantity,
                        Unit = tr.Unit,
                        UnitPrice = tr.UnitPrice,
                        SubTotal = (tr.UnitPrice * tr.Quantity),
                        Date = tr.Date,
                        Description = tr.Description
                    };

            var ProjectResourceList = q.ToList();

            ViewBag.Count = ProjectResourceList.Count;

            return PartialView(ProjectResourceList.ToPagedList(page ?? 1, 10));
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
            return View(ResourceCatRepo.GetCollections());
        }

        public ActionResult AddCat(ResourceCategory cat)
        {
            ResourceCatRepo.Add(cat);
            return RedirectToAction("ExpCatMgr");
        }

        public ActionResult UpdateCat(ResourceCategory cat)
        {
            ResourceCatRepo.Update(cat);
            return RedirectToAction("ExpCatMgr");
        }

        public ActionResult DeleteCat(int? id)
        {
            ResourceCatRepo.Delete(ResourceCatRepo.Find(id));
            return RedirectToAction("ExpCatMgr");
        }
    }
}
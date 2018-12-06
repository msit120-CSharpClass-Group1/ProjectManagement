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

        public ActionResult GetTaskResources(Guid id,int? page,string sortBy)
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

            var ProjectResourceList = q.ToList().AsQueryable();

            ViewBag.Count = ProjectResourceList.Count();
            ViewBag.sortByDate = string.IsNullOrEmpty(sortBy) ? "DateDesc" : "";
            ViewBag.sortByTaskName = sortBy == "TaskName" ? "TaskNameDesc" : "TaskName";
            ViewBag.sortByResourceName = sortBy == "ResourceName" ? "ResourceNameDesc" : "ResourceName";
            ViewBag.sortByResourceCat = sortBy == "ResourceCat" ? "ResourceCatDesc" : "ResourceCat";
            ViewBag.sortByQuantity = sortBy == "Quantity" ? "QuantityDesc" : "Quantity";
            ViewBag.sortByUnitPrice = sortBy == "UnitPrice" ? "UnitPriceDesc" : "UnitPrice";
            ViewBag.sortBySubtotal = sortBy == "SubTotal" ? "SubTotalDesc" : "SubTotal";

            switch (sortBy)
            {
                case "DateDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.Date);
                    break;
                case "TaskName":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.TaskName);
                    break;
                case "TaskNameDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.TaskName);
                    break;
                case "ResourceName":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.ResourceName);
                    break;
                case "ResourceNameDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.ResourceName);
                    break;
                case "ResourceCat":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.CategoryID);
                    break;
                case "ResourceCatDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.CategoryID);
                    break;
                case "Quantity":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.Quantity);
                    break;
                case "QuantityDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.Quantity);
                    break;
                case "UnitPrice":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.UnitPrice);
                    break;
                case "UnitPriceDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.UnitPrice);
                    break;
                case "SubTotal":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.SubTotal);
                    break;
                case "SubTotalDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.SubTotal);
                    break;
                default:
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.Date);
                    break;
            }

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
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
        Repository<ResourceCategory> rc = new Repository<ResourceCategory>();
        Repository<Department> D = new Repository<Department>();
        Repository<Project> Project = new Repository<Project>();
        ProjectManagementEntities dbContext = new ProjectManagementEntities();

        // GET: Cost
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ExpList()
        {
            var Departments = D.GetCollections();

            var q = (from d in dbContext.Department
                    join p in dbContext.Project on d.DepartmentGUID equals p.RequiredDeptGUID
                    select d).Distinct();

            ViewBag.Departments = q.ToList();
            return View();
        }

        public ActionResult ExpList(Guid ProjectGUID)
        {
            var q = from p in dbContext.Project
                    join t in dbContext.Tasks on p.ProjectGUID equals t.ProjectGUID
                    join tr in dbContext.TaskResource on t.TaskGUID equals tr.TaskGUID
                    join c in dbContext.ResourceCategory on tr.CategoryID equals c.CategoryID
                    where p.ProjectGUID == ProjectGUID
                    select new DisplayResource { ProjectGUID = p.ProjectGUID, ProjectName = p.ProjectName, TaskGUID = t.TaskGUID, TaskName = t.TaskName, ResourceGUID = tr.ResourceGUID, ResourceID = tr.ResourceID, ResourceName = tr.ResourceName, CategoryID = c.CategoryID, CategoryName = c.CategoryName, Quantity = tr.Quantity, Unit = tr.Unit, UnitPrice = tr.UnitPrice, SubTotal = (tr.UnitPrice * tr.Quantity), Date = DateTime.Now, Description = tr.Description };

            var DisplayList = q.ToList();

            ViewBag.Departments = D.GetCollections();
            return View(DisplayList);
        }

        public JsonResult GetProjectList(Guid DepartmentID)
        {
            var Projects = Project.GetCollections();

            List<Project> ProjectList = Project.GetCollections().Where(p => p.RequiredDeptGUID == DepartmentID).Select(p => new Project { ProjectID = p.ProjectID, ProjectName = p.ProjectName }).ToList();

            return Json(ProjectList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExpCatMgr()
        {
            return View(rc.GetCollections());
        }

        public ActionResult AddCat(ResourceCategory cat)
        {
            rc.Add(cat);
            //return RedirectToAction("ExpCatMgr");
            return RedirectToAction("ExpCatMgr");
        }

        public ActionResult Update(ResourceCategory cat)
        {
            rc.Update(cat);
            return RedirectToAction("ExpCatMgr");
        }

        public ActionResult Delete(int? id)
        {
            rc.Delete(rc.Find(id));
            return RedirectToAction("ExpCatMgr");
        }
    }
}
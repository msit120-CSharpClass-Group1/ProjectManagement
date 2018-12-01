using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員")]
    public class DepartmentController : Controller
    {
        private IRepository<Department> depRepository = new Repository<Department>();
        Department db = new Department();

        public ActionResult Index()
        {
            return View(depRepository.GetCollections().OrderBy(c=>c.DepartmentID));
        }

        public ActionResult Insert()
        {
            if (Request.Form.Count > 0)
            {
                Department _department = new Department();
                if (!(Request.Form["ParentDepartmentGUID"] == null || Request.Form["ParentDepartmentGUID"] == "0"))
                {
                    _department.ParentDepartmentGUID = new Guid(Request.Form["ParentDepartmentGUID"]);
                }
                _department.DepartmentGUID = Guid.NewGuid();
                _department.DepartmentName = Request.Form["DepartmentName"];
                depRepository.Add(_department);
                return RedirectToAction("Index");
            }
            return View(depRepository.GetCollections());
        }


        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.department = depRepository.Find(new Guid(id));
            return View(depRepository.GetCollections());
        }


        [HttpPost]
        public ActionResult Edit(Department department)
        {

            depRepository.Update(department);

            return RedirectToAction("Index");
        }


        public ActionResult Delete(Guid? id)
        {

            Department department = depRepository.Find(id);
            var childDeps = depRepository.GetCollections().Where(n => n.ParentDepartmentGUID == id).ToList();
            foreach (var childDep in childDeps)
            {
                childDep.ParentDepartmentGUID = department.ParentDepartmentGUID;
                depRepository.Update(childDep);
            }

            depRepository.Delete(department);

            return RedirectToAction("Index");
        }
    }
}
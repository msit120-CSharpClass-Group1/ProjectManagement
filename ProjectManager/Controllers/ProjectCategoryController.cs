using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員")]
    public class ProjectCategoryController : Controller
    {
        private IRepository<ProjectCategory> projectCategoryRepository = new Repository<ProjectCategory>();
        public ActionResult Index()
        {
            return View(projectCategoryRepository.GetCollections());
        }

        public ActionResult Insert(ProjectCategory projectCategory)
        {
            if(Request.Form.Count>0)
            {
                projectCategoryRepository.Add(projectCategory);
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(projectCategoryRepository.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(ProjectCategory projectCategory)
        {
            ProjectCategory _projectCategory = projectCategoryRepository.Find(projectCategory.ProjectCategoryID);
            _projectCategory.ProjectCategoryName = projectCategory.ProjectCategoryName;

            projectCategoryRepository.Update(_projectCategory);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            ProjectCategory _projectCategory = projectCategoryRepository.Find(id);
            projectCategoryRepository.Delete(_projectCategory);

            return RedirectToAction("Index");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    public class ProjectStatusController : Controller
    {
        private IRepository<ProjectStatus> ProStatusRepository = new Repository<ProjectStatus>();


        public ActionResult Index()
        {
            return View(ProStatusRepository.GetCollections());
        }
        
        public ActionResult Insert(ProjectStatus _projectStatus)
        {
            if(Request.Form.Count>0)
            {
                ProStatusRepository.Add(_projectStatus);
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(ProStatusRepository.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(ProjectStatus projectStatus)
        {

            ProjectStatus _projectStatus = ProStatusRepository.Find(projectStatus.ProjectStatusID);
            _projectStatus.ProjectStatusName = projectStatus.ProjectStatusName;

            ProStatusRepository.Update(_projectStatus);

            return RedirectToAction("Index");
        }


    public ActionResult Delete(int id)
        {
            ProjectStatus _projectStatus = ProStatusRepository.Find(id);
            ProStatusRepository.Delete(_projectStatus);
            return RedirectToAction("Index");

        }


    }
}
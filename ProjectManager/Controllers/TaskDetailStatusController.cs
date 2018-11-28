using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    public class TaskDetailStatusController : Controller
    {
        private IRepository<TaskDetailStatus> TDstatusRepository  = new Repository<TaskDetailStatus>();

        public ActionResult Index()
        {
            return View(TDstatusRepository.GetCollections());
        }

        public ActionResult Insert(TaskDetailStatus taskDetailStatus)
        {
            if(Request.Form.Count>0)
            {
                TDstatusRepository.Add(taskDetailStatus);
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(TDstatusRepository.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(TaskDetailStatus taskDetailStatus)
        {
            TaskDetailStatus _taskDetailStatus = TDstatusRepository.Find(taskDetailStatus.TaskDetailStatusID);
            _taskDetailStatus.TaskDetailStatusName = Request.Form["TaskDetailStatusName"];

            TDstatusRepository.Update(_taskDetailStatus);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            TaskDetailStatus _taskDetailStatus = TDstatusRepository.Find(id);
            TDstatusRepository.Delete(_taskDetailStatus);

            return RedirectToAction("Index");
        }
    }
}
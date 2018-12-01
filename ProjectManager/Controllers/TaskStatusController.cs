using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員")]
    public class TaskStatusController : Controller
    {
        private IRepository<TaskStatus> taskStatusRepository = new Repository<TaskStatus>();

        public ActionResult Index()
        {
            return View(taskStatusRepository.GetCollections());
        }

        public ActionResult Insert(TaskStatus taskStatus)
        {
            if(Request.Form.Count>0)
            {
                taskStatusRepository.Add(taskStatus);

                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(taskStatusRepository.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(TaskStatus taskStatus)
        {
            TaskStatus _taskStatus = taskStatusRepository.Find(taskStatus.TaskStatusID);
            _taskStatus.TaskStatusName = taskStatus.TaskStatusName;

            taskStatusRepository.Update(_taskStatus);

            return RedirectToAction("Index");

        }

        public ActionResult Delete(int id)
        {
            TaskStatus _taskStatus = taskStatusRepository.Find(id);
            taskStatusRepository.Delete(_taskStatus);

            return RedirectToAction("Index");
        }

    }
}
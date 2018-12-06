using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private IRepository<Tasks> tasksRepository = new Repository<Tasks>();
        public ActionResult Index()
        {
            return View(tasksRepository.GetCollections().OrderBy(p=>p.TaskID));
        }

        public ActionResult Insert(Tasks tasks)
        {
            if(Request.Form.Count>0)
            {
                Tasks _tasks = new Tasks();
                if (!(Request.Form["ParentTaskGUID"] == null || Request.Form["ParentTaskGUID"] == "0"))
                {
                    _tasks.ParentTaskGUID = new Guid(Request.Form["ParentTaskGUID"]);
                }
                _tasks.TaskGUID = Guid.NewGuid();
                _tasks.TaskName = tasks.TaskName;
                _tasks.EstStartDate = (DateTime)tasks.EstEndDate;
                _tasks.EstEndDate = (DateTime)tasks.EstEndDate;
                _tasks.StartDate = (DateTime)tasks.StartDate;
                _tasks.EndDate = (DateTime)tasks.EndDate;
                _tasks.EstWorkTime = tasks.EstWorkTime;
                _tasks.WorkTime = tasks.WorkTime;
                _tasks.Tag = tasks.Tag;
                _tasks.Description = tasks.Description;               
                tasksRepository.Add(_tasks);
                return RedirectToAction("Index");
            }

            return View(tasksRepository.GetCollections());
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.Task = tasksRepository.Find(new Guid(id));
            return View(tasksRepository.GetCollections());
        }

        [HttpPost]
        public ActionResult Edit(Tasks tasks)
        {
            //Tasks _tasks = new Tasks();
            //_tasks.TaskName = tasks.TaskName;
            //_tasks.EstStartDate = (DateTime)tasks.EstEndDate;
            //_tasks.EstEndDate = (DateTime)tasks.EstEndDate;
            //_tasks.StartDate = (DateTime)tasks.StartDate;
            //_tasks.EndDate = (DateTime)tasks.EndDate;
            //_tasks.EstWorkTime = tasks.EstWorkTime;
            //_tasks.WorkTime = tasks.WorkTime;
            //_tasks.Tag = tasks.Tag;
            //_tasks.Description = tasks.Description;
            tasksRepository.Update(tasks);
            return RedirectToAction("Index");
        }
    }
}
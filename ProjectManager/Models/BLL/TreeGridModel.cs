using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class TreeGridModel
    {
        public TreeGridModel()
        {
            SortedTasks = new List<Tasks>();
            ChildTasks = new List<Tasks>();
            ChildLeafTasks = new List<Tasks>();
        }
        
        public TreeGridModel(List<Tasks> tasksFromRepo)
        {
            SortedTasks = new List<Tasks>();
            ChildTasks = new List<Tasks>();
            ChildLeafTasks = new List<Tasks>();
            TasksFromRepo = tasksFromRepo;
        }
        private List<Tasks> tasksFromRepo;
        /// <summary>
        /// 回傳帶有orderID、parentOrderID排序完成的List&lt;Task>
        /// </summary>
        public List<Tasks> SortedTasks { get; set; }
        public List<Tasks> ChildTasks { get; set; }
        public List<Tasks> ChildLeafTasks { get; set; }
        public List<Tasks> TasksFromRepo
        { 
            set
            {
                tasksFromRepo = value;
                var q = tasksFromRepo.Where(w => w.ParentTaskGUID == null);
                foreach (var rootTask in q)
                {
                    SortedTasks.Add(rootTask);
                    GetChildren(rootTask);
                }
                GetOrderIDParentOrderID();
            }
        }
        public void GetChildren(Tasks parentTask)
        {
            var q = tasksFromRepo.Where(w => w.ParentTaskGUID == parentTask.TaskGUID);
            foreach (var task in q)
            {
                SortedTasks.Add(task);
                GetChildren(task);
            }
        }
        public void GetChildren(Tasks parentTask,List<Tasks> tasksFromRepo)
        {
            var q = tasksFromRepo.Where(w => w.ParentTaskGUID == parentTask.TaskGUID);
            foreach (var task in q)
            {
                ChildTasks.Add(task);
                GetChildren(task,tasksFromRepo);
            }
        }
        public void GetOrderIDParentOrderID()
        {
            for (int i = 0; i < SortedTasks.Count; i++)
            {
                SortedTasks[i].orderID = i + 1;
                if (SortedTasks[i].ParentTaskGUID != null)
                {
                    var parentWork = SortedTasks.Where(t => t.TaskGUID == SortedTasks[i].ParentTaskGUID).FirstOrDefault();
                    SortedTasks[i].parentOrderID = parentWork.orderID;
                }
            }

        }
        public void GetChildLeafTasks(Tasks rootTask)
        {            
            var NotLeafTasks = tasksFromRepo.Where(t => t.ParentTaskGUID != null)
                        .Select(t => t.ParentTaskGUID).Distinct().ToList();
            var q = tasksFromRepo.Where(t => t.ParentTaskGUID == rootTask.TaskGUID);
            foreach (var task in q)
            {
                if (!NotLeafTasks.Where(id=>id == task.TaskGUID).Any())
                {
                    ChildLeafTasks.Add(task);
                }
                GetChildLeafTasks(task);
            }            
        }
    }
}
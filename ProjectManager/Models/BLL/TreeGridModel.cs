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

        }
        
        public TreeGridModel(List<Tasks> tasksFromRepo)
        {
            SortedTasks = new List<Tasks>();
            TasksFromRepo = tasksFromRepo;
        }
        private List<Tasks> tasksFromRepo;
        /// <summary>
        /// 回傳帶有orderID、parentOrderID排序完成的List&lt;Task>
        /// </summary>
        public List<Tasks> SortedTasks { get; set; }
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
    }
}
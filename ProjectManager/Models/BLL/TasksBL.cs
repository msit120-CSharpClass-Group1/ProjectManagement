using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public static class TasksBL
    {
        /// <summary>
        /// 回傳帶有orderID、parentOrderID排序完成的List&lt;Task>
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static IEnumerable<Tasks> GetSortedTasks(this IEnumerable<Tasks> tasks)
        {
            TreeGridModel treeGrid = new TreeGridModel(tasks.ToList());
            return treeGrid.SortedTasks;
        }
        
        public static IEnumerable<Tasks> GetLeafTasks(this IEnumerable<Tasks> tasks)
        {
            var parentTasks = tasks.Where(t=>t.ParentTaskGUID != null)
                .Select(t => t.ParentTaskGUID).Distinct().ToList();

            List<Tasks> leafTasks = new List<Tasks>();
            foreach (var item in tasks.Where(t => t.ParentTaskGUID != null))
            {
                if(!parentTasks.Where(parent=> parent == item.ProjectGUID).Any())
                {
                    leafTasks.Add(item);
                }
            }
            return leafTasks;
        }
        public static IEnumerable<Tasks> GetAllChildTasks(this Tasks task)
        {            
            TreeGridModel model = new TreeGridModel();
            model.GetChildren(task, new Repository<Tasks>().GetCollections().ToList());
            return model.ChildTasks;
        }
        public static bool IsAnyResource(this IEnumerable<Tasks> tasks)
        {
            Repository<TaskResource> resourceRepo = new Repository<TaskResource>();
            foreach (var task in tasks)
            {
                if (resourceRepo.GetCollections().Where(r => r.TaskGUID == task.TaskGUID).Any())
                    return true;
            }
            return false;
        }
    }
}
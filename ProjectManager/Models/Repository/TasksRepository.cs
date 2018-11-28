using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class TasksRepository:Repository<Tasks>
    {
        public IEnumerable<Tasks> ShowSortedTasks(Guid? ProjectGUID)
        {  
            var tasks = base.dbset.OrderBy(t=>t.TaskID).Where(t => t.ProjectGUID == ProjectGUID).ToList();
            TreeGridModel treeGrid = new TreeGridModel(tasks);            
            return treeGrid.SortedTasks;
        }
    }
}
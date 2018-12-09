using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class TasksGroupByStatus
    {
        public int? TaskStatusID { get; set; }
        public IGrouping<int?,Tasks> group { get; set; }
    }
}
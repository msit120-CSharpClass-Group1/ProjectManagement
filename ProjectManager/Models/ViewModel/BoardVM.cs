using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class BoardVM
    {
        public IEnumerable<Project> Project { get; set; }
        public IEnumerable<TaskStatus> TaskStatus { get; set; }
        public IEnumerable<Tasks> Tasks { get; set; }
        public Tasks Task { get; set; }
        public IEnumerable<TaskDetail> TaskDetail { get; set; }
        public TaskDetail TaskDetails { get; set; }
    }
}
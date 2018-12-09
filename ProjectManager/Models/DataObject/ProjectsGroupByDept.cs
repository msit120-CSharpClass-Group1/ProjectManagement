using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class ProjectsGroupByDept
    {
        public string DepartmentName { get; set; }
        public IGrouping<string, Project> group { get; set; }
    }
}
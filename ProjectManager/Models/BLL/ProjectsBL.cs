using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public static class ProjectsBL
    {
        public static int GetLastProjectIntID(this IEnumerable<Project> projects)
        {
            var q = projects.Select(n => n.ProjectID);
            List<int> ProjectIntID = new List<int>();
            foreach (var x in q)
            {
                ProjectIntID.Add(int.Parse(x.Substring(1, 5)));
            }
            return ProjectIntID.Max();
        }
    }
}
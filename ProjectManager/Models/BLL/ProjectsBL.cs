using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public static class ProjectsBL
    {
        /// <summary>
        /// 回傳去掉開頭P的最後一個專案編號
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 回傳List&lt;GroupedProject>，將傳入值GroupBy(requiredDeptName)
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public static IEnumerable<GroupedProject> GetGroupedProject(this IEnumerable<Project> projects)
        {
            return projects.AsEnumerable()
                .OrderBy(p => p.ProjectID)
                .GroupBy(p => p.Department.DepartmentName)
                .Select(g => new GroupedProject() { DepartmentName = g.Key, group = g }).ToList();
        }
    }
}
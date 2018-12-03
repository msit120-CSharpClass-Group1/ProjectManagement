using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class ProjectsRepository : Repository<Project>
    {
        public ProjectsRepository() : base()
        {

        }
        /// <summary>
        /// 回傳List&lt;GroupedProject>，將傳入值GroupBy(requiredDeptName)
        /// </summary>
        /// <returns></returns>        
        public static IEnumerable<GroupedProject> ShowGroupedProject(IEnumerable<Project> projects)
        {
            return projects.AsEnumerable()
                .OrderBy(p => p.ProjectID)
                .GroupBy(p => p.Department.DepartmentName)
                .Select(g => new GroupedProject() { DepartmentName = g.Key, group = g }).ToList();
        }

    }
}
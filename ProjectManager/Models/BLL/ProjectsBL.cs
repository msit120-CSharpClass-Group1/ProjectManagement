using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ProjectManager.Models.TasksBL;

namespace ProjectManager.Models
{
    public static class ProjectsBL
    {
        public enum Project_Status
        {
            InProgress=1,
            WaitForConfirmed,
            Completed,
        }
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
        public static void LoadProjectsCompletedRate(this IEnumerable<Project> projects, IEnumerable<Tasks> taskFromRepo)
        {
            foreach (var project in projects)
            {
                var leafTasks = taskFromRepo.Where(t => t.ProjectGUID == project.ProjectGUID).GetLeafTasks();
                int completedLeafTaskCount = 0;
                int totalLeafCount = leafTasks.Count();
                foreach (var leaf in leafTasks)
                {
                    string status = (leaf.TaskStatusID ?? default(int)).ToString();
                    Task_Status leaf_Status = (Task_Status)Enum.Parse(typeof(Task_Status), status);
                    if (leaf_Status == Task_Status.Completed)
                    {
                        completedLeafTaskCount++;
                    }
                }
                int rate = 0;
                if (totalLeafCount == 0)
                {
                    string status = (project.ProjectStatusID ?? default(int)).ToString();
                    Project_Status project_Status = (Project_Status)Enum.Parse(typeof(Project_Status), status);
                    if (project_Status == Project_Status.Completed)
                        rate = 100;
                }
                else
                {
                    rate = completedLeafTaskCount * 100 / totalLeafCount;
                }
                project.CompletedRate = rate;
            }
            
        }
        public static IEnumerable<int> GetWorkTimeSumOfProjectMembers(this IEnumerable<ProjectMembers> projectMembers, IEnumerable<Tasks> tasksFromRepo)
        {
            List<int> workTimeSums = new List<int>();
            var leafTasks = tasksFromRepo.GetLeafTasks();
            foreach (var member in projectMembers)
            {
               int _sum = (int)leafTasks.Where(t => t.EmployeeGUID == member.EmployeeGUID).Select(t => t.EstWorkTime).Sum();
                workTimeSums.Add(_sum);
            }
            return workTimeSums;
        }
    }
}
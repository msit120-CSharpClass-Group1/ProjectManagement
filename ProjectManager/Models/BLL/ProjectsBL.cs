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
        public static IEnumerable<Grouped<string,Project>> GetGroupedProject(this IEnumerable<Project> projects)
        {
            return projects.AsEnumerable()
                .OrderBy(p => p.ProjectID)
                .GroupBy(p => p.Department.DepartmentName)
                .Select(g => new Grouped<string,Project>() { Key = g.Key, group = g }).ToList();
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
        public static void LoadProjectSaveTimeRate(this IEnumerable<Project> projects, IEnumerable<Tasks> taskFromRepo)
        {
            foreach (var project in projects)
            {
                var completedleafTasks = taskFromRepo.Where(t => t.ProjectGUID == project.ProjectGUID).GetLeafTasks()
                    .Where(t=>t.TaskStatusID == (int)Task_Status.Completed);
                int leafTaskEstDurationSum = completedleafTasks.Sum(t => (int)t.EstWorkTime);
                int leafTaskDurationSum = completedleafTasks.Sum(t => (int)t.WorkTime);
                var rate = 0;
                if (leafTaskEstDurationSum != 0)
                    rate = (leafTaskEstDurationSum - leafTaskDurationSum) * 100 / leafTaskEstDurationSum ;

                project.DurationSavedRate = rate;
            }

        }
    }
}
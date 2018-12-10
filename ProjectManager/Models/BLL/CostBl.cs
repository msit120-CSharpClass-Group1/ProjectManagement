using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManager.Models;

namespace ProjectManager.Models
{
    public static class CostBL
    {
        public static IEnumerable<int> GetSubtotalByDepartment(this IEnumerable<Department> departments)
        {
            List<int> result = new List<int>();

            foreach(var department in departments)
            {
                int sum = 0;

                foreach (var project in department.Project)
                {
                    foreach(var task in project.Tasks.GetLeafTasks())
                    {
                        sum += (int)task.TaskResource.Select(r => r.UnitPrice * r.Quantity).Sum();
                    }
                }
                result.Add(sum);
            }

            return result;
        }

        public static IEnumerable<double> GetOverallRates(this IEnumerable<Project> projects)
        {
            List<double> result = new List<double>();

            int CountOfAllLeafTasks = 0;
            int CountOfCompletedLeafTasks = 0;
            int BudgetOfAllProjects = 0;
            int CostOfAllProjects = 0;

            foreach (var project in projects)
            {
                CountOfAllLeafTasks += project.Tasks.GetLeafTasks().Count();
                CountOfCompletedLeafTasks += project.Tasks.GetLeafTasks().Where(t => t.TaskStatusID == 4).Count();
                BudgetOfAllProjects += project.ProjectBudget != null ? (int)project.ProjectBudget : 0;
                foreach(var task in project.Tasks)
                {
                    CostOfAllProjects += (int)task.TaskResource.Select(r => r.UnitPrice * r.Quantity).Sum();
                }
            }

            double CompletionRate = Math.Round(CountOfCompletedLeafTasks * 100D / CountOfAllLeafTasks, 2, MidpointRounding.AwayFromZero);
            double BudgetUsedRate = Math.Round(CostOfAllProjects * 100D / BudgetOfAllProjects, 2, MidpointRounding.AwayFromZero);

            result.Add(CompletionRate);
            result.Add(BudgetUsedRate);

            return result;
        }

        public static IEnumerable<int> GetSubtotalByCat(this IEnumerable<ResourceCategory> cats)
        {
            List<int> result = new List<int>();

            foreach(var cat in cats)
            {
               int sum = (int)cat.TaskResource.Select(r => r.UnitPrice * r.Quantity).Sum();
                result.Add(sum);
            }

            return result;
        }

        public static IEnumerable<int> CountTasksByStatus(this IEnumerable<TaskStatus> statuses)
        {
            List<int> result = new List<int>();

            foreach(var status in statuses)
            {
                int count = status.Tasks.GetLeafTasks().Count();
                result.Add(count);
            }

            return result;
        }
    }
}
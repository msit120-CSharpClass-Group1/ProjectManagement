using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ProjectManager.Models;

namespace ProjectManager.Models
{
    public static class CostBL
    {
        public static IQueryable<ProjectResourceVM> Sort(this IQueryable<ProjectResourceVM> ProjectResourceList,string sortBy)
        {
            switch (sortBy)
            {
                case "DateDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.Date);
                    break;
                case "ProjectName":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.ProjectName);
                    break;
                case "ProjectNameDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.ProjectName);
                    break;
                case "TaskName":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.TaskName);
                    break;
                case "TaskNameDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.TaskName);
                    break;
                case "ResourceName":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.ResourceName);
                    break;
                case "ResourceNameDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.ResourceName);
                    break;
                case "ResourceCat":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.CategoryID);
                    break;
                case "ResourceCatDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.CategoryID);
                    break;
                case "Quantity":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.Quantity);
                    break;
                case "QuantityDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.Quantity);
                    break;
                case "UnitPrice":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.UnitPrice);
                    break;
                case "UnitPriceDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.UnitPrice);
                    break;
                case "SubTotal":
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.SubTotal);
                    break;
                case "SubTotalDesc":
                    ProjectResourceList = ProjectResourceList.OrderByDescending(r => r.SubTotal);
                    break;
                default:
                    ProjectResourceList = ProjectResourceList.OrderBy(r => r.Date);
                    break;
            }

            return ProjectResourceList;
        }

        public static IQueryable<ProjectResourceVM> Filter(this IQueryable<ProjectResourceVM> ProjectResourceList, ResourceFilterVM filter)
        {
            if (filter.filter_EndDate == DateTime.MinValue)
            {
                filter.filter_EndDate = DateTime.MaxValue;
            }

            ProjectResourceList = ProjectResourceList.Where(r => r.Date > filter.filter_StartDate && r.Date < filter.filter_EndDate);

            if (filter.filter_TaskGUID != null)
            {
                ProjectResourceList = ProjectResourceList.Where(r => r.TaskGUID == filter.filter_TaskGUID);
            }

            if (filter.filter_CategoryID != null)
            {
                ProjectResourceList = ProjectResourceList.Where(r => r.CategoryID == filter.filter_CategoryID);
            }

            return ProjectResourceList;
        }

        public static IEnumerable<int> GetCostsByDepartment(this IEnumerable<Department> departments)
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

        public static IEnumerable<int> GetBudgetsByDepartment(this IEnumerable<Department> departments)
        {
            List<int> result = new List<int>();

            foreach (var department in departments)
            {
                int sum = 0;

                foreach (var project in department.Project)
                {
                    sum += (int)project.ProjectBudget;
                }

                result.Add(sum);
            }

            return result;
        }

        public static IEnumerable<int> GetCostsByProjects(this IEnumerable<Project> projects)
        {
            List<int> result = new List<int>();

            foreach (var project in projects)
            {
                int sum = 0;

                foreach (var task in project.Tasks.GetLeafTasks())
                {
                    sum += (int)task.TaskResource.Select(r => r.UnitPrice * r.Quantity).Sum();
                }
                result.Add(sum);
            }

            return result;
        }

        public static IEnumerable<int> GetCostsByTasks(this IEnumerable<Tasks> rootTasks)
        {
            List<int> result = new List<int>();

            foreach (var root in rootTasks)
            {
                int sum = 0;

                foreach (var child in root.GetAllChildTasks())
                {
                    sum += (int)child.TaskResource.Select(r => r.UnitPrice * r.Quantity).Sum();
                }

                result.Add(sum);
            }

            return result;
        }

        public static IEnumerable<int> GetBudgetsByProject(this IEnumerable<Project> projects)
        {
            List<int> result = new List<int>();

            foreach (var project in projects)
            {
                result.Add((int)project.ProjectBudget);
            }

            return result;
        }

        public static IEnumerable<double> GetOverallRates(this IEnumerable<Project> projects, Guid? DepartmentID, Guid? ProjectID)
        {
            List<double> result = new List<double>();
            List<Project> ProjectList = new List<Project>();

            if (DepartmentID == null && ProjectID == null)
            {
                ProjectList = projects.ToList();
            }
            else if (ProjectID != null)
            {
                ProjectList.Add(projects.Where(p => p.ProjectGUID == ProjectID).Single());
            }
            else
            {
                ProjectList = projects.Where(p => p.RequiredDeptGUID == DepartmentID).ToList();
            }

            int CountOfAllLeafTasks = 0;
            int CountOfCompletedLeafTasks = 0;
            int BudgetOfAllProjects = 0;
            int CostOfAllProjects = 0;

            foreach (var project in ProjectList)
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

        public static IEnumerable<int> CountTasksByStatus(this IEnumerable<TaskStatus> statuses,Guid projectGUID)
        {
            List<int> result = new List<int>();

            foreach (var status in statuses)
            {
                int count = status.Tasks.Where(t => t.ProjectGUID == projectGUID).GetLeafTasks().Count();
                result.Add(count);
            }

            return result;
        }

    }
}
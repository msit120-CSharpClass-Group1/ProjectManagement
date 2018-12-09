﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public static class TasksBL
    {
        public enum Task_Status
        {
            Discussing = 1,
            InProgress,
            WaitForConfirmed,
            Completed,
            Ended
        }
        /// <summary>
        /// 回傳帶有orderID、parentOrderID排序完成的List&lt;Task>
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static IEnumerable<Tasks> GetSortedTasks(this IEnumerable<Tasks> tasks)
        {
            TreeGridModel treeGrid = new TreeGridModel(tasks.ToList());
            return treeGrid.SortedTasks;
        }        
        public static IEnumerable<Tasks> GetLeafTasks(this IEnumerable<Tasks> tasks)
        {
            var parentTasks = tasks.Where(t => t.ParentTaskGUID != null)
                .Select(t => t.ParentTaskGUID).Distinct().ToList();

            List<Tasks> leafTasks = new List<Tasks>();
            foreach (var item in tasks.Where(t => t.ParentTaskGUID != null))
            {
                if (!parentTasks.Where(parentGUID => parentGUID == item.TaskGUID).Any())
                {
                    leafTasks.Add(item);
                }
            }

            return leafTasks;
        }
        public static IEnumerable<Tasks> GetRootTasks(this IEnumerable<Tasks> tasks)
        {
            return tasks.Where(t => t.ParentTaskGUID == null).ToList();
        }

        #region ProjectRepo Chart
        public static IEnumerable<int> GetRootTasksCompletedRate(this IEnumerable<Tasks> rootTasks, IEnumerable<Tasks> tasksFromRepo)
        {
            List<int> rootTasksCompletedRate = new List<int>();
            TreeGridModel treeGrid = new TreeGridModel(tasksFromRepo.ToList());
            foreach (var root in rootTasks)
            {
                treeGrid.ChildLeafTasks = new List<Tasks>();
                treeGrid.GetChildLeafTasks(root);
                int completedLeafTaskCount = 0;
                int totalLeafCount = treeGrid.ChildLeafTasks.Count();
                foreach (var leaf in treeGrid.ChildLeafTasks)
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
                    string status = (root.TaskStatusID ?? default(int)).ToString();
                    Task_Status root_Status = (Task_Status)Enum.Parse(typeof(Task_Status), status);
                    if (root_Status == Task_Status.Completed)
                        rate = 100;
                }
                else
                {
                    rate = completedLeafTaskCount * 100 / totalLeafCount;
                }

                rootTasksCompletedRate.Add(rate);
            }

            return rootTasksCompletedRate;
        }
        public static IEnumerable<int> GetRootTasksWorkTimeSum(this IEnumerable<Tasks> rootTasks, IEnumerable<Tasks> tasksFromRepo)
        {
            List<int> rootTasksWorkTimeSum = new List<int>();
            TreeGridModel treeGrid = new TreeGridModel(tasksFromRepo.ToList());
            foreach (var root in rootTasks)
            {
                treeGrid.ChildLeafTasks = new List<Tasks>();
                treeGrid.GetChildLeafTasks(root);
                int _sum = (int)treeGrid.ChildLeafTasks.ToList().Select(t => t.EstWorkTime).Sum();
                rootTasksWorkTimeSum.Add(_sum);
            }
            return rootTasksWorkTimeSum;
        }
        public static IEnumerable<int> GetRootTasksResourceSum(this IEnumerable<Tasks> rootTasks, IEnumerable<Tasks> tasksFromRepo)
        {
            List<int> rootResourceSum = new List<int>();
            foreach (var root in rootTasks)
            {
                int _sum = (int)root.TaskResource.Select(r => r.UnitPrice * r.Quantity).Sum();
                foreach (var child in root.GetAllChildTasks())
                {
                    // _sum += (int)resourceFromRepo.Where(r => r.TaskGUID == child.TaskGUID).Select(r => r.UnitPrice * r.Quantity).Sum();   
                    _sum += (int)child.TaskResource.Select(r => r.UnitPrice * r.Quantity).Sum();
                }
                rootResourceSum.Add(_sum);
            }
            return rootResourceSum;
        }

        #endregion

        public static IEnumerable<Tasks> GetAllChildTasks(this Tasks task)
        {            
            TreeGridModel model = new TreeGridModel();
            model.GetChildren(task, new Repository<Tasks>().GetCollections().ToList());
            return model.ChildTasks;
        }
        public static bool IsAnyResource(this IEnumerable<Tasks> tasks)
        {
            Repository<TaskResource> resourceRepo = new Repository<TaskResource>();
            foreach (var task in tasks)
            {
                if (resourceRepo.GetCollections().Where(r => r.TaskGUID == task.TaskGUID).Any())
                    return true;
            }
            return false;
        }
        public static IEnumerable<Group<string, DisplayWorkloadVM>>GetTeamWorkLoad (this IEnumerable<Tasks> tasks)
        {
            Repository<Tasks> tasksRepo = new Repository<Tasks>();
            var workload = tasksRepo.GetCollections().Where(t => t.EmployeeGUID != null && t.TaskStatusID ==2).GetLeafTasks().GroupBy(g => g.Employee.EmployeeName)
                                           .Select(g => new Group<string, DisplayWorkloadVM> { Key = g.Key, Sum = g.Sum(e => e.EstWorkTime) }).OrderByDescending(g=>g.Sum);
            return workload;
        }
        public static int GetEstWorkTime(this Tasks task, HolidaysVM holidays)
        {
            int estWorkDays = 0;
            DateTime estStart = (DateTime)task.EstStartDate;
            DateTime estEnd = (DateTime)task.EstEndDate;
            if (holidays != null)
            {                
                var q = holidays.result.results.Select(r => new { date = DateTime.Parse(r.date), r.isHoliday });
                while (estStart.Date <= estEnd.Date)
                {
                    if (!(q.AsEnumerable().Where(r => r.date.Date == estStart.Date).Select(r => r.isHoliday).FirstOrDefault() == "是"))
                    {
                        estWorkDays++;
                    }
                    estStart = estStart.AddDays(1);
                }
            }
            else
            {
                //if api not work, turn to simple mode forbidding Weekends
                while (estStart.Date <= estEnd.Date)
                {
                    if (estStart.DayOfWeek != DayOfWeek.Saturday && estStart.DayOfWeek != DayOfWeek.Sunday)
                    {
                        estWorkDays++;
                    }
                    estStart = estStart.AddDays(1);
                }
            }

            return estWorkDays * 8;
        }
        public static int GetWorkTime(this Tasks task, HolidaysVM holidays)
        {
            int WorkDays = 0;
            DateTime startDate = (DateTime)task.StartDate;
            DateTime endDate = (DateTime)task.EndDate;
            if (holidays != null)
            {
                var q = holidays.result.results.Select(r => new { date = DateTime.Parse(r.date), r.isHoliday });
                while (startDate.Date <= endDate.Date)
                {
                    if (!(q.AsEnumerable().Where(r => r.date.Date == startDate.Date).Select(r => r.isHoliday).FirstOrDefault() == "是"))
                    {
                        WorkDays++;
                    }
                    startDate = startDate.AddDays(1);
                }
            }
            else
            {
                //if api not work, turn to simple mode forbidding Weekends
                while (startDate.Date <= endDate.Date)
                {
                    if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        WorkDays++;
                    }
                    startDate = startDate.AddDays(1);
                }
            }

            return WorkDays * 8;
        }
        public static IEnumerable<TasksGroupByStatus> GetTasksGroupByStatus(this IEnumerable<Tasks> tasks)
        {
            var q = tasks.Where(t => t.TaskStatusID != (int)TasksBL.Task_Status.Discussing)
                .OrderBy(t => t.TaskStatusID)
                .GroupBy(t => t.TaskStatusID)
                .Select(g => new Grouped<int?,Tasks> { Key = g.Key, group = g });
        }

    }
}
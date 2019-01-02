using System;
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
            Closed
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
        public static IEnumerable<Tasks> GetAllChildTasks(this Tasks task)
        {

            TreeGridModel model = new TreeGridModel();
            model.GetChildren(task, new Repository<Tasks>().GetCollections().ToList());
            return model.ChildTasks;
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
        public static IEnumerable<int> GetRootTasksEstWorkTimeSum(this IEnumerable<Tasks> rootTasks, IEnumerable<Tasks> tasksFromRepo)
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
        public static IEnumerable<int> GetEstWorkTimeSumOfProjectMembers(this IEnumerable<ProjectMembers> projectMembers, IEnumerable<Tasks> tasksFromRepo)
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
        public static IEnumerable<int> GetWorkTimeSumOfProjectMembers(this IEnumerable<ProjectMembers> projectMembers, IEnumerable<Tasks> tasksFromRepo)
        {
            List<int> workTimeSums = new List<int>();
            var leafTasks = tasksFromRepo.GetLeafTasks();
            foreach (var member in projectMembers)
            {
                int _sum = (int)leafTasks.Where(t => t.EmployeeGUID == member.EmployeeGUID).Select(t => t.WorkTime).Sum();
                workTimeSums.Add(_sum);
            }
            return workTimeSums;
        }
        #endregion

        public static IEnumerable<Tasks> UpdateStatusAndDuration(this IEnumerable<Tasks> tasks)
        {   //this function cause 1-2 sec delay of page...
            tasks = tasks.GetSortedTasks().Reverse();   //turn to post-order
            foreach (var task in tasks)
            {
                var childen = task.GetAllChildTasks();
                if (childen.Count() > 0)
                {
                    if(childen.All(t=>t.TaskStatusID == (int)Task_Status.Discussing || t.TaskStatusID == (int)Task_Status.Closed))
                    {
                        task.TaskStatusID = (int)Task_Status.Discussing;
                    }
                    else
                    {
                        task.TaskStatusID = (int)Task_Status.InProgress;
                    }
                    if(childen.All(t=>t.TaskStatusID == (int)Task_Status.Completed || t.TaskStatusID == (int)Task_Status.Closed))
                    {
                        task.TaskStatusID = (int)Task_Status.Completed;
                    }

                }
            }
            return tasks.Reverse().ToList();
        }

        /// <summary>
        /// 參數StatusID指task變更後的StatusID
        /// </summary>
        /// <param name="task"></param>
        /// <param name="taskRepo"></param>
        /// <param name="StatusID"></param>a
        public static void ParentTaskStatusUpdate(this Tasks task, Repository<Tasks> taskRepo, int StatusID)
        {
            bool flag = false;
            if (task.ParentTaskGUID == null)
                return;
            var parentTask = taskRepo.Find(task.ParentTaskGUID);

            if (parentTask.GetAllChildTasks().All(t => t.TaskStatusID == (int)Task_Status.Closed))
            {
                parentTask.TaskStatusID = (int)Task_Status.Closed;
                taskRepo.Update(taskRepo.Find(parentTask.TaskGUID));
                flag = true;
            }
            else if (parentTask.GetAllChildTasks()
                .All(t => t.TaskStatusID == StatusID || t.TaskStatusID == (int)Task_Status.Closed))
            {
                parentTask.TaskStatusID = StatusID;
                taskRepo.Update(taskRepo.Find(parentTask.TaskGUID));
                flag = true;
            }
            if (flag && parentTask.ParentTaskGUID != null)
                ParentTaskStatusUpdate(taskRepo.Find(task.ParentTaskGUID), taskRepo, StatusID);
                       
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
        public static int GetAutoEstWorkTime(this Tasks task, HolidaysVM holidays)
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

        public static IEnumerable<Group<string, DisplayWorkloadVM>>GetTeamWorkLoad (this IEnumerable<Tasks> tasks)
        {
            Repository<Tasks> tasksRepo = new Repository<Tasks>();
            var workload = tasksRepo.GetCollections().Where(t => t.EmployeeGUID != null && t.TaskStatusID ==2).GetLeafTasks().GroupBy(g => g.Employee.EmployeeName)
                                           .Select(g => new Group<string, DisplayWorkloadVM> { Key = g.Key, Sum = g.Sum(e => e.EstWorkTime) }).OrderByDescending(g=>g.Sum);
            return workload;
        }

        #region Dashboard
        public static IEnumerable<Grouped<string, Tasks>> GetTasksGroupByStatus(this IEnumerable<Tasks> tasks)
        {
            return tasks.Where(t => t.TaskStatusID != (int)TasksBL.Task_Status.Discussing)
                .OrderBy(t => t.TaskStatusID)
                .GroupBy(t => t.TaskStatus.TaskStatusName)
                .Select(g => new Grouped<string, Tasks> { Key = g.Key, group = g });
        }
        #endregion
        #region Excel Import
        public static IEnumerable<Tasks> GetSortedExcelTasks(this IEnumerable<ExcelTasks> excelTasks, Guid projectGUID)
        {
            List<Tasks> sortedTasks = new List<Tasks>();
            excelTasks = excelTasks.OrderBy(x => x.ExcelTaskID).ToList();
            var roots = excelTasks.Where(x => x.ExcelParentTaskID == null);
            foreach (var root in roots)
            {
                Tasks task = new Tasks();
                root.ProjectGUID = task.ProjectGUID = projectGUID;
                root.TaskGUID = task.TaskGUID = Guid.NewGuid();                            
                root.ParentTaskGUID = task.ParentTaskGUID = null;
                root.TaskStatusID = task.TaskStatusID = (int)TasksBL.Task_Status.Discussing;
                root.AssignedDate = task.AssignedDate = DateTime.Now;
                root.IsRead = task.IsRead = false;

                task.TaskName = root.TaskName;
                task.EstStartDate = root.EstStartDate;
                task.EstEndDate = root.EstEndDate;
                root.StartDate = task.StartDate = task.EstStartDate;
                root.EndDate = task.EndDate = task.EstEndDate;
                task.EstWorkTime = root.EstWorkTime;
                task.Description = root.Description;               
                
                sortedTasks.Add(task);
                root.GetChildTasks(excelTasks, sortedTasks);
            }
            return sortedTasks;
            
        }
        public static void GetChildTasks(this ExcelTasks parentTask, IEnumerable<ExcelTasks> excelTasks, List<Tasks> sortedTasks)
        {
            var children = excelTasks.Where(et => et.ExcelParentTaskID == parentTask.ExcelTaskID).ToList();
            foreach (var child in children)
            {
                Tasks task = new Tasks();
                child.ProjectGUID = task.ProjectGUID = parentTask.ProjectGUID;
                child.TaskGUID = task.TaskGUID = Guid.NewGuid();
                child.ParentTaskGUID = task.ParentTaskGUID = parentTask.TaskGUID;
                child.TaskStatusID = task.TaskStatusID = (int)TasksBL.Task_Status.Discussing;
                child.AssignedDate = task.AssignedDate = DateTime.Now;
                child.IsRead = task.IsRead = false;

                task.TaskName = child.TaskName;                
                task.EstStartDate = child.EstStartDate;
                task.EstEndDate = child.EstEndDate;
                child.StartDate = task.StartDate = task.EstStartDate;
                child.EndDate = task.EndDate = task.EstEndDate;
                task.EstWorkTime = child.EstWorkTime;
                task.Description = child.Description;

                sortedTasks.Add(task);
                child.GetChildTasks(excelTasks, sortedTasks);
            }
        }
        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManager.Models;

namespace ProjectManager.Models
{
    public static class CostBL
    {
        public static IEnumerable<int> GetSubtotalByDepartment(this IEnumerable<Department> dpts)
        {
            List<int> result = new List<int>();

            foreach(var dpt in dpts)
            {
                int sum = 0;

                foreach (var project in dpt.Project)
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

    }
}
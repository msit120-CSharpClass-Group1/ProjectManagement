using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class ExcelTasks
    {
        public string ExcelTaskID { get; set; }
        public string ExcelParentTaskID { get; set; }

        public Guid? TaskGUID { get; set; }
        public Guid? ParentTaskGUID { get; set; }
        public Nullable<System.Guid> ProjectGUID { get; set; }
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        public Nullable<System.DateTime> EstStartDate { get; set; }
        public Nullable<System.DateTime> EstEndDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> EstWorkTime { get; set; }
        public Nullable<int> WorkTime { get; set; }
        public Nullable<int> TaskStatusID { get; set; }
        public Nullable<System.Guid> EmployeeGUID { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public Nullable<byte> ReviewScore { get; set; }
        public string ReviewDescription { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public Nullable<bool> IsRead { get; set; }


    }
}
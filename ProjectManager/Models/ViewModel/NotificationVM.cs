using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models.ViewModel
{
    public class NotificationVM
    {
        public string TaskName { get; set; }
        public string ProjectName { get; set; }
        public string Category { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsRead { get; set; }
        public string EmployeeName { get; set; }
        public string MangerName { get; set; }
        public Guid EmployeeGUID { get; set; }
        public Guid ProjectGUID { get; set; }
    }
}
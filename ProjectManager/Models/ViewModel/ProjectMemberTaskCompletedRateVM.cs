using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class ProjectMemberTaskCompletedRateVM
    {
        public string EmployeeName { get; set; }
        public int CompletedRate { get; set; }
        public int InChargeTaskCount { get; set; }
        public int EstWorkTimeSum { get; set; }
    }
}
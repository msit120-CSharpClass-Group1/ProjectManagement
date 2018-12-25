using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class SelectMBoardVM
    {
        public string EmployeeName { get; set; }
        public Guid? EmployeeGuid { get; set; }
        public int CompletedRate { get; set; }
        public int InchargeTaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
    }
}
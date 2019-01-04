using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class InviteMemberVM
    {
        //public IEnumerable<Employee> Employees { get; set; }
        //public IEnumerable<ProjectMembers> ProjectMembers { get; set; }
        public Guid? DepartmentGUID { get; set; }
        public Guid? EmployeeGUID { get; set; }
        public string EmployeeName { get; set; }
        public Guid? TitleGUID { get; set; }
        public double? PMReviewAVGScore { get; set; }
        public int OwnProjectCount { get; set; }
    }
}
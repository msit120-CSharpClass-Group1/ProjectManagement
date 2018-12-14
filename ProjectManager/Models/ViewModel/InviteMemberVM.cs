using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class InviteMemberVM
    {
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<ProjectMembers> ProjectMembers { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class ProjectMemberScoreVM
    {
        public System.Guid EmployeeGUID { get; set; }
        public IEnumerable<Guid> ProjectGUID { get; set; }
        public IEnumerable<string> ProjectName { get; set; }
        public Nullable<byte> PMscore { get; set; }
        public Nullable<byte> Selfscore { get; set; }
        public IEnumerable<string> InProgressProjectStatusName { get; set; }
        public IEnumerable<string> ClosedProjectStatusName { get; set; }
        //public virtual Employee Employee { get; set; }
        public virtual Project Project { get; set; }
        public List<Project> InProgressProject { get; set; }
        public IEnumerable<Guid> ClosedProject { get; set; }
        public IEnumerable<Group<string, ProjectMemberScoreVM>> GroupMember { get; set; }
        public IEnumerable<Group<string,ProjectMemberScoreVM>> GetPMscore { get; set; }
        public IEnumerable<Group<string, ProjectMemberScoreVM>> GetMemberCount { get; set; }
        //public IEnumerable<Project> project { get; set; }
    }    
}
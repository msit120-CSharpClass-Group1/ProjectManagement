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
        public virtual Project Project { get; set; }
        public List<Project> InProgressProject { get; set; }
        public List<Project> ClosedProject { get; set; }
        public IEnumerable<Group<string, ProjectMembers>> GroupMembersScore { get; set; }
        public IEnumerable<Group<string, ProjectMemberScoreVM>> GetPMscore { get; set; }
        public IEnumerable<Group<string, ProjectMemberScoreVM>> GetMemberCount { get; set; }
        public IEnumerable<ProjectMembers> TeamMember { get; set; }
        public IEnumerable<Group<string, ProjectMembers>> GetHighestMember { get; set; }
        public IEnumerable<Group<string, ProjectMembers>> GetLowestMember { get; set; }
        public List<ProjectMembers> GetAboveAVGMember { get; set; }
        public List<ProjectMembers> GetUnderAVGMember { get; set; }
        public IEnumerable<Group<string, ProjectMembers>> GetNoneScore { get; set; }
        public IEnumerable<Group<string, Tasks>> GroupMemberTaskScore { get; set; }
        public IEnumerable<Group<string, Tasks>> GetTaskAVGScore { get; set; }
        public IEnumerable<ProjectMembers> ProjectMembers { get; set; }
    }
}
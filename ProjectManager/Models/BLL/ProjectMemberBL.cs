using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public static class ProjectMemberBL
    {
        /// <summary>
        /// 回傳處理過的PM評分、自評、團隊數量
        /// </summary>
        /// <param name="projectMembers"></param>
        /// <returns></returns>
        public static IEnumerable<Group<string, ProjectMemberScoreVM>> GetTeamMemberCount(this IEnumerable<ProjectMembers> projectMembers)
        {
            var teamCount = projectMembers.Where(p => p.EmployeeGUID != null).GroupBy(p => p.Project.ProjectName).Select(g => new Group<string, ProjectMemberScoreVM> { Key = g.Key, Count = g.Count() });
            return teamCount;
        }

        public static IEnumerable<Group<string, ProjectMemberScoreVM>> GetTeamPMAvgScore(this IEnumerable<ProjectMembers> projectMembers)
        {                   
            var averageScore = projectMembers.Where(p => p.EmployeeGUID != null && p.PMscore!=null).GroupBy(g => g.Project.ProjectName)
                                           .Select(g => new Group<string, ProjectMemberScoreVM> { Key = g.Key, Avg= g.Average(p=>p.PMscore)}).OrderByDescending(p=>p.Avg);
            List<Group<string, ProjectMemberScoreVM>> avgPMScore = new List<Group<string, ProjectMemberScoreVM>>();
            foreach (var item in averageScore)
            {
                item.Avg = Math.Round((double)item.Avg, 2);
                avgPMScore.Add(item);
            }
            return avgPMScore;
        }

        public static IEnumerable<Group<string, ProjectMemberScoreVM>> GetTeamSelfScore(this IEnumerable<ProjectMembers> projectMembers)
        {           
            var averageScore = projectMembers.Where(p => p.EmployeeGUID != null && p.Selfscore != null).GroupBy(g => g.Project.ProjectName)
                                            .Select(g => new Group<string, ProjectMemberScoreVM> { Key = g.Key, Avg = g.Average(p => p.PMscore) }).OrderByDescending(p => p.Avg);
            List<Group<string, ProjectMemberScoreVM>> avgSelfScore = new List<Group<string, ProjectMemberScoreVM>>();
            foreach (var item in averageScore)
            {
                item.Avg = Math.Round((double)item.Avg, 2);
                avgSelfScore.Add(item);
            }
            return avgSelfScore;
        }

        public static IEnumerable<Group<string, ProjectMemberScoreVM>> GroupMembers(this IEnumerable<ProjectMembers> projectMembers)
        {
            var groupList = projectMembers.Where(p => p.EmployeeGUID != null).GroupBy(g => g.Project.ProjectName)
                                            .Select(g => new Group<string, ProjectMemberScoreVM> {Key = g.Key});
            List<Group<string, ProjectMemberScoreVM>> avgSelfScore = new List<Group<string, ProjectMemberScoreVM>>();
            return groupList;
        }
    }
}
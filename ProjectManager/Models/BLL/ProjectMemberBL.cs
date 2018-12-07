﻿using System;
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

        public static IEnumerable<Group<string, ProjectMembers>> GroupMembersScore (this IEnumerable<ProjectMembers> projectMembers)
        {
            var groupList = projectMembers.Where(p => p.EmployeeGUID != null).GroupBy(g => g.Project.ProjectName)
                                            .Select(g => new Group<string,ProjectMembers> { Key=g.Key, value = g });         
            return groupList;
        }

        public static IEnumerable<Group<string, ProjectMembers>> GetHighestMember(this IEnumerable<ProjectMembers> projectMembers)
        {
            var memeberList = projectMembers.Where(p => p.PMscore >= 80 && p.EmployeeGUID != null).GroupBy(g => g.Employee.EmployeeName)
                                         .Select(g => new Group<string, ProjectMembers> { Key = g.Key,value =g,Count=g.Count()});
            return memeberList;
        }

        public static IEnumerable<Group<string, ProjectMembers>> GetLowestMember(this IEnumerable<ProjectMembers> projectMembers)
        {
            var memeberList = projectMembers.Where(p => p.PMscore <= 40 && p.EmployeeGUID != null).GroupBy(g => g.Employee.EmployeeName)
                                         .Select(g => new Group<string, ProjectMembers> { Key = g.Key, value = g, Count = g.Count() });
            return memeberList;
        }

        public static IEnumerable<Group<string, ProjectMembers>> GetNoneScore(this IEnumerable<ProjectMembers> projectMembers)
        {
            var memeberList = projectMembers.Where(p=>p.PMscore==null && p.EmployeeGUID != null).GroupBy(g => g.Employee.EmployeeName)
                                         .Select(g => new Group<string, ProjectMembers> { Key = g.Key, value = g, Count = g.Count() });
            return memeberList;
        }

        public static List<ProjectMembers> GetAboveAVGMember(this IEnumerable<ProjectMembers> projectMembers)
        {
            List<ProjectMembers> aboveAvgMember = new List<ProjectMembers>();
            double? memberPMScoreAVG = projectMembers.GetTeamPMAvgScore().FirstOrDefault().Avg;
            var highestMemberPMScore = projectMembers.Where(p=>p.EmployeeGUID != null).GroupBy(g => g.Employee.EmployeeName)
                                         .Select(g => new Group<string, ProjectMembers> { Key = g.Key, value = g, Count = g.Count()});
            foreach (var _member in highestMemberPMScore)
            {
               foreach(var _item in _member.value)
                {
                    if (_item.PMscore > memberPMScoreAVG)
                    {
                        aboveAvgMember.Add(_item);
                    }
                }
            }
            return aboveAvgMember;
        }

        public static List<ProjectMembers> GetUnderAVGMember(this IEnumerable<ProjectMembers> projectMembers)
        {
            List<ProjectMembers> AvgMemberScore = new List<ProjectMembers>();
            double? memberPMScoreAVG = projectMembers.GetTeamPMAvgScore().FirstOrDefault().Avg;
            var lowestMemberPMScore = projectMembers.Where(p => p.EmployeeGUID != null).GroupBy(g => g.Employee.EmployeeName)
                                         .Select(g => new Group<string, ProjectMembers> { Key = g.Key, value = g, Count = g.Count() });
            foreach (var _member in lowestMemberPMScore)
            {
                foreach (var _item in _member.value)
                {
                    if (_item.PMscore < memberPMScoreAVG)
                    {
                        AvgMemberScore.Add(_item);
                    }
                }
            }
            return AvgMemberScore;
        }
    }
}
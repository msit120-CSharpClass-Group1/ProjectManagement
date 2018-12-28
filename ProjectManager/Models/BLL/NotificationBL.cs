using ProjectManager.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models.BLL
{
    public static class NotificationBL
    {
        public static List<NotificationVM> GetNotifications(this Members _members, List<Tasks> tasks, List<ProjectMembers> projectMembers, List<Calendar> calendars)
        {
            List<NotificationVM> _GetNotifications = new List<NotificationVM>();
            var _tasks = tasks
                .Where(n => n.EmployeeGUID == _members.EmployeeGUID)
                .Select(n => new { n.TaskName, n.AssignedDate, n.IsRead, n.EmployeeGUID, n.Project.ProjectName, n.ProjectGUID, n.Project.Employee1.EmployeeName });
            var _projectMember = projectMembers
                .Where(n => n.EmployeeGUID == _members.EmployeeGUID)
                .Select(n => new { n.ProjectGUID, n.Project.ProjectName, n.Project.Employee1.EmployeeName, n.IsRead, n.InvideDate });
            var _calendars = calendars
                .Where(n => n.Members.EmployeeGUID == _members.EmployeeGUID&& n.CategoryID ==2)
                .Select(n => new { n.Subject, n.IsRead, n.Start ,n.CreateDate});
            foreach (var t in _tasks)
            {
                NotificationVM vm = new NotificationVM();
                vm.TaskName = t.TaskName;
                vm.Category = "Task";
                vm.ProjectName = t.ProjectName;
                vm.ProjectGUID = (Guid)t.ProjectGUID;
                vm.NotificationDate = (DateTime)t.AssignedDate;
                vm.EmployeeName = _members.Employee.EmployeeName;
                vm.EmployeeGUID = (Guid)_members.EmployeeGUID;
                vm.MangerName = t.EmployeeName;
                vm.IsRead = (bool)t.IsRead;
                _GetNotifications.Add(vm);
            }
            foreach (var pb in _projectMember)
            {
                NotificationVM vm = new NotificationVM();
                vm.NotificationDate = (DateTime)pb.InvideDate;
                vm.Category = "InvideProject";
                vm.ProjectName = pb.ProjectName;
                vm.ProjectGUID = pb.ProjectGUID;
                vm.MangerName = pb.EmployeeName;
                vm.EmployeeName = _members.Employee.EmployeeName;
                vm.EmployeeGUID = (Guid)_members.EmployeeGUID;
                vm.IsRead = (bool)pb.IsRead;
                _GetNotifications.Add(vm);
            }
            foreach (var c in _calendars)
            {
                NotificationVM vm = new NotificationVM();
                vm.NotificationDate = (DateTime)c.CreateDate;
                vm.Category = "Calendar";
                vm.CalendarDate = c.Start;
                vm.CalendarName = c.Subject;
                vm.EmployeeName = _members.Employee.EmployeeName;
                vm.EmployeeGUID = (Guid)_members.EmployeeGUID;
                vm.IsRead = (bool)c.IsRead;
                _GetNotifications.Add(vm);
            }
            return _GetNotifications;
        }


    }
}
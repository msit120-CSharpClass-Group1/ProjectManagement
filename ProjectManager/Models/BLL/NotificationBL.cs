using ProjectManager.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models.BLL
{
    public static class NotificationBL
    {
        public static List<NotificationVM> GetNotifications(this Members _members)
        {
            IRepository<Tasks> tasks = new Repository<Tasks>();
            IRepository<ProjectMembers> projectMembers = new Repository<ProjectMembers>();

            List<NotificationVM> _GetNotifications = new List<NotificationVM>();
            var _tasks = tasks.GetCollections()
                .Where(n => n.EmployeeGUID == _members.EmployeeGUID)
                .Select(n => new { n.TaskName, n.AssignedDate, n.IsRead, n.EmployeeGUID, n.Project.ProjectName, n.ProjectGUID, n.Project.Employee1.EmployeeName });
            var _projectMember = projectMembers.GetCollections()
                .Where(n => n.EmployeeGUID == _members.EmployeeGUID)
                .Select(n => new { n.ProjectGUID, n.Project.ProjectName, n.Project.Employee1.EmployeeName, n.IsRead, n.InvideDate });
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
            return _GetNotifications;
        }


    }
}
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectManager.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ProjectManagementEntities : DbContext
    {
        public ProjectManagementEntities()
            : base("name=ProjectManagementEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Calendar> Calendar { get; set; }
        public virtual DbSet<Dashboard> Dashboard { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentCategory> DocumentCategory { get; set; }
        public virtual DbSet<DocumentModified> DocumentModified { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<JobTitle> JobTitle { get; set; }
        public virtual DbSet<Meeting> Meeting { get; set; }
        public virtual DbSet<MeetingDetail> MeetingDetail { get; set; }
        public virtual DbSet<Members> Members { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<PreTasks> PreTasks { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectCategory> ProjectCategory { get; set; }
        public virtual DbSet<ProjectDiscuss> ProjectDiscuss { get; set; }
        public virtual DbSet<ProjectMembers> ProjectMembers { get; set; }
        public virtual DbSet<ProjectStatus> ProjectStatus { get; set; }
        public virtual DbSet<ResourceCategory> ResourceCategory { get; set; }
        public virtual DbSet<TaskDetail> TaskDetail { get; set; }
        public virtual DbSet<TaskDetailStatus> TaskDetailStatus { get; set; }
        public virtual DbSet<TaskModified> TaskModified { get; set; }
        public virtual DbSet<TaskResource> TaskResource { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<TaskStatus> TaskStatus { get; set; }
        public virtual DbSet<WidgetDetail> WidgetDetail { get; set; }
        public virtual DbSet<Widgets> Widgets { get; set; }
    
        public virtual int InsertProjectForDemo()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertProjectForDemo");
        }
    
        public virtual int InsertTasksForDemo()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertTasksForDemo");
        }
    }
}

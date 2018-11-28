using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    [MetadataType(typeof(metadataTasks))]
    public partial class Tasks
    {
        public class metadataTasks
        {
            [JsonIgnore]
            public Nullable<System.Guid> ProjectGUID { get; set; }
            public System.Guid TaskGUID { get; set; }
            [JsonIgnore]
            public Nullable<System.Guid> ParentTaskGUID { get; set; }
            [JsonIgnore]
            public int TaskID { get; set; }
            [JsonIgnore]
            public Nullable<System.DateTime> EstStartDate { get; set; }
            [JsonIgnore]
            public Nullable<System.DateTime> EstEndDate { get; set; }
            [JsonIgnore]
            public Nullable<System.DateTime> StartDate { get; set; }
            [JsonIgnore]
            public Nullable<System.DateTime> EndDate { get; set; }
            [JsonIgnore]
            public Nullable<int> EstWorkTime { get; set; }
            [JsonIgnore]
            public Nullable<int> WorkTime { get; set; }
            [JsonIgnore]
            public Nullable<int> TaskStatusID { get; set; }
            [JsonIgnore]
            public Nullable<System.Guid> EmployeeGUID { get; set; }
            [JsonIgnore]
            public string Tag { get; set; }
            public string Description { get; set; }
            [JsonIgnore]
            public Nullable<int> TaskStatusIDChanged { get; set; }
            [JsonIgnore]
            public virtual Employee Employee { get; set; }
            [JsonIgnore]
            public virtual PreTasks PreTasks { get; set; }
            [JsonIgnore]
            public virtual Project Project { get; set; }
            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<TaskDetail> TaskDetail { get; set; }
            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<TaskModified> TaskModified { get; set; }
            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<TaskResource> TaskResource { get; set; }
            [JsonIgnore]
            public virtual TaskStatus TaskStatus { get; set; }



        }
    }
}
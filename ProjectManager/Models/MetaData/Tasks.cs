using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Newtonsoft.Json;

namespace ProjectManager.Models
{
    [MetadataType(typeof(TasksMetadata))]
    public partial class Tasks
    {
        [NotMapped]
        public int orderID { get; set; }    //for treeGrid class
        [NotMapped]
        public int parentOrderID { get; set; }

        public class TasksMetadata
        {
            [DisplayName("工作項目名稱")]
            public string TaskName { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
            [DisplayName("預計開始時間")]
            public Nullable<System.DateTime> EstStartDate { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
            [DisplayName("預計結束時間")]
            public Nullable<System.DateTime> EstEndDate { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
            [DisplayName("實際開始時間")]
            public Nullable<System.DateTime> StartDate { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
            [DisplayName("實際結束時間")]
            public Nullable<System.DateTime> EndDate { get; set; }

            [DisplayName("實際開始時間")]
            public Nullable<int> EstWorkTime { get; set; }

            [DisplayName("實際結束時間")]
            public Nullable<int> WorkTime { get; set; }

            [DisplayName("狀態")]
            public Nullable<int> TaskStatusID { get; set; }

            [DisplayName("負責人")]
            public Nullable<System.Guid> EmployeeGUID { get; set; }

            [DisplayName("備註")]
            public string Tag { get; set; }

            [DisplayName("說明")]
            public string Description { get; set; }

            [JsonIgnore()]
            public virtual Employee Employee { get; set; }
            [JsonIgnore()]
            public virtual PreTasks PreTasks { get; set; }
            [JsonIgnore()]
            public virtual Project Project { get; set; }
            [JsonIgnore()]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<TaskDetail> TaskDetail { get; set; }
            [JsonIgnore()]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<TaskModified> TaskModified { get; set; }
            [JsonIgnore()]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<TaskResource> TaskResource { get; set; }
            [JsonIgnore()]
            public virtual TaskStatus TaskStatus { get; set; }
        }
    }
}
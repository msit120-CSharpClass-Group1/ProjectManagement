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
        [NotMapped]
        [DisplayName("預計區間總時數")]
        public int AutoWorkTime { get; set; }

        public class TasksMetadata
        {
            public Nullable<System.Guid> ProjectGUID { get; set; }
            public System.Guid TaskGUID { get; set; }
            [DisplayName("父工作項目")]
            public Nullable<System.Guid> ParentTaskGUID { get; set; }
            [DisplayName("工作項目編號")]
            public int TaskID { get; set; }

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

            [DisplayName("預計工時")]
            public Nullable<int> EstWorkTime { get; set; }

            [DisplayName("實際工時")]
            public Nullable<int> WorkTime { get; set; }

            [DisplayName("狀態")]
            public Nullable<int> TaskStatusID { get; set; }

            [DisplayName("負責人")]
            public Nullable<System.Guid> EmployeeGUID { get; set; }

            [DisplayName("備註")]
            public string Tag { get; set; }

            [DisplayName("說明")]
            public string Description { get; set; }

            [DisplayName("驗收評分")]
            public Nullable<byte> ReviewScore { get; set; }

            [DisplayName("驗收評價")]
            public string ReviewDescription { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
            [DisplayName("分配日期")]
            public Nullable<System.DateTime> AssignedDate { get; set; }
            [DisplayName("已讀")]
            public Nullable<bool> IsRead { get; set; }

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
            [JsonIgnore()]
            public virtual ICollection<Tasks> Tasks1 { get; set; }
            [JsonIgnore()]
            public virtual Tasks Tasks2 { get; set; }
        }
    }
}
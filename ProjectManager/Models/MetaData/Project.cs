using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(ProjectMetaData))]
    public partial class Project
    {
        [NotMapped]
        [DisplayName("完成比率")]
        public int CompletedRate { get; set; }

        [NotMapped]
        [DisplayName("預估天期變動比率")]
        public int EstDurationModifiedRate { get; set; }

        [NotMapped]
        [DisplayName("實際工期變動比率")]
        public int DurationModifiedRate { get; set; }

        [NotMapped]
        [DisplayName("實際工期節省比率")]
        public int DurationSavedRate { get; set; }      
    }
    public class ProjectMetaData
    {
        [DisplayName("專案編號")]
        public string ProjectID { get; set; }        
        public System.Guid ProjectGUID { get; set; }
        [DisplayName("需求部門")]
        public Nullable<System.Guid> RequiredDeptGUID { get; set; }
        [DisplayName("需求部門PM")]
        public Nullable<System.Guid> RequiredDeptPMGUID { get; set; }
        [DisplayName("專案名稱")]
        public string ProjectName { get; set; }
        [DisplayName("預計開始日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<System.DateTime> EstStartDate { get; set; }
        [DisplayName("預計結束日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<System.DateTime> EstEndDate { get; set; }
        [DisplayName("實際開始日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DisplayName("實際結束日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<System.DateTime> EndDate { get; set; }        
        public Nullable<System.Guid> InChargeDeptGUID { get; set; }        
        public Nullable<System.Guid> InChargeDeptPMGUID { get; set; }
        [DisplayName("專案階段")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<int> ProjectStatusID { get; set; }
        [DisplayName("專案類別")]        
        public Nullable<int> ProjectCategoryID { get; set; }
        [DisplayName("專案督導")]
        public Nullable<System.Guid> ProjectSupervisorGUID { get; set; }
        [DisplayName("總經理關注")]
        public Nullable<bool> IsGeneralManagerConcerned { get; set; }
        [DisplayName("專案預算")]
        public Nullable<int> ProjectBudget { get; set; }
        [DisplayName("專案描述")]
        public string Description { get; set; }

        //Michael Add
        [JsonIgnore]
        public virtual Department Department { get; set; }

        [JsonIgnore]
        public virtual Department Department1 { get; set; }

        [JsonIgnore]
        public virtual ProjectStatus ProjectStatus { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<ProjectMembers> ProjectMembers { get; set; }
        [JsonIgnore]
        public virtual ProjectCategory ProjectCategory { get; set; }

    }
}

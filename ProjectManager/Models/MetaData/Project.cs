using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(ProjectMetaData))]
    public partial class Project
    {
    }

    public class ProjectMetaData
    {
        [DisplayName("專案編號")]
        public string ProjectID { get; set; }
        public System.Guid ProjectGUID { get; set; }
        public Nullable<System.Guid> RequiredDeptGUID { get; set; }
        public Nullable<System.Guid> RequiredDeptPMGUID { get; set; }
        [DisplayName("專案名稱")]
        public string ProjectName { get; set; }
        [DisplayName("預計開始日期")]
        public Nullable<System.DateTime> EstStartDate { get; set; }
        [DisplayName("預計結束日期")]
        public Nullable<System.DateTime> EstEndDate { get; set; }
        [DisplayName("實際開始日期")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DisplayName("實際結束日期")]
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.Guid> InChargeDeptGUID { get; set; }
        public Nullable<System.Guid> InChargeDeptPMGUID { get; set; }
        [DisplayName("專案階段編號")]
        public Nullable<int> ProjectStatusID { get; set; }
        [DisplayName("專案類別編號")]
        public Nullable<int> ProjectCategoryID { get; set; }
        public Nullable<System.Guid> ProjectSupervisorGUID { get; set; }
        [DisplayName("總經理關注")]
        public Nullable<bool> IsGeneralManagerConcerned { get; set; }
        [DisplayName("專案預算")]
        public Nullable<int> ProjectBudget { get; set; }
        [DisplayName("專案描述")]
        public string Description { get; set; }
    }
}

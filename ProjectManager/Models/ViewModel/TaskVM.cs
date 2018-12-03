using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManager.Models.ViewModel
{
    public class TaskVM
    {

        public Nullable<System.Guid> ProjectGUID { get; set; }
        public System.Guid TaskGUID { get; set; }
        public Nullable<System.Guid> ParentTaskGUID { get; set; }

        [DisplayName("任務編號")]
        public int TaskID { get; set; }

        [DisplayName("任務名稱")]
        public string TaskName { get; set; }

        [DisplayName("預計開始日期")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> EstStartDate { get; set; }

        [DisplayName("預計結束日期")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> EstEndDate { get; set; }

        [DisplayName("實際開始日期")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> StartDate { get; set; }

        [DisplayName("實際結束日期")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> EndDate { get; set; }

        [DisplayName("預計工作時間")]
        public Nullable<int> EstWorkTime { get; set; }

        [DisplayName("實際工作時間")]
        public Nullable<int> WorkTime { get; set; }
        public Nullable<System.Guid> EmployeeGUID { get; set; }

        [DisplayName("標籤")]
        public string Tag { get; set; }

        [DisplayName("描述")]
        public string Description { get; set; }

    }
}
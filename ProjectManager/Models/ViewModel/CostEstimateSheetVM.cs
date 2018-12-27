using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class CostEstimateSheetVM
    {
        public System.Guid SheetGUID { get; set; }
        public int SheetID { get; set; }
        [DisplayName("估價單名稱")]
        public string SheetName { get; set; }
        public Nullable<System.Guid> ProjectGUID { get; set; }
        [DisplayName("專案名稱")]
        public string ProjectName { get; set; }
        [DisplayName("備註")]
        public string Description { get; set; }
        [DisplayName("建立日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DisplayName("最後修改日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
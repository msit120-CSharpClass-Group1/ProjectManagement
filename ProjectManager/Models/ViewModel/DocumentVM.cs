using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class DocumentVM
    {
        public System.Guid DocumentGUID { get; set; }
        public Nullable<System.Guid> ProjectGUID { get; set; }
        public Nullable<System.Guid> TaskGUID { get; set; }
        [DisplayName("文件編號")]
        public int DocumentID { get; set; }
        [DisplayName("文件名稱")]
        public string DocumentName { get; set; }
        [DisplayName("文件類別")]
        public string DocumentCategory { get; set; }
        public Nullable<int> CategoryID { get; set; }
        [DisplayName("備註")]
        public string Description { get; set; }
        [DisplayName("建立日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DisplayName("建立人員")]
        public string CreateEmpName { get; set; }
        public Nullable<System.Guid> CreateEmpGUID { get; set; }
        [DisplayName("最後修改日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("最後修改人員")]
        public string ModifiedEmpName{ get; set; }
        public Nullable<System.Guid> ModifiedEmpGUID { get; set; }
    }
}
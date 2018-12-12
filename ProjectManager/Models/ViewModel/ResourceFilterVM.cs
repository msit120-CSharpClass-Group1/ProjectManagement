using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class ResourceFilterVM
    {
        public DateTime filter_StartDate { get; set; }
        public DateTime filter_EndDate { get; set; }
        public Nullable<Guid> filter_TaskGUID { get; set; }
        public Nullable<int> filter_CategoryID { get; set; }
    }
}
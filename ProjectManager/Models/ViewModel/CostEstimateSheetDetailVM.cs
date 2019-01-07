using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class CostEstimateSheetDetailVM
    {
        public System.Guid DetailGUID { get; set; }
        public int DetailID { get; set; }
        public Nullable<System.Guid> SheetGUID { get; set; }
        public Nullable<System.Guid> TaskGUID { get; set; }
        public string TaskName { get; set; }
        public Nullable<int> ResourceCategoryID { get; set; }
        public string ResourceCategoryName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public IEnumerable<CostEstimateSheetDetailVM> group { get; set; }
    }
}
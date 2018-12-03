using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class ChartDataVM
    {
        
    }

    public class ChartData
    {
        public ChartData()
        {
            labels = new List<string>();
            datasets = new List<ChartDataset>();
        }
        /// <summary>
        /// 個別資料名稱
        /// </summary>
        public List<string> labels { get; set; }
        public List<ChartDataset> datasets { get; set; }
    }

    public class ChartDataset
    {
        /// <summary>
        /// 表格資料名稱
        /// </summary>
        public string label { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        /// <summary>
        /// 個別資料數值
        /// </summary>
        public IEnumerable<int> data { get; set; }
    }
}
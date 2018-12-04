using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class ChartDataVM
    {
        
    }

    public class ChartData<T> where T:class
    {
        public ChartData()
        {
            labels = new List<string>();
            datasets = new List<T>();
        }
        /// <summary>
        /// 個別資料名稱
        /// </summary>
        public List<string> labels { get; set; }
        public List<T> datasets { get; set; }
    }

    public class BarChartDataset
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
    public class PieChartDataset
    {
        /// <summary>
        /// 表格資料名稱
        /// </summary>
        public string label { get; set; }
        public IEnumerable<string> backgroundColor { get; set; }
        public IEnumerable<string> borderColor { get; set; }
        /// <summary>
        /// 個別資料數值
        /// </summary>
        public IEnumerable<int> data { get; set; }
    }
}
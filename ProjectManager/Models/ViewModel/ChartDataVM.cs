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

    public class SingleColorChartDataset
    {
        /// <summary>
        /// 次要資料表類型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 表格資料名稱
        /// </summary>
        public string label { get; set; }
        public string backgroundColor { get; set; } 
        public string borderColor { get; set; }
        public bool fill { get; set; }
        /// <summary>
        /// 個別資料數值
        /// </summary>
        public IEnumerable<int> data { get; set; }
    }

    public class MultiColorChartDataset
    {
        /// <summary>
        /// 次要資料表類型
        /// </summary>
        public string type { get; set; }
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

    public class SingleColorChartDataset<T> where T:struct
    {
        /// <summary>
        /// 次要資料表類型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 表格資料名稱
        /// </summary>
        public string label { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public bool fill { get; set; }
        /// <summary>
        /// 個別資料數值
        /// </summary>
        public IEnumerable<T> data { get; set; }
    }

    public class MultiColorChartDataset<T> where T : struct
    {
        /// <summary>
        /// 次要資料表類型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 表格資料名稱
        /// </summary>
        public string label { get; set; }
        public IEnumerable<string> backgroundColor { get; set; }
        public IEnumerable<string> borderColor { get; set; }
        /// <summary>
        /// 個別資料數值
        /// </summary>
        public IEnumerable<T> data { get; set; }
    }

}
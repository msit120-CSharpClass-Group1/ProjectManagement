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
        public List<string> labels { get; set; }
        public List<T> datasets { get; set; }
    }

    /// <summary>
    /// T 為data的類型，請填int、double等。
    /// label 表格資料名稱，data 為個別資料數值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleColorChartDataset<T> where T:struct
    {        
        public string label { get; set; }
        public string backgroundColor { get; set; } 
        public string borderColor { get; set; }
        public bool fill { get; set; }
        public IEnumerable<T> data { get; set; }
    }

    /// <summary>
    /// T 為data的類型，請填int、double等。
    /// label 表格資料名稱，data 為個別資料數值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiColorChartDataset<T> where T : struct
    {
        public string label { get; set; }
        public IEnumerable<string> backgroundColor { get; set; }
        public IEnumerable<string> borderColor { get; set; }
        public IEnumerable<T> data { get; set; }
    }

}
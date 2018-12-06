using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class HolidaysVM
    {
        public HolidaysVM()
        {
            result = new MainResult();
        }
        public MainResult result { get; set; }        
    }
    public class MainResult
    {
        public IEnumerable<HolidayResults> results { get; set; }
        public int count { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
    }
    public class HolidayResults
    {
        public int _id { get; set; }
        public string date { get; set; }
        public string isHoliday { get; set; }
        public string name { get; set; }
        public string holidayCategory { get; set; }
        public string description { get; set; }
    }
}
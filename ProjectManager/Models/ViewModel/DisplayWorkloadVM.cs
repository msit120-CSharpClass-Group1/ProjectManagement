using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class DisplayWorkloadVM
    {
        public System.Guid TaskGUID { get; set; }
        public string Description { get; set; }
        public Nullable<int> EstWorkTime { get; set; }
        public Nullable<int> WorkTime { get; set; }
        public int Key { get; set; }
        public Nullable<System.Guid> EmployeeGUID { get; set; }
    }

    public class Group<K, T>
    {
        public K Key;//EmpName
        public IEnumerable<T> value;

        //[DisplayFormat(DataFormatString = "{0:P1}")]
        public int? Sum; //算工時
    }
}
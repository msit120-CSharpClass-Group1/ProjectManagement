using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class Grouped<K,T> where T:class
    {
        public K Key { get; set; }
        public IGrouping<K,T> group { get; set; }
    }
}
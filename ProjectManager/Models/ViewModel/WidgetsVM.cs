using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models.ViewModel
{
    public class WidgetsVM
    {
        public IEnumerable<Widgets> WidgetsColumn1 { get; set; }
        public IEnumerable<Widgets> WidgetsColumn2 { get; set; }
    }
}
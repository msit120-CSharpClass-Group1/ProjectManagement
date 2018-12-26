using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    [MetadataType(typeof(CalendarMetadata))]
    public partial class Calendar
    {
        private class CalendarMetadata
        {
            [JsonIgnore]
            public virtual Members Members { get; set; }
            [JsonIgnore]
            public virtual CalendarCategory CalendarCategory { get; set; }
        }
    }
}
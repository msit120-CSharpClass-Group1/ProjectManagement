using Newtonsoft.Json;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InviteTeamMember.Models
{
    [MetadataType(typeof(EmployeeMetaData))]
    public partial class Employee
    {
        public class EmployeeMetaData
        {
            [JsonIgnore]
            public virtual Department Department { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            [JsonIgnore]
            public virtual ICollection<Employee> Employee1 { get; set; }
            [JsonIgnore]
            public virtual Employee Employee2 { get; set; }
            [JsonIgnore]
            public virtual ICollection<ProjectMembers> ProjectMembers { get; set; }
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Newtonsoft.Json;

namespace ProjectManager.Models
{
    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee
    {
        public class EmployeeMetadata
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
            [JsonIgnore]
            public virtual JobTitle JobTitle { get; set; }
            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Project> Project { get; set; }
            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Project> Project1 { get; set; }
            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Project> Project2 { get; set; }
        }
    }
}
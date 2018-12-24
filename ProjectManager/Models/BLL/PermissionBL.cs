using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class PermissionBL
    {
        public enum Emp_Title
        {
            //According to JobTitle.TitleID
            Admin = 0, PG = 1, Minister = 3, PM = 5, Director = 4,
        }
    }
}
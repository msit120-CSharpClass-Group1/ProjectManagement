using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ProjectManager.Models.ViewModel
{
    public class EmployeeInfo
    {
        public static string EmployeeName
        {
            get
            {
                var httpContext = HttpContext.Current;
                var identity = httpContext.User.Identity as FormsIdentity;

                if (identity == null)
                {
                    return "";
                }
                else
                {
                    var memberGUID = identity.Name;
                    return SystemUserName(memberGUID);
                }
            }
        }

       
        public static string SystemUserName(string memberGUID)
        {
            string userName = "";

            Guid systemUserID;

            if (!Guid.TryParse(memberGUID.ToString(), out systemUserID))
            {
                return userName;
            }
            if (systemUserID.Equals(Guid.Empty))
            {
                userName = "使用者名稱";
            }
            else
            {
                IRepository<Members> members = new Repository<Members>();
                var user = members.Find(new Guid(memberGUID));
                    userName = (user == null) ? string.Empty : user.Employee.EmployeeName;
                
            }
            return userName;
        }
    }
}
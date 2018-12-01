using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace ProjectManager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            bool hasMember = HttpContext.Current.User != null;
            bool isAuthenticated = hasMember && HttpContext.Current.User.Identity.IsAuthenticated;
            bool isIdentity = isAuthenticated && HttpContext.Current.User.Identity is FormsIdentity;
            if (isIdentity)
            {
                FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                string[] roles = new string[] { ticket.UserData };
                HttpContext.Current.User = new GenericPrincipal(id, roles);
            }
        }
    }

}

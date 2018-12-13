using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private IRepository<Members> memberRes = new Repository<Members>();
        private IRepository<Employee> employeeRes = new Repository<Employee>();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Members members, bool keepLogin)
        {
            var hasMembers = memberRes.GetCollections().Where(n => n.MemberID.Trim() == members.MemberID && n.Password == members.Password).FirstOrDefault();
            int Msg = 0;
            if (hasMembers != null)
            {
                Response.Cookies["MemberGUID"].Value = hasMembers.MemberGUID.ToString();
                Response.Cookies["TitleGUID"].Value = hasMembers.Employee.TitleGUID.ToString();
                if (keepLogin)
                {
                    Response.Cookies["MemberGUID"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies["TitleGUID"].Expires = DateTime.Now.AddDays(1);
                }
                hasMembers.LastLoginDate = DateTime.Now;
                memberRes.Update(hasMembers);
                Msg = 1;
                LoginProcess(hasMembers, keepLogin);
            }
                return Json(Msg);
        }
        [HttpPost]
        public ActionResult CreateAccount(Members members,int EmployeeID)
        {
            var allMember = memberRes.GetCollections();
            Employee hasEmployeeID = employeeRes.GetCollections().Where(n => n.EmployeeID == EmployeeID).FirstOrDefault();
            Members hasMembers = allMember.Where(n => n.MemberID.Trim() == members.MemberID).FirstOrDefault() ;
            Members hasAccount =null;
            if (hasEmployeeID != null)
            {
                hasAccount = allMember.Where(n => n.EmployeeGUID == hasEmployeeID.EmployeeGUID).FirstOrDefault();
            }
            string memberMsg = "0";
            if (hasEmployeeID != null && hasMembers == null&& hasAccount==null)
            {
                members.EmployeeGUID = hasEmployeeID.EmployeeGUID;
                members.MemberGUID = Guid.NewGuid();
                members.CreateDate = DateTime.Now;
                members.ModifiedDate = DateTime.Now;
                memberRes.Add(members);
                memberMsg = "1";
            }
            return Json(memberMsg);
        }
        public ActionResult Logout()
        {
            Response.Cookies["MemberGUID"].Expires = DateTime.Now.AddSeconds(-1);
            Response.Cookies["TitleGUID"].Expires = DateTime.Now.AddSeconds(-1);
            Response.Cookies["ProjectGUID"].Expires = DateTime.Now.AddSeconds(-1);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        private void LoginProcess(Members members, bool keepLogin)
        {
            var ticket = new FormsAuthenticationTicket(
                version: 1,
                name: members.MemberGUID.ToString(),
                issueDate: DateTime.Now,
                expiration: DateTime.Now.AddDays(1),
                isPersistent: keepLogin,
                userData: members.Employee.JobTitle.TitleName,
                cookiePath: FormsAuthentication.FormsCookiePath);
           
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName,encryptedTicket);
            if (keepLogin)
            {
                cookie.Expires = DateTime.Now.AddDays(1);
            }
            Response.Cookies.Add(cookie);
        }

    }
}
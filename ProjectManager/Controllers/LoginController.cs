using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            if (Request.Cookies["MemberGUID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(Members members, string keepLogin)
        {
            var hasMembers = memberRes.GetCollections().Where(n => n.MemberID.Trim() == members.MemberID && n.Password == members.Password).FirstOrDefault();
            int Msg = 0;
            if (hasMembers != null)
            {
                Response.Cookies["MemberGUID"].Value = hasMembers.MemberGUID.ToString();
                Response.Cookies["MemberID"].Value = hasMembers.MemberID;
                Response.Cookies["TitleGUID"].Value = hasMembers.Employee.TitleGUID.ToString();
                if (keepLogin != null)
                {
                    Response.Cookies["MemberGUID"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies["MemberID"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies["TitleGUID"].Expires = DateTime.Now.AddDays(1);
                }
                hasMembers.LastLoginDate = DateTime.Now;
                memberRes.Update(hasMembers);
                Msg = 1;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Json(Msg);
            }
            
        }
        [HttpPost]
        public ActionResult CreateAccount(Members members,int EmployeeID)
        {
            var allMember = memberRes.GetCollections();
            var hasMembers = allMember.Where(n => n.MemberID.Trim() == members.MemberID).FirstOrDefault();
            var hasEmployeeID = employeeRes.GetCollections().Where(n=>n.EmployeeID == EmployeeID).FirstOrDefault();
            var hasAccount = allMember.Where(n => n.EmployeeGUID == hasEmployeeID.EmployeeGUID).FirstOrDefault();
            string memberMsg = "0";
            if (hasMembers == null&& hasEmployeeID!=null && hasAccount==null)
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
            Session.Abandon();
            Response.Cookies["MemberGUID"].Expires = DateTime.Now.AddSeconds(-1);
            Response.Cookies["MemberID"].Expires = DateTime.Now.AddSeconds(-1);
            Response.Cookies["TitleGUID"].Expires = DateTime.Now.AddSeconds(-1);
            return RedirectToAction("Index", "Login");
        }

    }
}
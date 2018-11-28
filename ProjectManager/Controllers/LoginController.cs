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
            ViewBag.msg = "帳號密碼錯誤";
            if (hasMembers != null)
            {
                Response.Cookies["MemberGUID"].Value = hasMembers.MemberGUID.ToString() ;
                Session["MemberGUID"] = hasMembers.MemberGUID;
                if (keepLogin!=null)
                {
                    Response.Cookies["MemberGUID"].Expires = DateTime.Now.AddDays(1);
                }
                return RedirectToAction("Index", "Home");
            }
            return View("Index");
        }
        [HttpPost]
        public ActionResult CreateAccount(Members members)
        {
            var hasMembers = memberRes.GetCollections().Where(n => n.MemberID.Trim() == members.MemberID).FirstOrDefault();
            string memberMsg = "0";
            if (hasMembers == null)
            {

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
            return RedirectToAction("Index", "Login");
        }

    }
}
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
        IRepository<Members> memberRes = new Repository<Members>();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateAccount(Members members)
        {
            var hasMembers = memberRes.GetCollections().Where(n => n.MemberID.Trim() == members.MemberID).FirstOrDefault();
            string memberMsg = "已有帳號";
            members.MemberGUID = Guid.NewGuid();
            if (hasMembers == null)
            {
                memberRes.Add(members);
                memberMsg = "註冊成功";
            }
            return Json(memberMsg);
        }
    }
}
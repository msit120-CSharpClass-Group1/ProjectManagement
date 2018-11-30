using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    public class MembersController : Controller
    {
        private IRepository<Members> memberRepository = new Repository<Members>();
        private IRepository<Permissions> permissionsRepository = new Repository<Permissions>();
        ProjectManagementEntities db = new ProjectManagementEntities();
        public ActionResult Index()
        {
          
            ViewBag.members = memberRepository.GetCollections().ToList();

            return View();
        }

        public ActionResult Reset(Guid id)
        {
            var member =  memberRepository.Find(id);
            member.Password = member.MemberID;
            memberRepository.Update(member);
            ViewBag.msg = "重設成功";
            ViewBag.members = memberRepository.GetCollections().ToList();
            return View("Index");
        }
        public ActionResult Edit()
        {
            if (Request.Cookies["MemberGUID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.id= Request.Cookies["MemberGUID"].Value;
            return View();
        }
        [HttpPost]
        public ActionResult _Edit(Members members,string NewPassword,string RepeatPassword)
        {
            ViewBag.msg = "密碼不符";
            ViewBag.id = members.MemberGUID;
            if (NewPassword != RepeatPassword)
            {
                return View("Edit");
            }
            else
            {
                var member = memberRepository.Find(members.MemberGUID);
                if (members.Password != member.Password)
                {
                    return View("Edit");
                }
                else
                {
                    member.Password = NewPassword;
                    memberRepository.Update(member);
                    ViewBag.msg = "修改成功";
                    return View("Edit");
                }
            }
            
        }
    }
}
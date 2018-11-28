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
         
            return View(memberRepository.GetCollections());
        }

        //public ActionResult Insert(Members _members)
        //{
        //    if(Request.Form.Count>0)
        //    {
        //        _members.MemberGUID = Guid.NewGuid();
        //        _members.CreateDate = DateTime.Now;
        //        memberRepository.Create(_members);

        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            return View(permissionsRepository.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(Members members)
        {
            var empolyees = db.Employee.Select(c => new
            {
                c.EmployeeGUID,
                c.EmployeeName
            });

            ViewBag.employee = new SelectList(empolyees, "EmployeeGUID", "EmployeeName");


            var permissions = db.Permissions.Select(g => new
            {
                g.PermissionsGUID,
                g.PermissionsName
            });

            ViewBag.permissions = new SelectList(permissions, "PermissionsGUID", "PermissionsName");
            Members _members = memberRepository.Find(members.MemberGUID);
            _members.ModifiedDate = DateTime.Now;
            memberRepository.Update(_members);

            return RedirectToAction("Index");
        }



        public ActionResult Delete(Guid? id)
        {


            Members _members = memberRepository.Find(id);
     

            memberRepository.Delete(_members);

            return RedirectToAction("Index");
        }

        public ActionResult EditPermisson(Members _member)
        {

            return View();
        }

    }
}
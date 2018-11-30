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
        private IRepository<Employee> empRepository = new Repository<Employee>();
        ProjectManagementEntities db = new ProjectManagementEntities();
        public ActionResult Index()
        {       
            ViewBag.members = memberRepository.GetCollections().ToList();
            return View(memberRepository.GetCollections());
        }

        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            ViewBag.PermissonName = permissionsRepository.GetCollections().ToList();
            ViewBag.EmployeeName = empRepository.GetCollections().ToList();          
            return View(memberRepository.Find(id));
        }

        public ActionResult LoadPermissionsGUID(string PermissionsName)
        {
            var GUID = permissionsRepository.GetCollections().Where(p => p.PermissionsName == PermissionsName).FirstOrDefault().PermissionsGUID;
            return Content(GUID.ToString());
            
        }

        //public ActionResult LoadEmpolyeeGUID(string EmployeeName)
        //{
        //    var GUID2 = empRepository.GetCollections().Where(p => p.EmployeeName == EmployeeName).FirstOrDefault().EmployeeGUID;
        //    return Content(GUID2.ToString());
        //}


        [HttpPost]
        public ActionResult Edit(Members members)
        {
            var MName = Request.Form["members"];
            var perID = members.PermissionsGUID;
            var empID = members.EmployeeGUID;
            Members _members = memberRepository.Find(new Guid(MName));
            _members.ModifiedDate = DateTime.Now;
            _members.PermissionsGUID = perID;
            _members.EmployeeGUID = empID;
            memberRepository.Update(_members);
            return RedirectToAction("Index");
        }


        public ActionResult Delete(Guid? id)
        {


            Members _members = memberRepository.Find(id);
     

            memberRepository.Delete(_members);

            return RedirectToAction("Index");
        }
    }
}
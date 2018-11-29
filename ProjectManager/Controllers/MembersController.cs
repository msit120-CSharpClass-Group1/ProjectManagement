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

            return View(memberRepository.GetCollections());
        }

        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            ViewBag.PermissonName = permissionsRepository.GetCollections().ToList();
            return View(memberRepository.Find(id));
        }

        public ActionResult LoadMemberGUID(string PermissionsName)
        {
            var GUID = permissionsRepository.GetCollections().Where(p => p.PermissionsName == PermissionsName).FirstOrDefault().PermissionsGUID;
            return Content(GUID.ToString());
            
        }

        [HttpPost]
        public ActionResult Edit(Members members)
        {
            var MName = Request.Form["members"];
            var pgID = Request.Form["PermissionsGUID"];
            Members _members = memberRepository.Find(new Guid(MName));
            _members.ModifiedDate = DateTime.Now;
            _members.PermissionsGUID = new Guid(pgID);
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
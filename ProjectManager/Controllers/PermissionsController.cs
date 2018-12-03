using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員")]
    public class PermissionsController : Controller
    {
        private IRepository<Permissions> permissionsRepository = new Repository<Permissions>();
        public ActionResult Index()
        {
            return View(permissionsRepository.GetCollections().OrderBy(c=>c.PermissionsID));
        }

        public ActionResult Insert(Permissions _permissions)
        {
            if (Request.Form.Count > 0) 
            {
                _permissions.PermissionsGUID = Guid.NewGuid();
                permissionsRepository.Add(_permissions);

                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit(Guid? id)
        {

            return View(permissionsRepository.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(Permissions permissions)
        {
            Permissions _permissions = permissionsRepository.Find(permissions.PermissionsGUID);
            _permissions.PermissionsID = permissions.PermissionsID;
            _permissions.PermissionsName = permissions.PermissionsName;

            permissionsRepository.Update(_permissions);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid? id)
        {

            Permissions _permissions = permissionsRepository.Find(id);
            permissionsRepository.Delete(_permissions);

            return RedirectToAction("Index");
        }

    }
}
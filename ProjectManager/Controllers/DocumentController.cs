using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        IRepository<Document> Doc = new Repository<Document>();
        IRepository<DocumentCategory> DocCategory = new Repository<DocumentCategory>();
        IRepository<DocumentModified> DocModified = new Repository<DocumentModified>();
        IRepository<Members> members = new Repository<Members>();
        // GET: Document
        public ActionResult Index()
        {
            ViewBag.DocCategory = new SelectList(DocCategory.GetCollections(), "CategoryID", "CategoryName");
            return View(DocCategory.GetCollections());
        }

        public ActionResult GetDocument(int id, int? page=1)
        {
            ViewBag.Category = id;
            List<DocumentVM> Docs = new List<DocumentVM>();
            foreach (var doc in Doc.GetCollections().Where(n=>n.DocumentCategory == id))
            {
                DocumentVM vm = new DocumentVM();
                vm.DocumentGUID = doc.DocumentGUID;
                vm.ProjectGUID = doc.ProjectGUID;
                vm.TaskGUID = doc.TaskGUID;
                vm.DocumentID = doc.DocumentID;
                vm.DocumentName = doc.DocumentName;
                vm.DocumentCategory = doc.DocumentCategory1.CategoryName;
                vm.CategoryID = doc.DocumentCategory;
                vm.Description = doc.Description;
                vm.CreateDate = doc.CreateDate;
                vm.CreateEmpName = doc.Employee.EmployeeName;
                vm.CreateEmpGUID = doc.CreateEmpGUID;
                var modified = doc.DocumentModified.OrderByDescending(n => n.ModifiedDate).FirstOrDefault();
                vm.ModifiedDate = modified.ModifiedDate;
                vm.ModifiedEmpName = modified.Employee.EmployeeName;
                vm.ModifiedEmpGUID = modified.ModifiedEmpGUID;
                //if ((bool)modified.IsChecked)
                //{
                //    Docs.Add(vm);
                //}
                Docs.Add(vm);
            }
            return PartialView(Docs.ToPagedList(page ?? 1, 8));
        }
        public ActionResult AddFile(HttpPostedFileBase file, Document _doc)
        {
            if (file != null)
            {
                if (file.ContentLength >= 0)
                {
                    
                    DocumentModified _docm = new DocumentModified();
                    Members _members = members.Find(new Guid(Request.Cookies["MemberGUID"].Value));
                    int lastDocID = 0;
                    var _lastDocID = Doc.GetCollections().Where(n => n.DocumentCategory == _doc.DocumentCategory);
                    if (_lastDocID.Count() != 0)
                    {
                        lastDocID = _lastDocID.Select(n => n.DocumentID).Max();
                    }
                    _doc.DocumentID = lastDocID + 1;
                    _doc.DocumentGUID = Guid.NewGuid();
                    _doc.CreateEmpGUID = _members.EmployeeGUID;
                    _doc.CreateDate = DateTime.Now;
                    Doc.Add(_doc);
                    _docm.DocumentGUID = _doc.DocumentGUID;
                    _docm.IsChecked = false;
                    _docm.ModifiedEmpGUID = _members.EmployeeGUID;
                    _docm.ModifiedDate = DateTime.Now;
                    _docm.Version = 1;
                    var fileExtension = System.IO.Path.GetExtension(file.FileName);
                    _docm.FileName = _doc.DocumentName + "_" + _docm.Version + fileExtension;
                    _docm.ModifiedGUID = Guid.NewGuid();
                    DocModified.Add(_docm);
                    var fileName = _docm.FileName;
                    var path = Path.Combine(Server.MapPath("/Document/" + _doc.DocumentCategory), fileName);
                    file.SaveAs(path);
                }
            }
            return RedirectToAction("/Index");
        }
        public ActionResult DownLoad(Guid id)
        {
            var _doc = Doc.Find(id);
            var file = DocModified.GetCollections().Where(n => n.DocumentGUID == id).OrderByDescending(n => n.Version).First();
            var filePath = Server.MapPath("~/Document/" + _doc.DocumentCategory + "/" + file.FileName);
            var fileName = System.IO.Path.GetFileName(filePath);
            Stream iStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream,System.IO.Path.GetExtension(filePath),fileName);
        }
        public ActionResult Modified(Guid id)
        {
            ViewBag.doc = Doc.Find(id);
            var docm = DocModified.GetCollections().Where(n => n.DocumentGUID == id);
            return View(docm);
        }
        public ActionResult MDownLoad(Guid id)
        {
            var _docm = DocModified.Find(id);
            var doc = Doc.Find(_docm.DocumentGUID);
            var filePath = Server.MapPath("~/Document/" + doc.DocumentCategory + "/" + _docm.FileName);
            var fileName = System.IO.Path.GetFileName(filePath);
            Stream iStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, System.IO.Path.GetExtension(filePath), fileName);
        }
        public ActionResult Edit(HttpPostedFileBase file, Document _doc)
        {
            if (file != null)
            {
                if (file.ContentLength >= 0)
                {

                    DocumentModified _docm = new DocumentModified();
                    Members _members = members.Find(new Guid(Request.Cookies["MemberGUID"].Value));
                    Doc.Update(_doc);
                    _docm.DocumentGUID = _doc.DocumentGUID;
                    _docm.IsChecked = false;
                    _docm.ModifiedEmpGUID = _members.EmployeeGUID;
                    _docm.ModifiedDate = DateTime.Now;

                    int? lastVersion = 0;
                    var _lastDocID = DocModified.GetCollections().Where(n => n.DocumentGUID == _doc.DocumentGUID);
                    if (_lastDocID.Count() != 0)
                    {
                        lastVersion = _lastDocID.Select(n => n.Version).Max();
                    }
                    _docm.Version = lastVersion +1;
                    var fileExtension = System.IO.Path.GetExtension(file.FileName);
                    _docm.FileName = _doc.DocumentName + "_" + _docm.Version + fileExtension;
                    _docm.ModifiedGUID = Guid.NewGuid();
                    DocModified.Add(_docm);
                    var fileName = _docm.FileName;
                    var path = Path.Combine(Server.MapPath("/Document/" + _doc.DocumentCategory), fileName);
                    file.SaveAs(path);
                }
            }
            return RedirectToAction("/Index");
        }
    }
}
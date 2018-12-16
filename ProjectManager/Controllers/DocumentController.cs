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
                Docs.Add(vm);
            }
            return PartialView(Docs.ToPagedList(page ?? 1, 8));
        }
        public ActionResult AddFile(HttpPostedFileBase file, Document _doc)
        {
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    
                    DocumentModified _docm = new DocumentModified();
                    Members _members = members.Find(new Guid(Request.Cookies["MemberGUID"].Value));
                    int lastDocID = Doc.GetCollections().Where(n => n.DocumentCategory == _doc.DocumentCategory).Select(n => n.DocumentID).Max();
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
                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = _docm.FileName;
                    var path = Path.Combine(Server.MapPath("/Document/" + _doc.DocumentCategory), fileName);
                    file.SaveAs(path);
                }
            }
            return RedirectToAction("/Index");
        }
    }
}
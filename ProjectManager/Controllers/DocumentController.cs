using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
using ProjectManager.Models.BLL;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        IRepository<Document> Doc = new Repository<Document>();
        IRepository<DocumentCategory> DocCategory = new Repository<DocumentCategory>();
        IRepository<DocumentModified> DocModified = new Repository<DocumentModified>();
        IRepository<Members> members = new Repository<Members>();
        IRepository<Project> projects = new Repository<Project>();
        IRepository<ProjectMembers> projectMembers = new Repository<ProjectMembers>();
        IRepository<Tasks> tasks = new Repository<Tasks>();
        // GET: Document
        public ActionResult Index()
        {
            ViewBag.DocCategory = new SelectList(DocCategory.GetCollections(), "CategoryID", "CategoryName");
            return View(DocCategory.GetCollections());
        }

        public ActionResult GetDocument(int id, int? page=1)
        {
            ViewBag.Category = id;
            var memberGuid = new Guid(Request.Cookies["MemberGUID"].Value);
            var employeeGuid = members.Find(memberGuid).EmployeeGUID;
            List<DocumentVM> Docs =null;
            switch (id)
            {
                case 1:
                    Docs = id.GetDocumentsByCategory();
                    break;
                default:
                    var _projectEmployee =projectMembers.GetCollections().Where(n => n.EmployeeGUID == employeeGuid).Select(n=>n.ProjectGUID).ToList();
                    var _projectManger = projects.GetCollections().Where(n => n.InChargeDeptPMGUID == employeeGuid).Select(n => n.ProjectGUID).ToList();
                    var _project = _projectEmployee.Union(_projectManger).ToList();
                    List<DocumentVM> _Docs = new List<DocumentVM>();
                    foreach (var p in _project)
                    {
                        var q =id.GetDocumentsByCategory().Where(n => n.ProjectGUID == p).ToList();
                        _Docs.AddRange(q);
                    }
                    Docs = _Docs;
                    break;
               
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
            var docm = DocModified.GetCollections().Where(n => n.DocumentGUID == id).OrderBy(n=>n.Version);
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
                    var doc = Doc.Find(_doc.DocumentGUID);                   
                    doc.Description = _doc.Description;
                    Doc.Update(doc);                    
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
                    _docm.FileName = doc.DocumentName + "_" + _docm.Version + fileExtension;
                    _docm.ModifiedGUID = Guid.NewGuid();
                    DocModified.Add(_docm);
                    var fileName = _docm.FileName;
                    var path = Path.Combine(Server.MapPath("/Document/" + doc.DocumentCategory), fileName);
                    file.SaveAs(path);
                }
            }
            return RedirectToAction("/Modified/"+ _doc.DocumentGUID);
        }

        public ActionResult Delete()
        {
            var vm =DocumentBL.GetDocuments();
            return View(vm);
        }

        public ActionResult _Delete(Guid id)
        {
            var doc = Doc.Find(id);
            var mdoc =DocModified.GetCollections().Where(n => n.DocumentGUID == id).ToList();
            foreach (var _mdoc in mdoc)
            {
                DocModified.Delete(_mdoc);
            }
            Doc.Delete(doc);
            return RedirectToAction("/Delete");
        }
        public ActionResult MDelete(Guid id)
        {
            ViewBag.doc = Doc.Find(id);
            var docm = DocModified.GetCollections().Where(n => n.DocumentGUID == id).OrderBy(n => n.Version);
            return View(docm);
        }
        public ActionResult _MDelete(Guid id)
        {
            var docm = DocModified.Find(id);
            Guid _docGuid = docm.DocumentGUID;
            var doc = DocModified.GetCollections().Where(n => n.DocumentGUID == _docGuid).Count();
            if (doc>1)
            {
                DocModified.Delete(docm);
            }
            return RedirectToAction("/MDelete/"+ _docGuid);
        }
        public ActionResult GetProjectList()
        {
            var member = new Guid(Request.Cookies["MemberGUID"].Value);
            var employeeGuid = members.Find(member).EmployeeGUID;
            var _GetProjectList= projectMembers.GetCollections().Where(n => n.EmployeeGUID == employeeGuid).Select(n => new { n.Project.ProjectName, n.ProjectGUID }).ToList();
            return Json(_GetProjectList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTaskList(Guid id)
        {
            var member = new Guid(Request.Cookies["MemberGUID"].Value);
            var employeeGuid = members.Find(member).EmployeeGUID;
            var _GetTaskList = tasks.GetCollections().Where(n => n.EmployeeGUID == employeeGuid&& n.ProjectGUID==id).Select(n => new { n.TaskName, n.TaskGUID }).ToList();
            return Json(_GetTaskList, JsonRequestBehavior.AllowGet);
        }
    }
}
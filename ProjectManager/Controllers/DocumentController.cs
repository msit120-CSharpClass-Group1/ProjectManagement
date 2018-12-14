using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using PagedList;
using PagedList.Mvc;
namespace ProjectManager.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        IRepository<Document> Doc = new Repository<Document>();
        IRepository<DocumentCategory> DocCategory = new Repository<DocumentCategory>();
        IRepository<DocumentModified> DocModified = new Repository<DocumentModified>();
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models.BLL
{
    public static class DocumentBL
    {
        public static List<DocumentVM> GetDocumentsByCategory(this int DocCategory)
        {
            IRepository<Document> Doc = new Repository<Document>();
            List<DocumentVM> Docs = new List<DocumentVM>();
            foreach (var doc in Doc.GetCollections().Where(n => n.DocumentCategory == DocCategory))
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
            return Docs.OrderBy(n => n.DocumentID).ToList();

        }
        public static List<DocumentVM> GetDocuments()
        {
            IRepository<Document> Doc = new Repository<Document>();
            List<DocumentVM> Docs = new List<DocumentVM>();
            foreach (var doc in Doc.GetCollections())
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
            return Docs.OrderBy(n => n.DocumentID).ToList();

        }
    }
}
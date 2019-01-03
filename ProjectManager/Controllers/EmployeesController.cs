using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員")]
    public class EmployeesController : Controller
    {
        // GET: Employees
        private IRepository<Members> memberRepository = new Repository<Members>();
        private IRepository<Employee> employeeRepository = new Repository<Employee>();
        private IRepository<Department> departmentRepository = new Repository<Department>();
        private IRepository<JobTitle> jobTitleRepository = new Repository<JobTitle>();


        public ActionResult Index()
        {
            ViewBag.employees = employeeRepository.GetCollections().OrderBy(n=>n.EmployeeID).ToList();
            return View();
        }
        public ActionResult Add()
        {
            ViewBag.employees = employeeRepository.GetCollections().ToList();
            ViewBag.department = departmentRepository.GetCollections().ToList();
            ViewBag.jobTitle = jobTitleRepository.GetCollections().ToList();
            return View();
        }
        public ActionResult _Add(Employee employee)
        {
            var addEmployeeID = employeeRepository.GetCollections().Select(n => n.EmployeeID).Max();
            if (addEmployeeID < 100000)
            {
                addEmployeeID += 100001;
            }
            else
            {
                addEmployeeID++;
            }
            employee.EmployeeGUID = Guid.NewGuid();
            employee.EmployeeID = addEmployeeID;
            employeeRepository.Add(employee);
            return RedirectToAction("Index", "Employees");
        }
        public ActionResult Edit(Guid id)
        {
            ViewBag.editemployee = employeeRepository.Find(id);
            ViewBag.editmember = memberRepository.GetCollections().Where(n => n.EmployeeGUID == id).FirstOrDefault();
            ViewBag.employees = employeeRepository.GetCollections().ToList();
            ViewBag.department = departmentRepository.GetCollections().ToList();
            ViewBag.jobTitle = jobTitleRepository.GetCollections().ToList();
            return View();
        }
        public ActionResult _Edit(Employee employee)
        {
            employeeRepository.Update(employee);
            return RedirectToAction("Index", "Employees");
        }
        public ActionResult Delete(Guid id)
        {
            Employee employee = employeeRepository.Find(id);
            Members members = memberRepository.GetCollections().Where(n => n.EmployeeGUID == id).FirstOrDefault();
            if (members != null)
            {
                memberRepository.Delete(members);
            }
            employeeRepository.Delete(employee);
            return RedirectToAction("Index", "Employees");
        }
    }
}
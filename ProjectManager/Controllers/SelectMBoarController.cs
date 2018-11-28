using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    
    public class SelectMBoarController : Controller
    {
        Repository<ProjectManager.Models.Employee> f = new Repository<ProjectManager.Models.Employee>();
        // GET: SelectMBoar
        public ActionResult Index(Guid id)
        {
            var MenberBoard = f.GetCollections();
            Response.Cookies["PID"].Value = id.ToString();
            return View(MenberBoard);
          
        }
    }
}
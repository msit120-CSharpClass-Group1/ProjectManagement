using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    //[Authorize(Roles = "管理員,工程師")]
    [Authorize]
    public class HomeController : Controller
    {
        Repository<Tasks> taskRepo = new Repository<Tasks>();
        
        public ActionResult Index()
        {
            return View();
        }
    }
}
﻿using System;
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
        // GET: Home
        
        public ActionResult Index()
        {
            return View();
        }
    }
}
﻿using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManager.Controllers
{
    [Authorize]
    public class SelectMBoardController : Controller
    {
        Repository<ProjectManager.Models.ProjectMembers> pm = new Repository<ProjectManager.Models.ProjectMembers>();
        Repository<ProjectManager.Models.Project> p = new Repository<ProjectManager.Models.Project>();
        // GET: SelectMBoard
        public ActionResult Index(Guid id)
        {
            var MenberBoard = pm.GetCollections().Where(x => x.ProjectGUID == id);
            Response.Cookies["PID"].Value = id.ToString();
            return View(MenberBoard);
          
        }
    }
}
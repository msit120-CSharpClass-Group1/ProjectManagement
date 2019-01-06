using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using PagedList;
using PagedList.Mvc;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace ProjectManager.Controllers
{
    [Authorize(Roles = "管理員,專案經理,處長")]
    public class CostController : Controller
    {
        Repository<ResourceCategory> ResourceCatRepo = new Repository<ResourceCategory>();
        Repository<TaskResource> ResourceRepo = new Repository<TaskResource>();
        Repository<TaskStatus> StatusRepo = new Repository<TaskStatus>();
        Repository<Department> DptRepo = new Repository<Department>();
        Repository<Project> ProjectRepo = new Repository<Project>();
        Repository<Tasks> TaskRepo = new Repository<Tasks>();
        Repository<CostPool> PoolRepo = new Repository<CostPool>();
        Repository<CostEstimateSheet> SheetRepo = new Repository<CostEstimateSheet>();
        Repository<CostEstimateSheetDetail> SheetDetailRepo = new Repository<CostEstimateSheetDetail>();

        #region Action For ExpList
        public ActionResult ExpList()
        {
            ViewBag.Departments = (from d in DptRepo.GetCollections()
                                   join p in ProjectRepo.GetCollections() on d.DepartmentGUID equals p.RequiredDeptGUID
                                   select d).Distinct().ToList();

            ViewBag.ExpCats = new SelectList(ResourceCatRepo.GetCollections(), "CategoryID", "CategoryName");

            return View();
        }

        public ActionResult GetProjectListByDptID(Guid DepartmentID)
        {
            List<Project> ProjectList = ProjectRepo.GetCollections()
                                                   .Where(p => p.RequiredDeptGUID == DepartmentID)
                                                   .Select(p => new Project
                                                   {
                                                       ProjectGUID = p.ProjectGUID,
                                                       ProjectID = p.ProjectID,
                                                       ProjectName = p.ProjectName
                                                   })
                                                   .ToList();

            return Content(JsonConvert.SerializeObject(ProjectList), "application/json");
        }

        public ActionResult GetTaskListByProjectGuid(Guid ProjectGuid)
        {
            List<Tasks> TaskList = TaskRepo.GetCollections()
                                           .Where(t => t.ProjectGUID == ProjectGuid)
                                           .GetLeafTasks()
                                           .Select(t => new Tasks
                                           {
                                               TaskGUID = t.TaskGUID,
                                               TaskName = t.TaskName
                                           })
                                           .ToList();

            return Content(JsonConvert.SerializeObject(TaskList), "application/json");
        }

        public ActionResult GetTaskResources(Guid? DepartmentID, Guid? ProjectID, int? page, string sortBy, ResourceFilterVM filterBy)
        {
            IEnumerable<Project> projects;

            if (DepartmentID == null && ProjectID == null)
            {
                projects = ProjectRepo.GetCollections();

            }
            else if (ProjectID != null)
            {
                projects = ProjectRepo.GetCollections().Where(p => p.ProjectGUID == ProjectID);
            }
            else
            {
                projects = ProjectRepo.GetCollections().Where(p => p.RequiredDeptGUID == DepartmentID);
            }

            var q = from p in projects
                    join t in TaskRepo.GetCollections() on p.ProjectGUID equals t.ProjectGUID
                    join tr in ResourceRepo.GetCollections() on t.TaskGUID equals tr.TaskGUID
                    join c in ResourceCatRepo.GetCollections() on tr.CategoryID equals c.CategoryID
                    select new ProjectResourceVM
                    {
                        ProjectGUID = p.ProjectGUID,
                        ProjectName = p.ProjectName,
                        TaskGUID = t.TaskGUID,
                        TaskName = t.TaskName,
                        ResourceGUID = tr.ResourceGUID,
                        ResourceID = tr.ResourceID,
                        ResourceName = tr.ResourceName,
                        CategoryID = c.CategoryID,
                        CategoryName = c.CategoryName,
                        Quantity = tr.Quantity,
                        Unit = tr.Unit,
                        UnitPrice = tr.UnitPrice,
                        SubTotal = (tr.UnitPrice * tr.Quantity),
                        Date = tr.Date,
                        Description = tr.Description
                    };

            var ProjectResourceList = q.ToList().AsQueryable().Sort(sortBy).Filter(filterBy);

            Response.Cookies["sortBy"].Value = sortBy;
            ViewBag.Count = ProjectResourceList.Count();
            ViewBag.sortByDate = string.IsNullOrEmpty(sortBy) ? "DateDesc" : "";
            ViewBag.sortByProjectName = sortBy == "ProjectName" ? "ProjectNameDesc" : "ProjectName";
            ViewBag.sortByTaskName = sortBy == "TaskName" ? "TaskNameDesc" : "TaskName";
            ViewBag.sortByResourceName = sortBy == "ResourceName" ? "ResourceNameDesc" : "ResourceName";
            ViewBag.sortByResourceCat = sortBy == "ResourceCat" ? "ResourceCatDesc" : "ResourceCat";
            ViewBag.sortByQuantity = sortBy == "Quantity" ? "QuantityDesc" : "Quantity";
            ViewBag.sortByUnitPrice = sortBy == "UnitPrice" ? "UnitPriceDesc" : "UnitPrice";
            ViewBag.sortBySubtotal = sortBy == "SubTotal" ? "SubTotalDesc" : "SubTotal";
            ViewBag.sortByDescription = sortBy == "Description" ? "DescriptionDesc" : "Description";

            return PartialView(ProjectResourceList.ToPagedList(page ?? 1, 8));
        }

        public ActionResult AddTaskResource(TaskResource resource)
        {
            if (resource.ResourceName != null)
            {
                resource.ResourceGUID = Guid.NewGuid();
                ResourceRepo.Add(resource);
            }

            return RedirectToAction("ExpList");
        }
        
        public ActionResult UpdateTaskResource(TaskResource resource)
        {
            if (resource.ResourceName != null)
            {
                ResourceRepo.Update(resource);
            }

            return RedirectToAction("ExpList");
        }

        public ActionResult DeleteTaskResource(Guid? id)
        {
            if (id != null)
            {
                ResourceRepo.Delete(ResourceRepo.Find(id));
            }

            return RedirectToAction("ExpList");
        }

        public ActionResult ExportAsExcel(FormCollection form, Guid? DepartmentID, Guid? ProjectID, string sortBy, ResourceFilterVM filterBy)
        {
            IEnumerable<Project> projects;

            if (DepartmentID == null && ProjectID == null)
            {
                projects = ProjectRepo.GetCollections();

            }
            else if (ProjectID != null)
            {
                projects = ProjectRepo.GetCollections().Where(p => p.ProjectGUID == ProjectID);
            }
            else
            {
                projects = ProjectRepo.GetCollections().Where(p => p.RequiredDeptGUID == DepartmentID);
            }

            var q = from p in projects
                    join t in TaskRepo.GetCollections() on p.ProjectGUID equals t.ProjectGUID
                    join tr in ResourceRepo.GetCollections() on t.TaskGUID equals tr.TaskGUID
                    join c in ResourceCatRepo.GetCollections() on tr.CategoryID equals c.CategoryID
                    select new ProjectResourceVM
                    {
                        ProjectGUID = p.ProjectGUID,
                        ProjectName = p.ProjectName,
                        TaskGUID = t.TaskGUID,
                        TaskName = t.TaskName,
                        ResourceGUID = tr.ResourceGUID,
                        ResourceID = tr.ResourceID,
                        ResourceName = tr.ResourceName,
                        CategoryID = c.CategoryID,
                        CategoryName = c.CategoryName,
                        Quantity = tr.Quantity,
                        Unit = tr.Unit,
                        UnitPrice = tr.UnitPrice,
                        SubTotal = (tr.UnitPrice * tr.Quantity),
                        Date = tr.Date,
                        Description = tr.Description
                    };

            var ProjectResourceList = q.AsQueryable().Sort(sortBy).Filter(filterBy).Select(p => new
            {
                費用發生日期 = p.Date.ToShortDateString(),
                專案名稱 = p.ProjectName,
                工作名稱 = p.TaskName,
                費用名稱 = p.ResourceName,
                類別名稱 = p.CategoryName,
                數量 = p.Quantity,
                單價 = p.UnitPrice,
                小計 = p.SubTotal,
                備註 = p.Description
            });

            var gv = new GridView();
            gv.DataSource = ProjectResourceList.ToList();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename = [" + DateTime.Now.ToShortDateString() + "]費用清單.xls");
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return Content("Complete!","text/plain");
        }
        #endregion

        #region Action For ExpCarMgr
        public ActionResult ExpCatMgr()
        {
            return View(ResourceCatRepo.GetCollections());
        }

        public ActionResult AddCat(ResourceCategory cat)
        {
            cat.CategoryID = ResourceCatRepo.GetCollections().Select(c => c.CategoryID).Max() + 1;

            ResourceCatRepo.Add(cat);
            return RedirectToAction("ExpCatMgr");
        }

        public ActionResult UpdateCat(ResourceCategory cat)
        {
            ResourceCatRepo.Update(cat);
            return RedirectToAction("ExpCatMgr");
        }

        public ActionResult DeleteCat(int? id)
        {
            List<TaskResource> resources = ResourceRepo.GetCollections().Where(r => r.CategoryID == id).ToList();

            if(resources.Count > 0)
            {
                return Content("failure", "text/plain");
            }
            else
            {
                ResourceCatRepo.Delete(ResourceCatRepo.Find(id));
                return RedirectToAction("ExpCatMgr");
            }
        }
        #endregion

        #region Action For Charts
        public ActionResult Charts()
        {
            ViewBag.Departments = (from d in DptRepo.GetCollections()
                                   join p in ProjectRepo.GetCollections() on d.DepartmentGUID equals p.RequiredDeptGUID
                                   select d).Distinct().ToList();

            return View();
        }

        public ActionResult CostsByDepartments(Guid? DepartmentGUID, Guid? ProjectGUID)
        {
            ChartData<SingleColorChartDataset<int>> chartData = new ChartData<SingleColorChartDataset<int>>();

            if (DepartmentGUID == null && ProjectGUID == null)
            {
                List<Department> departments = (from d in DptRepo.GetCollections()
                                                join p in ProjectRepo.GetCollections() on d.DepartmentGUID equals p.RequiredDeptGUID
                                                select d).Distinct().ToList();

                chartData.labels = departments.Select(d => d.DepartmentName).ToList();

                chartData.datasets.Add(new SingleColorChartDataset<int>
                {
                    type = "line",
                    label = "累計費用",
                    backgroundColor = "#d9006c",
                    borderColor = "#d9006c",
                    data = departments.GetCostsByDepartment()
                });

                chartData.datasets.Add(new SingleColorChartDataset<int>
                {
                    label = "總專案預算",
                    backgroundColor = "rgba(91, 155, 213, 0.5)",
                    borderColor = "rgba(91, 155, 213, 1)",
                    data = departments.GetBudgetsByDepartment()
                });
            }
            else if (ProjectGUID != null)
            {
                Project project = ProjectRepo.Find(ProjectGUID);
                List<int> months = new List<int>();

                for (int i = 2; i >= 0; i--) 
                {
                    if(DateTime.Now.Month - i <= 0)
                    {
                        months.Add(DateTime.Now.Month + 12 - i);
                    }
                    else
                    {
                        months.Add(DateTime.Now.Month - i);
                    }
                }

                List<string> chtmonths = months.GetChineseMonths().ToList();

                chartData.labels = chtmonths;

                chartData.datasets.Add(new SingleColorChartDataset<int>
                {
                    label = "當月費用",
                    backgroundColor = "#90C3D4",
                    borderColor = "#90C3D4",
                    data = project.GetCostsByMonths(months)
                });
            }
            else
            {
                List<Project> projects = ProjectRepo.GetCollections().Where(p => p.RequiredDeptGUID == DepartmentGUID).ToList();

                chartData.labels = projects.Select(p => p.ProjectName).ToList();

                chartData.datasets.Add(new SingleColorChartDataset<int>
                {
                    type = "line",
                    label = "累計費用",
                    backgroundColor = "#d9006c",
                    borderColor = "#d9006c",
                    data = projects.GetCostsByProjects()
                });

                chartData.datasets.Add(new SingleColorChartDataset<int>
                {
                    label = "專案預算",
                    backgroundColor = "rgba(91, 155, 213, 0.5)",
                    borderColor = "rgba(91, 155, 213, 1)",
                    data = projects.GetBudgetsByProject()
                });

            }


            return Content(JsonConvert.SerializeObject(chartData), "application/json");
        }

        public ActionResult OverallRates(Guid? DepartmentGUID, Guid? ProjectGUID)
        {
            ChartData<SingleColorChartDataset<double>> chartData = new ChartData<SingleColorChartDataset<double>>();

            if (DepartmentGUID == null && ProjectGUID == null)
            {
                chartData.labels = new List<string>() { "總體專案完成率", "總體預算執行率" };

            }
            else if (ProjectGUID != null)
            {
                chartData.labels = new List<string>() { "專案完成率", "預算執行率" };
            }
            else
            {
                chartData.labels = new List<string>() { "部門專案完成率", "部門預算執行率" };
            }

            chartData.datasets.Add(new SingleColorChartDataset<double>
            {
                label = "Rate",
                backgroundColor = "rgba(91, 155, 213, 0.5)",
                borderColor = "rgba(91, 155, 213, 1)",
                data = ProjectRepo.GetCollections().GetOverallRates(DepartmentGUID, ProjectGUID)
            });

            return Content(JsonConvert.SerializeObject(chartData), "application/json");
        }

        public ActionResult CostsByCategories(Guid? DepartmentGUID, Guid? ProjectGUID)
        {
            ChartData<MultiColorChartDataset<int>> chartData = new ChartData<MultiColorChartDataset<int>>();
            List<ResourceCategory> Cats = new List<ResourceCategory>();

            if (DepartmentGUID == null && ProjectGUID == null)
            {
                Cats = ResourceCatRepo.GetCollections().ToList();
            }
            else if (ProjectGUID != null)
            {
                Cats = (from p in ProjectRepo.GetCollections().Where(p => p.ProjectGUID == ProjectGUID)
                        join t in TaskRepo.GetCollections() on p.ProjectGUID equals t.ProjectGUID
                        join r in ResourceRepo.GetCollections() on t.TaskGUID equals r.TaskGUID
                        join c in ResourceCatRepo.GetCollections() on r.CategoryID equals c.CategoryID
                        select c).Distinct().ToList();
            }
            else
            {
                Cats = (from p in ProjectRepo.GetCollections().Where(p => p.RequiredDeptGUID == DepartmentGUID)
                        join t in TaskRepo.GetCollections() on p.ProjectGUID equals t.ProjectGUID
                        join r in ResourceRepo.GetCollections() on t.TaskGUID equals r.TaskGUID
                        join c in ResourceCatRepo.GetCollections() on r.CategoryID equals c.CategoryID
                        select c).Distinct().ToList();
            }

            List<string> Colors = new List<string>() { "#90C3D4", "#C390D4", "#AFDEA0", "#EBB6A4", "#EEF2A5", "#A5F2CF" };
            List<string> CatNames = Cats.Select(c => c.CategoryName).ToList();
            List<int> Subtotals = Cats.GetSubtotalByCat().ToList();

            Dictionary<string, int> CatPairs = new Dictionary<string, int>();

            for (int i = 0; i <= CatNames.Count - 1; i++)
            {
                CatPairs.Add(CatNames[i], Subtotals[i]);
            }

            List<KeyValuePair<string,int>> SortedCatPairs = CatPairs.OrderByDescending(c => c.Value).ToList();

            List<string> SortedCats = new List<string>();
            List<int> SortedSubtotals = new List<int>();
            int sum = 0;

            for (int i = 0; i <= SortedCatPairs.Count() - 1; i++)
            {
                if(i < 6)
                {
                    SortedCats.Add(SortedCatPairs[i].Key);
                    SortedSubtotals.Add(SortedCatPairs[i].Value);
                }
                else if(i == 6)
                {
                    SortedCats.Add("其他");
                    sum += SortedCatPairs[i].Value;
                }
                else
                {
                    sum += SortedCatPairs[i].Value;
                }
            }

            SortedSubtotals.Add(sum);

            chartData.labels = SortedCats;
            chartData.datasets.Add(new MultiColorChartDataset<int>
            {
                backgroundColor = Colors,
                borderColor = Colors,
                data = SortedSubtotals,
            });

            return Content(JsonConvert.SerializeObject(chartData), "application/json");
        }

        public ActionResult SumOfResources(Guid? DepartmentGUID, Guid? ProjectGUID)
        {
            ChartData<MultiColorChartDataset<int>> chartData = new ChartData<MultiColorChartDataset<int>>();
            List<string> Colors = new List<string>() { "#90C3D4", "#C390D4", "#AFDEA0", "#EBB6A4", "#EEF2A5", "#A5F2CF" };
            List<Project> projects = new List<Project>();
            List<Tasks> tasks = new List<Tasks>();
            List<string> targetNames = new List<string>();
            List<int> subtotals = new List<int>();
            Dictionary<string, int> Pairs = new Dictionary<string, int>();

            if (ProjectGUID == null && DepartmentGUID == null)
            {
                projects = ProjectRepo.GetCollections().ToList();
                targetNames = projects.Select(p => p.ProjectName).ToList();
                subtotals = projects.GetCostsByProjects().ToList();
            }
            else if (ProjectGUID != null)
            {
                tasks = TaskRepo.GetCollections().Where(t => t.ProjectGUID == ProjectGUID).GetRootTasks().ToList();
                targetNames = tasks.Select(t => t.TaskName).ToList();
                subtotals = tasks.GetCostsByTasks().ToList();
            }
            else
            {
                projects = ProjectRepo.GetCollections().Where(p => p.RequiredDeptGUID == DepartmentGUID).ToList();
                targetNames = projects.Select(p => p.ProjectName).ToList();
                subtotals = projects.GetCostsByProjects().ToList();
            }

            for (int i = 0; i <= targetNames.Count - 1; i++)
            {
                Pairs.Add(targetNames[i], subtotals[i]);
            }

            List<KeyValuePair<string, int>> SortedPairs = Pairs.OrderByDescending(c => c.Value).ToList();

            List<string> SortedNames = new List<string>();
            List<int> SortedSubtotals = new List<int>();
            int sum = 0;

            for (int i = 0; i <= SortedPairs.Count() - 1; i++)
            {
                if (i < 6)
                {
                    SortedNames.Add(SortedPairs[i].Key);
                    SortedSubtotals.Add(SortedPairs[i].Value);
                }
                else if (i == 6)
                {
                    SortedNames.Add("其他");
                    sum += SortedPairs[i].Value;
                }
                else
                {
                    sum += SortedPairs[i].Value;
                }
            }

            SortedSubtotals.Add(sum);

            chartData.labels = SortedNames;
            chartData.datasets.Add(new MultiColorChartDataset<int>
            {
                backgroundColor = Colors,
                borderColor = Colors,
                data = SortedSubtotals,
            });

            return Content(JsonConvert.SerializeObject(chartData), "application/json");
        }

        #endregion

        #region Action For CostEstimation

        public ActionResult CostEstimation()
        {
            ViewBag.Departments = (from d in DptRepo.GetCollections()
                                   join p in ProjectRepo.GetCollections() on d.DepartmentGUID equals p.RequiredDeptGUID
                                   select d).Distinct().ToList();

            Response.Cookies["sortBy"].Value = "";

            return View();
        }

        public ActionResult GetCostPool(Guid? projectGUID)
        {
            CostPool pool = new CostPool();

            if (projectGUID == null)
            {
                var q = PoolRepo.GetCollections().Where(p => p.ProjectGUID == null);

                if (q.Count() != 0)
                {
                    pool = q.FirstOrDefault();
                }
                else
                {
                    pool.PoolGUID = Guid.NewGuid();
                    pool.CreatedDate = DateTime.Now;
                    pool.ModifiedDate = DateTime.Now;
                    PoolRepo.Add(pool);
                }
            }
            else
            {
                var q = PoolRepo.GetCollections().Where(p => p.ProjectGUID == projectGUID);

                if (q.Count() != 0)
                {
                    pool = q.FirstOrDefault();
                }
                else
                {
                    CostPool standardPool = PoolRepo.GetCollections().Where(p => p.ProjectGUID == null).FirstOrDefault();
                    pool.PoolGUID = Guid.NewGuid();
                    pool.InterestExpense = standardPool.InterestExpense;
                    pool.ModifiedDate = DateTime.Now;
                    pool.OtherManagementCosts = standardPool.OtherManagementCosts;
                    pool.RentalPerMonth = standardPool.RentalPerMonth;
                    pool.RiskPreparationCost = standardPool.RiskPreparationCost;
                    pool.UtilityPerMonth = standardPool.UtilityPerMonth;
                    pool.WagePerHour = standardPool.WagePerHour;
                    pool.ProjectGUID = projectGUID;
                    pool.CreatedDate = DateTime.Now;
                    pool.ModifiedDate = DateTime.Now;
                    PoolRepo.Add(pool);
                }
            }

            return Json(pool, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditPool(CostPool pool)
        {
            CostPool standardPool = PoolRepo.GetCollections().Where(p => p.ProjectGUID == null).FirstOrDefault();

            if(pool.PoolGUID == standardPool.PoolGUID)
            {
                try
                {
                    standardPool.InterestExpense = pool.InterestExpense;
                    standardPool.ModifiedDate = DateTime.Now;
                    standardPool.OtherManagementCosts = pool.OtherManagementCosts;
                    standardPool.RentalPerMonth = pool.RentalPerMonth;
                    standardPool.RiskPreparationCost = pool.RiskPreparationCost;
                    standardPool.UtilityPerMonth = pool.UtilityPerMonth;
                    standardPool.WagePerHour = pool.WagePerHour;

                    PoolRepo.Update(standardPool);
                }
                catch (Exception e)
                {
                    return Json(e.ToString(), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var q = PoolRepo.GetCollections().Where(p => p.ProjectGUID == pool.ProjectGUID);

                if (q.Count() != 0)
                {
                    try
                    {
                        var projectPool = q.FirstOrDefault();

                        projectPool.InterestExpense = pool.InterestExpense;
                        projectPool.ModifiedDate = DateTime.Now;
                        projectPool.OtherManagementCosts = pool.OtherManagementCosts;
                        projectPool.RentalPerMonth = pool.RentalPerMonth;
                        projectPool.RiskPreparationCost = pool.RiskPreparationCost;
                        projectPool.UtilityPerMonth = pool.UtilityPerMonth;
                        projectPool.WagePerHour = pool.WagePerHour;

                        PoolRepo.Update(projectPool);
                    }
                    catch (Exception e)
                    {
                        return Json(e.ToString(), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    try
                    {
                        pool.PoolGUID = Guid.NewGuid();
                        pool.CreatedDate = DateTime.Now;
                        pool.ModifiedDate = DateTime.Now;
                        PoolRepo.Add(pool);
                    }
                    catch (Exception e)
                    {
                        return Json(e.ToString(), JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json("New Pool has been created!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCostEstimationSheets(Guid? DepartmentID, Guid? ProjectID, int? page, string sortBy)
        {
            List<CostEstimateSheet> sheets = new List<CostEstimateSheet>();

            if (DepartmentID == null && ProjectID == null)
            {
                sheets = SheetRepo.GetCollections().ToList();
            }
            else if (ProjectID != null)
            {
                sheets = SheetRepo.GetCollections().Where(s => s.ProjectGUID == ProjectID).ToList();
            }
            else
            {
                var projects = ProjectRepo.GetCollections().Where(p => p.RequiredDeptGUID == DepartmentID).Select(p => p.ProjectGUID).ToList();

                foreach (var s in SheetRepo.GetCollections().ToList())
                {
                    if (projects.Any(pid => s.ProjectGUID == pid))
                    {
                        sheets.Add(s);
                    }
                }
            }

            var q = from s in sheets
                    join p in ProjectRepo.GetCollections() on s.ProjectGUID equals p.ProjectGUID
                    select new CostEstimateSheetVM
                    {
                        SheetGUID = s.SheetGUID,
                        SheetID = s.SheetID,
                        ProjectName = p.ProjectName,
                        ProjectGUID = s.ProjectGUID,
                        CreatorName = s.CreatorName,
                        CreateDate = s.CreateDate,
                        Description = s.Description,
                        ModifierName = s.ModifierName,
                        ModifiedDate = s.ModifiedDate
                    };

            var SheetList = q.ToList().AsQueryable().Sort(sortBy);

            Response.Cookies["sortBy"].Value = sortBy;
            ViewBag.sortByDate = string.IsNullOrEmpty(sortBy) ? "DateDesc" : "";
            ViewBag.sortByProjectName = sortBy == "ProjectName" ? "ProjectNameDesc" : "ProjectName";
            ViewBag.sortByCreatorName = sortBy == "CreatorName" ? "CreatorNameDesc" : "CreatorName";
            ViewBag.sortBySheetID = sortBy == "SheetID" ? "SheetIDDesc" : "SheetID";
            ViewBag.sortByDescription = sortBy == "Description" ? "DescriptionDesc" : "Description";

            ViewBag.Count = q.ToList().Count();

            return PartialView(SheetList.ToPagedList(page ?? 1, 10));
        }

        public ActionResult CreateEstimationSheet(Guid projectGUID)
        {
            ViewBag.ExpCats = new SelectList(ResourceCatRepo.GetCollections(), "CategoryID", "CategoryName");
            ViewBag.RootTasks = TaskRepo.GetCollections().Where(t => t.ProjectGUID == projectGUID).GetRootTasks();
            Project project = ProjectRepo.Find(projectGUID);

            return PartialView(project);
        }

        public ActionResult AddEstimationSheet(CostEstimateSheet sheet)
        {
            sheet.CreateDate = DateTime.Now;
            sheet.ModifiedDate = DateTime.Now;

            try
            {
                SheetRepo.Add(sheet);
            }
            catch (Exception e)
            {
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);
            }

            return Json("New Sheet has been created!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEstimationSheetDetails(IEnumerable<CostEstimateSheetDetail> details)
        {
            try
            {
                SheetDetailRepo.AddList(details);
            }
            catch (Exception e)
            {
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);
            }

            return Json("Sheet details have been saved!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateTask(IEnumerable<Tasks> rootTasks)
        {
            List<Tasks> TaskList = new List<Tasks>();

            foreach(var rt in rootTasks)
            {
                var task = TaskRepo.GetCollections().Where(t => t.TaskGUID == rt.TaskGUID).Single();
                task.EstWorkTime = rt.EstWorkTime;
                TaskList.Add(task);
            }

            try
            {
                TaskRepo.UpdateList(TaskList);
            }
            catch (Exception e)
            {
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);
            }

            return Json("Task worktime have been updated!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateProjectBudget(Guid projectGUID,int budget)
        {
            Project p = ProjectRepo.Find(projectGUID);
            p.ProjectBudget = budget;

            try
            {
                ProjectRepo.Update(p);
            }
            catch (Exception e)
            {
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);
            }

            return Json("Project Budget has been updated!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSheet(Guid SheetGUID)
        {
            try
            {
                SheetRepo.Delete(SheetRepo.Find(SheetGUID));
                List<CostEstimateSheetDetail> SheetDetails = SheetDetailRepo.GetCollections().Where(d => d.SheetGUID == SheetGUID).ToList();
                SheetDetailRepo.DeleteList(SheetDetails);
            }
            catch (Exception e)
            {
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);
            }

            return Json("Deletion Complete!", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewSheet(Guid SheetGUID)
        {
            ViewBag.Sheet = SheetRepo.Find(SheetGUID);

            var SheetDetails = SheetDetailRepo.GetCollections().Where(d => d.SheetGUID == SheetGUID);

            var detailList = (from s in SheetDetails
                                  join t in TaskRepo.GetCollections() on s.TaskGUID equals t.TaskGUID
                                  orderby s.ResourceCategoryID
                                  select new CostEstimateSheetDetailVM
                                  {
                                      ResourceCategoryID = s.ResourceCategoryID,
                                      TaskGUID = t.TaskGUID,
                                      TaskName = t.TaskName,
                                      Amount = s.Amount
                                  }).ToList();

            foreach(var d in detailList)
            {
                var cat = ResourceCatRepo.Find(d.ResourceCategoryID);
                d.ResourceCategoryName = cat == null? "直接人工" : cat.CategoryName;
            }

            var q = detailList.GroupBy(d => d.TaskGUID).Select(g => new CostEstimateSheetDetailVM{ TaskGUID = g.Key,TaskName = g.Select(t => t.TaskName).First(), group = g });

            var result = q.ToList();

            return PartialView(result);
        }
        #endregion
    }
}
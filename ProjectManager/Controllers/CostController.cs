using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManager.Models;
using PagedList;
using PagedList.Mvc;
using Newtonsoft.Json;

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

        public ActionResult GetTaskResources(Guid id, int? page, string sortBy, ResourceFilterVM filterBy)
        {
            var q = from p in ProjectRepo.GetCollections()
                    join t in TaskRepo.GetCollections() on p.ProjectGUID equals t.ProjectGUID
                    join tr in ResourceRepo.GetCollections() on t.TaskGUID equals tr.TaskGUID
                    join c in ResourceCatRepo.GetCollections() on tr.CategoryID equals c.CategoryID
                    //where p.ProjectGUID == id
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
            ViewBag.sortByTaskName = sortBy == "TaskName" ? "TaskNameDesc" : "TaskName";
            ViewBag.sortByResourceName = sortBy == "ResourceName" ? "ResourceNameDesc" : "ResourceName";
            ViewBag.sortByResourceCat = sortBy == "ResourceCat" ? "ResourceCatDesc" : "ResourceCat";
            ViewBag.sortByQuantity = sortBy == "Quantity" ? "QuantityDesc" : "Quantity";
            ViewBag.sortByUnitPrice = sortBy == "UnitPrice" ? "UnitPriceDesc" : "UnitPrice";
            ViewBag.sortBySubtotal = sortBy == "SubTotal" ? "SubTotalDesc" : "SubTotal";

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
        #endregion

        #region Action For ExpCarMgr
        public ActionResult ExpCatMgr()
        {
            return View(ResourceCatRepo.GetCollections());
        }

        public ActionResult AddCat(ResourceCategory cat)
        {
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
            ResourceCatRepo.Delete(ResourceCatRepo.Find(id));
            return RedirectToAction("ExpCatMgr");
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

        public ActionResult CostsByDepartments(Guid? DepartmentID, Guid? ProjectID)
        {
            ChartData<SingleColorChartDataset<int>> chartData = new ChartData<SingleColorChartDataset<int>>();

            if (DepartmentID == null && ProjectID == null)
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
            //else if(ProjectID != null)
            //{
            //    //TODO: What should be placed at this situation?
            //}
            else
            {
                List<Project> projects = ProjectRepo.GetCollections().Where(p => p.RequiredDeptGUID == DepartmentID).ToList();

                chartData.labels = projects.Select(p => p.ProjectName).ToList();

                chartData.datasets.Add(new SingleColorChartDataset<int>
                {
                    type = "line",
                    label = "累計費用",
                    backgroundColor = "#d9006c",
                    borderColor = "#d9006c",
                    data = projects.GetCostsByProject()
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

        public ActionResult OverallRates(Guid? DepartmentID, Guid? ProjectID)
        {
            ChartData<SingleColorChartDataset<double>> chartData = new ChartData<SingleColorChartDataset<double>>();

            if (DepartmentID == null && ProjectID == null)
            {
                chartData.labels = new List<string>() { "總體專案完成率", "總體預算執行率" };

            }
            else if (ProjectID != null)
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
                data = ProjectRepo.GetCollections().GetOverallRates(DepartmentID, ProjectID)
            });

            return Content(JsonConvert.SerializeObject(chartData), "application/json");
        }

        public ActionResult CostsByCategories(Guid? DepartmentID, Guid? ProjectID)
        {
            ChartData<MultiColorChartDataset<int>> chartData = new ChartData<MultiColorChartDataset<int>>();
            List<ResourceCategory> Cats = new List<ResourceCategory>();

            if (DepartmentID == null && ProjectID == null)
            {
                Cats = ResourceCatRepo.GetCollections().ToList();
            }
            else if (ProjectID != null)
            {
                Cats = (from p in ProjectRepo.GetCollections().Where(p => p.ProjectGUID == ProjectID)
                        join t in TaskRepo.GetCollections() on p.ProjectGUID equals t.ProjectGUID
                        join r in ResourceRepo.GetCollections() on t.TaskGUID equals r.TaskGUID
                        join c in ResourceCatRepo.GetCollections() on r.CategoryID equals c.CategoryID
                        select c).Distinct().ToList();
            }
            else
            {
                Cats = (from p in ProjectRepo.GetCollections().Where(p => p.RequiredDeptGUID == DepartmentID)
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

        public ActionResult TasksByStatus(Guid? DepartmentID, Guid? ProjectID)
        {
            ChartData<SingleColorChartDataset<int>> chartData = new ChartData<SingleColorChartDataset<int>>();

            if (DepartmentID == null && ProjectID == null)
            {

            }
            else if (ProjectID != null)
            {
                //TODO: What should be shown at this situation?
            }
            else
            {

            }

            chartData.labels = StatusRepo.GetCollections().Select(s => s.TaskStatusName).ToList();
            chartData.datasets.Add(new SingleColorChartDataset<int>
            {
                label = "Count",
                backgroundColor = "rgba(91, 155, 213, 0.5)",
                borderColor = "rgba(91, 155, 213, 1)",
                data = StatusRepo.GetCollections().CountTasksByStatus(),
            });

            return Content(JsonConvert.SerializeObject(chartData), "application/json");
        }
        #endregion
    }
}
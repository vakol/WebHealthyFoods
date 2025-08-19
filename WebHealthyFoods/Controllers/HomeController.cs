using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.TimeUnitsDataSetTableAdapters;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TimeUnits()
        {

            // Connect to the database "healthy_foods_db".
            // This is typically done in the context of a database initializer or configuration class.
            var adapter = new casove_jednotkyTableAdapter();
            var table = adapter.GetData();

            var timeUnits = new List<TimeUnitViewModel>();
            foreach (var item in table)
            {
                timeUnits.Add(new TimeUnitViewModel
                {
                    Id = item.identifikace_casove_jednotky,
                    Name = item.nazev_casove_jednotky
                });
            }

            return View(timeUnits);
        }
    }

    public class TimeUnitViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
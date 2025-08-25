using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using WebHealthyFoods.Models;
using WebHealthyFoods.Models.PrepareFoodsDataSetTableAdapters;

namespace WebHealthyFoods.Controllers
{
    /**
     * HomeController class that handles requests for the home page, about page, contact page,
     * and food preparation.
     */
    public class HomeController : Controller
    {
        /**
         * Default action that displays the home page.
         * @return ActionResult containing the home view.
         */
        public ActionResult Index()
        {
            return View();
        }

        /**
         * Displays the About page.
         * @return ActionResult containing the about view.
         */
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        /**
         * Displays the Contact page.
         * @return ActionResult containing the contact view.
         */
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /**
         * Prepares the foods for display.
         * @param foodIndex  - The index of the food to display, defaults to 0 if null.
         * @return ActionResult containing the prepared foods view.
         */
        public ActionResult PrepareFoods(int? foodIndex)
        {
            // Initialize the TableAdapterManager and the DataSet.
            var manager = new TableAdapterManager();
            var dataSet = new PrepareFoodsDataSet();

            // Fill the DataSet with data from the database.
            manager.receptyTableAdapter = new receptyTableAdapter();
            manager.receptyTableAdapter.Fill(dataSet.recepty);

            manager.typy_pokrmuTableAdapter = new typy_pokrmuTableAdapter();
            manager.typy_pokrmuTableAdapter.Fill(dataSet.typy_pokrmu);

            manager.casove_jednotkyTableAdapter = new casove_jednotkyTableAdapter();
            manager.casove_jednotkyTableAdapter.Fill(dataSet.casove_jednotky);

            manager.enrgeticke_jednotkyTableAdapter = new enrgeticke_jednotkyTableAdapter();
            manager.enrgeticke_jednotkyTableAdapter.Fill(dataSet.enrgeticke_jednotky);

            manager.UpdateAll(dataSet);

            // Prepare the ViewBag with necessary data.
            foodIndex = foodIndex ?? 0;
            ViewBag.FoodIndex = foodIndex;
            ViewBag.previousFoodIndex = foodIndex - 1;
            ViewBag.nextFoodIndex = foodIndex + 1;
            ViewBag.foodCount = manager.receptyTableAdapter.GetData().Rows.Count;

            trimFoodIndices(ViewBag);

            // Create the ViewModel and pass it to the view.
            var model = new PrepareFoodsViewModel("PrepareFoods", manager, ViewBag);
            return View(model);
        }
        /**
         * Trims the food indices to ensure they are within valid bounds.
         * @param viewBag  - The dynamic object containing the view values.
         */
        private void trimFoodIndices(dynamic viewBag)
        {
            if (ViewBag.FoodIndex < 0)
            {
                ViewBag.FoodIndex = 0;
            }
            if (ViewBag.FoodIndex > (ViewBag.foodCount - 1))
            {
                ViewBag.FoodIndex = ViewBag.foodCount - 1;
            }

            if (ViewBag.previousFoodIndex < 0)
            {
                ViewBag.previousFoodIndex = 0;
            }
            if (ViewBag.previousFoodIndex > (ViewBag.foodCount - 1))
            {
                ViewBag.previousFoodIndex = ViewBag.foodCount - 1;
            }

            if (ViewBag.nextFoodIndex < 0)
            {
                ViewBag.nextFoodIndex = 0;
            }
            if (ViewBag.nextFoodIndex > (ViewBag.foodCount - 1))
            {
                ViewBag.nextFoodIndex = ViewBag.foodCount - 1;
            }
        }

        /**
         * Displays the FoodsIngredients page.
         * @return ActionResult containing the foods ingredients view.
         */
        public ActionResult FoodsIngredients()
        {
            // Initialize the TableAdapterManager and the DataSet.
            var manager = new TableAdapterManager();
            var dataSet = new PrepareFoodsDataSet();

            // Fill the DataSet with data from the database.
            manager.surovinyTableAdapter = new surovinyTableAdapter();
            manager.surovinyTableAdapter.Fill(dataSet.suroviny);
            // Convert DataTable to List<Dictionary<string, object>>
            ViewBag.FoodsList = dataSet.suroviny.ToDictionaryList();

            manager.slozky_surovinTableAdapter = new slozky_surovinTableAdapter();
            manager.slozky_surovinTableAdapter.Fill(dataSet.slozky_surovin);
            // Convert DataTable to List<Dictionary<string, object>>
            ViewBag.FoodIngredientsList = dataSet.slozky_surovin.ToDictionaryList();

            manager.slozkyTableAdapter = new slozkyTableAdapter();
            manager.slozkyTableAdapter.Fill(dataSet.slozky);
            // Convert DataTable to List<Dictionary<string, object>>
            ViewBag.IngredientsList = dataSet.slozky.ToDictionaryList();

            manager.UpdateAll(dataSet);

            // Prepare the ViewBag with necessary data.
            var model = new PrepareFoodsViewModel("FoodsIngredients", manager, ViewBag);
            return View(model);
        }
    }

    /**
     * ViewModel for preparing foods, encapsulating the data needed for the view.
     */
    public class PrepareFoodsViewModel
    {
        /**
         * Initializes a new instance of the PrepareFoodsViewModel class.
         * @param manager  - The TableAdapterManager to manage data operations.
         */
        private TableAdapterManager manager { get; set; }

        // Constructor to initialize the ViewModel with data from the database.
        public PrepareFoodsViewModel(string viewName, TableAdapterManager manager, dynamic viewBag)
        {
            try
            {
                // Assign the manager.
                this.manager = manager;

                // Load data for PrepareFoods view.
                if (viewName == "PrepareFoods") {
                    PreparedFoods = manager.receptyTableAdapter.GetData();
                    FoodTypes = manager.typy_pokrmuTableAdapter.GetData();
                    TimeUnits = manager.casove_jednotkyTableAdapter.GetData();
                    EnrgUnits = manager.enrgeticke_jednotkyTableAdapter.GetData();

                    // Initialize current food details.
                    if (!Convert.IsDBNull(PreparedFoods.Rows[viewBag.foodIndex].identifikace_energeticke_jednotky))
                    {
                        CurrentEnrgUnitId = (PreparedFoods.Rows[viewBag.foodIndex].identifikace_energeticke_jednotky).ToString();
                    }
                    if (!Convert.IsDBNull(PreparedFoods.Rows[viewBag.foodIndex].identifikace_typu_pokrmu))
                    {
                        CurrentFoodTypeId = (PreparedFoods.Rows[viewBag.foodIndex].identifikace_typu_pokrmu).ToString();
                    }
                    if (!Convert.IsDBNull(PreparedFoods.Rows[viewBag.foodIndex].identifikace_casove_jednotky))
                    {
                        CurrentTimeUnitId = (PreparedFoods.Rows[viewBag.foodIndex].identifikace_casove_jednotky).ToString();
                    }

                    // Map the current food details.
                    CurrentFoodType = (from footType in FoodTypes
                                       where footType.identifikace_typu_pokrmu == int.Parse(CurrentFoodTypeId)
                                       select footType.nazev_typu_pokrmu).FirstOrDefault();
                    CurrentPreparedFood = PreparedFoods.Rows[viewBag.foodIndex].nadpis;
                    CurrentPrepTime = PreparedFoods.Rows[viewBag.foodIndex].doba_pripravy;
                    CurrentPrepUnit = (from timeUnit in TimeUnits
                                       where timeUnit.identifikace_casove_jednotky == int.Parse(CurrentTimeUnitId)
                                       select timeUnit.nazev_casove_jednotky).FirstOrDefault();
                    CurrentEnrg = PreparedFoods.Rows[viewBag.foodIndex].energeticka_hodnota;
                    CurrentEnrgUnit = (from enrgUnit in EnrgUnits
                                       where enrgUnit.identifikace_energeticke_jednotky == int.Parse(CurrentEnrgUnitId)
                                       select enrgUnit.nazev_energeticke_jednotky).FirstOrDefault();
                    CurrentPrep = PreparedFoods.Rows[viewBag.foodIndex].priprava;
                }

                // Load data for FoodsIngredients view.
                if (viewName == "FoodsIngredients") {
                    Foods = manager.surovinyTableAdapter.GetData();
                    FoodIngredients = manager.slozky_surovinTableAdapter.GetData();
                    Ingredients = manager.slozkyTableAdapter.GetData();
                }                
            }
            catch (System.Exception ex)
            {
                // Handle exceptions appropriately, e.g., log the error or rethrow it.
                throw new System.Exception("Error initializing PrepareFoodsViewModel: " + ex.Message);
            }
        }

        // Properties to hold the data tables.
        public PrepareFoodsDataSet.receptyDataTable PreparedFoods { get; }
        public PrepareFoodsDataSet.typy_pokrmuDataTable FoodTypes { get; }
        public PrepareFoodsDataSet.casove_jednotkyDataTable TimeUnits { get; }
        public PrepareFoodsDataSet.enrgeticke_jednotkyDataTable EnrgUnits { get; }
        public PrepareFoodsDataSet.surovinyDataTable Foods { get; }
        public PrepareFoodsDataSet.slozky_surovinDataTable FoodIngredients { get; }
        public PrepareFoodsDataSet.slozkyDataTable Ingredients { get; }

        // Properties to hold the current food details.
        public string CurrentFoodTypeId { get; }
        public string CurrentTimeUnitId { get; }
        public string CurrentEnrgUnitId { get; }
        public string CurrentPrepTime { get; }
        public string CurrentFoodType { get; }
        public string CurrentPreparedFood { get; }
        public string CurrentPrepUnit { get; }
        public string CurrentEnrg { get; }
        public string CurrentEnrgUnit { get; }
        public string CurrentPrep { get; }
    }

    public static class DataTableExtensions
    {
        public static List<Dictionary<string, object>> ToDictionaryList(this System.Data.DataTable table)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (System.Data.DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                list.Add(dict);
            }
            return list;
        }
    }
}
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebHealthyFoods.Models;
using WebHealthyFoods.Models.FoodsDataSetTableAdapters;
using static WebHealthyFoods.Utility.WindowsCredentialManager;

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
        [AllowAnonymous]
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
            ViewBag.Message = "";

            // TODO: <---TEST C#
            //new TestCSharp().TestMethod();

            return View();
        }

        /**
         * Displays the Contact page.
         * @return ActionResult containing the contact view.
         */
        public ActionResult Contact()
        {
            ViewBag.Message = "";

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
            var dataSet = new FoodsDataSet();

            dataSet.EnforceConstraints = false;

            // Fill the DataSet with data from the database.
            manager.receptyTableAdapter = new receptyTableAdapter();
            manager.receptyTableAdapter.Fill(dataSet.recepty);

            manager.typy_pokrmuTableAdapter = new typy_pokrmuTableAdapter();
            manager.typy_pokrmuTableAdapter.Fill(dataSet.typy_pokrmu);

            manager.casove_jednotkyTableAdapter = new casove_jednotkyTableAdapter();
            manager.casove_jednotkyTableAdapter.Fill(dataSet.casove_jednotky);

            manager.energeticke_jednotkyTableAdapter = new energeticke_jednotkyTableAdapter();
            manager.energeticke_jednotkyTableAdapter.Fill(dataSet.energeticke_jednotky);

            manager.UpdateAll(dataSet);

            // Prepare the ViewBag with necessary data.
            foodIndex = foodIndex ?? 0;
            ViewBag.FoodIndex = foodIndex;
            ViewBag.previousFoodIndex = foodIndex - 1;
            ViewBag.nextFoodIndex = foodIndex + 1;
            ViewBag.foodCount = manager.receptyTableAdapter.GetData().Rows.Count;

            trimFoodIndices(ViewBag);
            
            // Create the ViewModel and pass it to the view.
            var model = new FoodsViewModel("PrepareFoods", manager, ViewBag);
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
            var dataSet = new FoodsDataSet();

            dataSet.EnforceConstraints = false;

            manager.slozky_surovinTableAdapter = new slozky_surovinTableAdapter();
            manager.slozky_surovinTableAdapter.Fill(dataSet.slozky_surovin);

            // Convert DataTable to List<Dictionary<string, object>>
            ViewBag.FoodIngredientsList = dataSet.slozky_surovin.ToDictionaryList();

            manager.slozkyTableAdapter = new slozkyTableAdapter();
            manager.slozkyTableAdapter.Fill(dataSet.slozky);

            manager.surovinyTableAdapter = new surovinyTableAdapter();
            manager.surovinyTableAdapter.Fill(dataSet.suroviny);
            ViewBag.FoodsList = dataSet.suroviny.ToDictionaryList();

            // Sort the DataTable by "nazev_slozky" column.
            var sortedList = dataSet.slozky.ToDictionaryList()
                .OrderBy(dict => dict["nazev_slozky"] as string)
                .ToList();
            // Convert DataTable to List<Dictionary<string, object>>
            ViewBag.IngredientsList = sortedList;

            manager.UpdateAll(dataSet);

            // Prepare the ViewBag with necessary data.
            var model = new FoodsViewModel("FoodsIngredients", manager, ViewBag);
            return View(model);
        }

        /**
         * Convert voice to text.
         */
        [HttpPost]
        public ActionResult Voice2Text(HttpPostedFileBase file)
        {
            ViewBag.Voice2Text = "Voice to text conversion not implemented.";

            if (file != null && file.ContentLength > 0)
            {
                Debug.WriteLine($"Vice2Text: Received file: {file.FileName}, size: {file.ContentLength} bytes");

                // You can access file.InputStream, file.FileName, etc.
                string fileName = Path.GetFileName(file.FileName);
                string inputVoiceFile = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);
                try
                {
                    Debug.WriteLine($"Voice2Text: Saving file to: {inputVoiceFile}");

                    // Ensure the directory exists and save the file.
                    file.SaveAs(inputVoiceFile);
                    string resultVoiceFile = Path.ChangeExtension(inputVoiceFile, ".mp3");

                    // If file extensions area not equal, make file conversion using FFMPEG.
                    bool success = true;
                    if (inputVoiceFile.ToLower() != resultVoiceFile.ToLower())
                    {
                        Debug.WriteLine($"Voice2Text: Converting file to: {resultVoiceFile}");

                        success = ConvertVoiceEncoding(inputVoiceFile, resultVoiceFile, out string resultMessage);
                        if (!success)
                        {
                            ViewBag.Voice2Text = resultMessage;
                            Debug.WriteLine($"Voice2Text: Conversion error. Message: {resultMessage}");
                        }
                    }

                    // If conversion was successful, proceed with voice to text conversion.
                    if (success)
                    {
                        Debug.WriteLine($"Voice2Text: Converting voice to text from file: {resultVoiceFile}");
                        success = ConvertVoice2Text(resultVoiceFile, out string textMessage);
                        if (success)
                        {
                            Debug.WriteLine($"Voice2Text: Conversion successful. Text: {textMessage}");
                            ViewBag.Voice2Text = textMessage;
                        }
                        else
                        {
                            Debug.WriteLine($"Voice2Text: Conversion failed: {textMessage}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Voice2Text: Conversion failed.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Voice2Text: Exception occurred - {ex.Message}");
                    ViewBag.Voice2Text = ex.Message;
                }
            }
            else
            {
                Debug.WriteLine("Voice2Text: No file received or file is empty.");
            }

            // Return view but set layout to null to avoid rendering the full layout.
            ViewBag.Layout = null;
            return View();
        }

        /**
         * Send an email with the provided details.
         * @return ActionResult indicating the result of the email sending operation.
         */
        [HttpPost]
        public ActionResult SendEmail(string emailAddress, string subject, string message)
        {
            try
            {
                // TODO: UNCOMMENT Compute secret from new password and place it into web.config.
                /*string secretEcryptedBase64 = null;
                bool ok = Protect(@"___VLOZTE_HESLO___", out secret);*/

                // Get SMTP user and protected password from web.config.
                string userName =  ConfigurationManager.AppSettings["EmailServerUser"];
                string secret = ConfigurationManager.AppSettings["EmailServerSecret"];

                char [] pwd = null;
                bool success = Unprotect(secret, out pwd);
                if (!success || userName == null || pwd == null)
                {
                    // Set pwd to zeros for security.
                    if (pwd != null)
                    {
                        Array.Clear(pwd, 0, pwd.Length);
                    }
                    string errorMsg = "Failed to send email: cannot access e-mail server.";
                    if (!success)
                    {
                        errorMsg += " Crendenatial not found!";
                    }
                    else
                    {
                        if (userName == null || userName == "")
                        {
                            errorMsg += " User name is empty!";
                        }
                        if (pwd == null || pwd.Length == 0)
                        {
                            errorMsg += " Password is empty!";
                        }
                    }
                    return Json(new { success = true, message = errorMsg });
                }

                // Configure SMTP client
                var smtpClient = new System.Net.Mail.SmtpClient("smtp.zoner.com", 587) // Use your SMTP server and port
                {
                    Credentials = new System.Net.NetworkCredential(userName, new string(pwd)),
                    EnableSsl = true // Set to false if your server does not use SSL
                };
                // Set pwd to zeros for security.
                Array.Clear(pwd, 0, pwd.Length);
                // Set timeout to 7 seconds.
                smtpClient.Timeout = 7000; 

                // Create the email.
                var mailMessage = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(userName), // Sender address.
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false // Set to true if sending HTML.
                };
                mailMessage.To.Add(emailAddress);

                // Send the email.
                smtpClient.Send(mailMessage);

                // Return a JSON result for AJAX.
                return Json(new { success = true, message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                // Log or handle the error as needed.
                return Json(new { success = false, message = "Failed to send email: " + ex.Message });
            }
        }

        /**
         * Convert voice encoding using FFMPEG. The conversion depends on input and output file extensions.
         * @return Result of the conversion process.
         */
        private bool ConvertVoiceEncoding(string inputVoiceFile, string outputVoiceFile, out string message)
        {
            try
            {
                // Path to ffmpeg executable and arguments for conversion.
                string ffmpegPath = @"ffmpeg.exe"; // Update with your ffmpeg.exe path
                string arguments = $"-i \"{inputVoiceFile}\" \"{outputVoiceFile}\"";

                // Start the process to run ffmpeg with the specified arguments.
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();

                // Capture the output and error messages.
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Handle error case.
                if (process.ExitCode != 0)
                {
                    message = $"ERROR: {error}";
                    return false;
                }
                // Success case.
                message = "SUCCESS: " + output;
                return true;
            }
            catch (Exception ex)
            {
                // Handle internal exceptions case.
                message = "INTERNAL ERROR: " + ex.Message;
                return false;
            }
        }

        /**
         * Convert voice to text using whisper.exe.
         * @return Result of the conversion process.
         */
        private bool ConvertVoice2Text(string voiceFile, out string textMessage)
        {
            try
            {
                // Path to whisper.exe and arguments for conversion.
                string whisperPath = Server.MapPath("~/App_Data/Tools/whisper.exe");
                // Check if whisper.exe exists
                if (!System.IO.File.Exists(whisperPath))
                {
                    textMessage = "Conversion application whisper.exe not found.";
                    return false;
                }

                string arguments = voiceFile;

                // Start the process to run ffmpeg with the specified arguments.
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = whisperPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();

                // Capture the output and error messages.
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Handle error case.
                if (process.ExitCode != 0)
                {
                    textMessage = $"ERROR: {error}";
                    return false;
                }

                // Parse the output.
                textMessage = output.GetFirstTagContent("TEXT");

                // Success case.
                return true;
            }
            catch (Exception ex)
            {
                // Handle internal exceptions case.
                textMessage = "INTERNAL ERROR: " + ex.Message;
                return false;
            }
        }

        /**
         * Displays the FoodsIngredients page.
         * @return ActionResult containing the foods ingredients view.
         */
        public ActionResult LearnMore()
        {
            return View();
        }
    }

    /**
     * ViewModel for foods, encapsulating the data needed for the view.
     */
    public class FoodsViewModel
    {
        /**
         * Initializes a new instance of the PrepareFoodsViewModel class.
         * @param manager  - The TableAdapterManager to manage data operations.
         */
        private TableAdapterManager manager { get; set; }

        // Constructor to initialize the ViewModel with data from the database.
        public FoodsViewModel(string viewName, TableAdapterManager manager, dynamic viewBag)
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
                    EnrgUnits = manager.energeticke_jednotkyTableAdapter.GetData();

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
        public FoodsDataSet.receptyDataTable PreparedFoods { get; }
        public FoodsDataSet.typy_pokrmuDataTable FoodTypes { get; }
        public FoodsDataSet.casove_jednotkyDataTable TimeUnits { get; }
        public FoodsDataSet.energeticke_jednotkyDataTable EnrgUnits { get; }
        public FoodsDataSet.surovinyDataTable Foods { get; }
        public FoodsDataSet.slozky_surovinDataTable FoodIngredients { get; }
        public FoodsDataSet.slozkyDataTable Ingredients { get; }

        // Properties to hold the current food details.
        public string CurrentFoodTypeId { get; }
        public string CurrentTimeUnitId { get; }
        public string CurrentEnrgUnitId { get; }
        public float  CurrentPrepTime { get; }
        public string CurrentFoodType { get; }
        public string CurrentPreparedFood { get; }
        public string CurrentPrepUnit { get; }
        public float  CurrentEnrg { get; }
        public string CurrentEnrgUnit { get; }
        public string CurrentPrep { get; }
    }

    /**
     * Extension methods for DataTable and String classes.
     */
    public static class DataTableExtensions
    {
        /**
         * Converts a DataTable to a List of Dictionaries.
         */
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

    /**
     * Extension methods for String class.
     */
    public static class StringExtensions
    {
        /**
         * Extracts content between specified tags in the format [@tagName]...[/@tagName].
         * @param text     - The input string containing the tags.
         * @param tagName  - The name of the tag to extract content from.
         * @return The content between the specified tags, or an empty string if not found.
         */
        public static string GetFirstTagContent(this string text, string tagName)
        {
            // "\[@TEXT\](.*?)\[/@TEXT]"
            string regexPattern = $"\\[@{tagName}\\](.*?)\\[/@{tagName}\\]";
            
            // Patch: replace new lines.
            text = Regex.Replace(text, @"\r?\n", " ");

            var match = Regex.Match(text, regexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            string content =  match.Success ? match.Groups[1].Value.Trim() : string.Empty;
            return content;
        }
    }
}
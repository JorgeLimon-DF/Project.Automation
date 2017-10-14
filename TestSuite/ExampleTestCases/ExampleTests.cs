using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Shared.Data;
using Shared.Functions;
using System.Data;
using Shared.Objects;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestSuite
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public User(string sUsername, string sPassword)
        {
            Username = sUsername;
            Password = sPassword;
        }
    }

    [TestClass]
    public class LoginTests
    {
        #region Class variables

        // Users dictionary
        private Dictionary<string, User> Users;

        // Configuration
        private ExcelReader reader;
        private static Dictionary<string, DataTable> configDatatables;
        private static Dictionary<string, DataTable> dataDatatables;
        private string configFile;
        private string dataFile;
        private SharedFunctions sharedFunctions;

        #endregion

        #region Setup and Tear Down

        [TestInitialize]
        public void setup()
        {
            // Set Configuration Variables and objects
            sharedFunctions = new SharedFunctions();

            // Read Config File
            configFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDi‌​rectory, "..\\..\\..\\Shared.Data\\Config.xlsx"));
            reader = new ExcelReader(configFile);
            configDatatables = reader.GetSheetsDataTables();
            dataFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDi‌​rectory, "..\\..\\ExampleTestCases\\ExampleTestData.xlsx"));
            reader = new ExcelReader(dataFile);
            dataDatatables = reader.GetSheetsDataTables();

            // Fetch Users
            Users = new Dictionary<string, User>();
            for (int i = 0; i < configDatatables["Users"].Rows.Count; i++)
            {
                Users.Add(configDatatables["Users"].Rows[i]["Type"].ToString(),
                          new User(configDatatables["Users"].Rows[i]["Username"].ToString(),
                                   configDatatables["Users"].Rows[i]["Password"].ToString()));
            }
        }

        [TestCleanup]
        public void tearDown()
        {

        }

        #endregion

        #region Test Scripts

        private void validLoginTestScript(int configIndex)
        {
            // Configuration Variables
            string selectedBrowser = configDatatables["DriverConfig"].Rows[configIndex]["Browser"].ToString();
            string environment = configDatatables["DriverConfig"].Rows[configIndex]["Environment"].ToString();

            // Initialize variables
            IList<string> testLog = new List<string>();
            OpenBrowser browser;
            string testLogName;
            string testLogPath;
            string userType;
            bool testPassed;
            LoginPage loginPage;
            HomePage homePage;

            // For each row in the "ValidLogin" sheet...
            for (int x = 0; x < dataDatatables["ValidLogin"].Rows.Count; x++)
            {
                // Initialize variables for this test
                testLog.Clear();
                testPassed = true;
                testLogName = selectedBrowser.ToLower() + "_" + "ValidLoginTest" + "_Row" + (x + 2) + ".txt";
                string logTitle = "------------------ " + selectedBrowser.ToLower() + " \"Valid Login\" test - Row " + (x + 2) + " ------------------";
                testLog.Add(logTitle);

                // Fetch test data
                testLog.Add("Fetching test data...");
                userType = dataDatatables["ValidLogin"].Rows[x]["User"].ToString();

                // Open browser
                browser = new OpenBrowser(environment, selectedBrowser);

                // Create an instance of LoginPage and wait for it to load
                loginPage = new LoginPage(browser.Driver);

                // Set Username, Password and click on selected button.
                if (loginPage.login(Users[userType].Username, Users[userType].Password, testLog))
                {
                    testLog.Add("Login steps completed for " + userType + " user.");
                }
                else
                {
                    testLog.Add("Login steps failed for " + userType + " user.");
                    testPassed = false;
                }

                // Load Home page instance and wait for it to finish loading.
                homePage = new HomePage(browser.Driver);

                /*
                // Wait for loading spinner to appear and disappear
                if (!sharedFunctions.waitForObject(homePage.spinner, 1000, 10000))
                {
                    testPassed = false;
                }
                if (!sharedFunctions.waitForObjectDisappears(homePage.spinner, 1000, 10000))
                {
                    testPassed = false;
                }
                */

                // Close Browser
                browser.Driver.Close();
                browser.Driver.Quit();

                // Delete old testLogs
                testLogPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDi‌​rectory, "..\\..\\TestLogs\\"));
                sharedFunctions.deleteTestLogs(testLogPath, testLogName);

                // Add the passed or failed tag to the log file
                testLogName = testPassed ? "Passed_" + testLogName : testLogName = "Failed_" + testLogName;
                testLogPath += testLogName;

                // Save test log in the corresponding text file
                TextWriter textWriter = new StreamWriter(testLogPath, false);
                foreach (string entry in testLog)
                {
                    textWriter.WriteLine(entry);
                }
                textWriter.Close();

                // Assertion for this individual test
                Assert.IsTrue(testPassed);
            }
        }

        private void HomePageDislayedTestScript(int configIndex)
        {
            //-------------Variables----------

            //obtener browser y URL
            string selectedBrowser = configDatatables["DriverConfig"].Rows[configIndex]["Browser"].ToString();
            string environment = configDatatables["DriverConfig"].Rows[configIndex]["Environment"].ToString();
            OpenBrowser browser;
            HomePage HomePage;
            // Initialize variables
            IList<string> testLog = new List<string>();
            string testLogName;
            string testLogPath;
            bool testPassed;

            // Initialize variables for this test
            testLog.Clear();
            testPassed = true;
            testLogName = selectedBrowser.ToLower() + "_" + "VerifyTextinHome" + ".txt";
            string logTitle = "------------------ " + selectedBrowser.ToLower() + " \"VerifyTextinHome\" test " + " ------------------";
            testLog.Add(logTitle);

            // Open browser
            browser = new OpenBrowser(environment, selectedBrowser);

            // Verify the correct text is displayed.
            HomePage = new HomePage(browser.Driver);
            if (HomePage.VerifyTextHome() == true)
            {
                Debug.WriteLine("El texto es desplegado correctamente");
            }
            else
            {
                Debug.WriteLine("El texto no esta funcionando");
                testPassed = false;
            }

            // Close Browser
            browser.Driver.Close();
            browser.Driver.Quit();


            Assert.IsTrue(testPassed);
        }

        private void AboutUsPageDisplayedTestScript(int configIndex)
        {
            //-------------Variables----------

            //obtener browser y URL
            string selectedBrowser = configDatatables["DriverConfig"].Rows[configIndex]["Browser"].ToString();
            string environment = configDatatables["DriverConfig"].Rows[configIndex]["Environment"].ToString();
            OpenBrowser browser;
            AboutUsPage AboutUsPage;

            // Initialize variables
            IList<string> testLog = new List<string>();
            string testLogName;
            string testLogPath;
            bool testPassed;

            // Initialize variables for this test
            testLog.Clear();
            testPassed = true;
            testLogName = selectedBrowser.ToLower() + "_" + "VerifyTextinHome" + ".txt";
            string logTitle = "------------------ " + selectedBrowser.ToLower() + " \"VerifyTextinHome\" test " + " ------------------";
            testLog.Add(logTitle);

            // Open browser
            browser = new OpenBrowser(environment, selectedBrowser);

            // Verify the correct text is displayed.
            AboutUsPage = new AboutUsPage(browser.Driver);

            AboutUsPage.AboutUsLink.Click();      
                  
           
            if (AboutUsPage.AboutUsVerifyText() == true)
            {
                Debug.WriteLine("About us page se despliega correctamente");
            }
            else
            {
                Debug.WriteLine("About us page no funciona");
                testPassed = false;
            }

            // Close Browser
            browser.Driver.Close();
            browser.Driver.Quit();


            Assert.IsTrue(testPassed);
        }

        #endregion

        #region Tests

        [TestMethod]
        public void ParallelValidLogin()
        {
            // Obtain the number of browsers and create task list
            int NumberOfBrowsers = configDatatables["DriverConfig"].Rows.Count;
            List<Task> ParallelTests = new List<Task>();

            // For each browser...
            for (int i = 0; i < NumberOfBrowsers; i++)
            {
                // Save row number of config excel
                int configIndex = i;

                // Add (and start) parallel task for that browser and environment
                ParallelTests.Add(Task.Factory.StartNew(() => validLoginTestScript(configIndex)));
            }

            // Wait for all browsers to end their tests
            Task.WaitAll(ParallelTests.ToArray());
        }

        [TestMethod]
        public void ValidLogin()
        {

        }
        [TestMethod]
        public void HomePageDislayedTest()
        {
            HomePageDislayedTestScript(0);

        }

        [TestMethod]
        public void AboutUsPageDisplayedTest()
        {
            AboutUsPageDisplayedTestScript(0);

        }
        #endregion
    }
}

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using System.Configuration;
using System.Drawing;

namespace Shared.Functions
{

    public class SharedFunctions
    {
        #region Class atributes

        static Random random = new Random();

        public IWebDriver driver { get; set; }

        #endregion

        #region Test Log

        // Test name format: <Browser.toUpper()>_<testName>_Row<rowNumber>.txt
        public void deleteTestLogs(string sourceDir, string testName)
        {
            // Get path to old posible logs
            string oldPassedTest = sourceDir + "Passed_" + testName;
            string oldFailedTest = sourceDir + "Failed_" + testName;

            // Delete files
            if (File.Exists(oldPassedTest)) File.Delete(oldPassedTest);
            if (File.Exists(oldFailedTest)) File.Delete(oldFailedTest);
        }

        public void createTestLog(string testCasePath, string text = "Test Case Execution Steps")
        {
            if (!File.Exists(testCasePath))
            {
                using (StreamWriter streamWriter = File.CreateText(testCasePath))
                    streamWriter.WriteLine("====== Test Case:" + text + " ======");
            }
            else if (File.Exists(testCasePath))
            {
                TextWriter textWriter = new StreamWriter(testCasePath, true);
                textWriter.WriteLine(text);
                textWriter.Close();
            }
        }

        public bool readTestLog(string testCasePath)
        {
            bool validationFlag;
            int counter = 0;
            int failureCounter = 0;
            int passCounter = 0;
            int executedSteps = 0;
            string line;
            validationFlag = true;
            // Read the file and display it line by line.
            StreamReader file = new StreamReader(testCasePath);

            while ((line = file.ReadLine()) != null)
            {
                counter++;
                if (line.Contains("FAIL"))
                {
                    validationFlag = false;
                    failureCounter = failureCounter + 1;
                }
                if (line.Contains("PASS"))
                {
                    passCounter = passCounter + 1;
                }
            }
            executedSteps = counter - 1;
            file.Close();
            TextWriter textWriter = new StreamWriter(testCasePath, true);
            textWriter.WriteLine("Executed Steps:" + executedSteps + " Steps Failed:" + failureCounter + " Successful Steps:" + passCounter);
            textWriter.Close();

            return validationFlag;

        }

        public void createGlobalLog(string globalLogPath, int passedTests, int failedTests)
        {
            int totalTests = passedTests + failedTests;
            if (!File.Exists(globalLogPath))
            {
                using (StreamWriter streamWriter = File.CreateText(globalLogPath))
                    streamWriter.WriteLine("<!DOCTYPE html> " +
                      "<html>" +
                      "<body style=background - color:white;>" +
                      "<h1 style=color:blue;><center>Blitz Business Trial</center></h1>" +
                      "<h1 style=color:blue;><center>Tests Execution Report</center></h1>" +
                      "<style>" +
                      "table {font - family: arial, sans - serif;border - collapse: collapse;width: 100 %;}" +
                      "td, th {border: 2px solid #dddddd;text - align: left;padding: 8px;}" +
                      "tr: nth - child(even) { background - color: white; }" +
                      "</style> " +
                      "<center>" +
                      "<table style = width:50%>" +
                      "<caption><b> Executed Tests </b></caption>" +
                      "<tr>" +
                      "<td><b> Passed </b></td>" +
                      "<td bgcolor=green><b>" + passedTests + "</b></td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td><b> Failed </b></td>" +
                      "<td bgcolor=red><b>" + failedTests + "</b></td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td><b>Total</b></td>" +
                      "<td><b>" + totalTests + "</b></td>" +
                      "</tr>" +
                      "</table>" +
                      "</center>" +
                      "<br>" +
                      "<br>");
            }
            else if (File.Exists(globalLogPath))
            {
                TextWriter textWriter = new StreamWriter(globalLogPath, true);
                textWriter.Close();
            }
        }

        public void SendResults(string report, string mailFrom, string Subject, string mailTo)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.sendgrid.net");

                mail.From = new MailAddress(mailFrom);
                mail.To.Add(mailTo);
                mail.Subject = Subject;
                mail.IsBodyHtml = true;

                string htmlBody;

                string getHTML = File.ReadAllText(report);

                htmlBody = getHTML;

                mail.Body = htmlBody;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("azure_a183a7f6efc909dd23e67f9e02d41411@azure.com", "m8vz7\\)EUuhq~7hK");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Waits

        /// <summary>
        /// Method to wait for an already defined element by IWebElement
        /// </summary>
        /// <param name="element"></param>
        public bool waitForObject(IWebElement obj, int sleepMilis, int maxWaitMilis)
        {
            bool isVisible = false;
            Point actualPosition = new Point();
            Point updatedPosition = new Point();

            do
            {
                try
                {
                    actualPosition = obj.Location;
                    isVisible = obj.Displayed;
                    Thread.Sleep(100);
                    updatedPosition = obj.Location;
                }
                catch (Exception)
                {
                    Thread.Sleep(sleepMilis);
                    maxWaitMilis -= sleepMilis + 100;
                }

            } while ((!isVisible && maxWaitMilis > 0) || !actualPosition.Equals(updatedPosition));

            return isVisible;
        }

        public bool waitForObjectDisappears(IWebElement obj, int sleepMilis, int maxWaitMilis)
        {
            bool isGone = false;

            do
            {
                try
                {
                    isGone = !obj.Displayed;
                    if (!isGone)
                    {
                        isGone = !obj.Displayed;
                        Thread.Sleep(sleepMilis);
                        maxWaitMilis -= sleepMilis;
                    }
                }
                catch (Exception)
                {
                    isGone = true;
                }

            } while (!isGone && maxWaitMilis > 0);

            return isGone;
        }

        #endregion

        #region Common routines

        /** clickButton
         * Selenium click routine with exception management, validation, log, and timeout.
         * 
         * @param button: Button to click
         * @param buttonName: Button name or text
         * @param success: Message to log when success
         * @param failure: Message to log when failure
         * @param elementToWait: Element to wait after click
         * @param waitToAppear: If true, wait for element to appear. If false, wait for element to disappear.
         * @param timeout: Miliseconds to wait before throwing a timeout exception
         * @param testLog: Test log string list
         */
        public bool clickButton(IWebElement button, string buttonName, string success, string failure, IWebElement elementToWait, bool waitToAppear, int timeout, IList<string> testLog)
        {
            // ToLower name for log aesthetics
            buttonName.ToLower();

            // Wait for object
            if (!waitForObject(button, 1000, timeout))
            {
                testLog.Add("Timeout waiting for \"" + buttonName + "\" to appear.");
                testLog.Add(failure);
                return false;
            }

            // Attempt click
            try
            {
                button.Click();
            }
            catch (Exception)
            {
                testLog.Add("Exception when clicking " + buttonName);
                return false;
            }

            // Validation
            if (waitToAppear)
            {
                // If elementToWait appears on time
                if (waitForObject(elementToWait, 1000, timeout))
                {
                    // Success
                    testLog.Add(success);
                    return true;
                }
                else
                {
                    // Failure
                    testLog.Add("Timeout after clicking " + buttonName);
                    testLog.Add(failure);
                    return false;
                }
            }
            else
            {
                // If elementToWait disappears on time
                if (waitForObjectDisappears(elementToWait, 1000, timeout))
                {
                    // Success
                    testLog.Add(success);
                    return true;
                }
                else
                {
                    // Failure
                    testLog.Add("Timeout after clicking " + buttonName);
                    testLog.Add(failure);
                    return false;
                }
            }
        }

        /** setCheckboxValue
         * Selenium set checkbox value routine with exception management, validation, log, and timeout.
         * 
         * @param checkbox: Checkbox web element
         * @param checkboxName: Checkbox label
         * @param value: true || false
         * @param success: Message to log when success
         * @param failure: Message to log when failure
         * @param timeout: Miliseconds to wait before throwing a timeout exception
         * @param testLog: Test log string list
         */
        public bool setCheckboxValue(IWebElement checkbox, string checkboxName, bool value, string success, string failure, int timeout, IList<string> testLog)
        {
            // ToLower name for log aesthetics
            checkboxName.ToLower();

            // Wait for object
            if (!waitForObject(checkbox, 1000, timeout))
            {
                testLog.Add("Timeout waiting for \"" + checkboxName + "\" to appear.");
                testLog.Add(failure);
                return false;
            }

            // Attempt to set checkbox value
            try
            {
                // Click if value isn't set already
                if (Boolean.Parse(checkbox.GetAttribute("checked")) != value)
                {
                    checkbox.Click();
                }
            }
            catch (Exception)
            {
                testLog.Add("Exception when setting " + checkboxName + " to " + value.ToString());
                return false;
            }

            // Validation
            if (Boolean.Parse(checkbox.GetAttribute("checked")) == value)
            {
                // Success
                testLog.Add(success);
                return true;
            }
            else
            {
                // Failure
                testLog.Add(failure);
                return false;
            }
        }

        /** setTextInTextField
         * Selenium "SendKeys" routine for text input with exception management, validation, log, and timout.
         * 
         * @param input: Text box/field web element
         * @param inputName: Name of text box/field
         * @param text: Text to input
         * @param success: Message to log when success
         * @param failure: Message to log when failure
         * @param timeout: Miliseconds to wait before throwing a timeout exception
         * @param testLog: Test log string list
         */
        public bool setInputText(IWebElement input, string inputName, string text, string success, string failure, int timeout, IList<string> testLog)
        {
            // ToLower name for log aesthetics
            inputName.ToLower();

            // Wait for object
            if (!waitForObject(input, 1000, timeout))
            {
                testLog.Add("Timeout waiting for \"" + inputName + "\" to appear.");
                testLog.Add(failure);
                return false;
            }

            // Attempt to clear input and type text
            try
            {
                input.Clear();
                input.SendKeys(text);
            }
            catch (Exception)
            {
                testLog.Add("Exception when setting \"" + text + "\" in " + inputName);
                return false;
            }

            // Validation
            if (input.GetAttribute("value") == text)
            {
                // Success
                testLog.Add(success);
                return true;
            }
            else
            {
                // Failure
                testLog.Add(failure);
                return false;
            }
        }

        #endregion

        #region Misc

        /// <summary>
        /// Function to generate a random string
        /// </summary>
        /// <param name="size"></param>
        /// <param name="lowerCase"></param>
        /// <returns></returns>
        public string RandomString(int size, bool lowerCase = true)
        {
            StringBuilder builder = new StringBuilder();
            char ch;

            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            if (lowerCase)
            {
                return builder.ToString().ToLower();
            }

            return builder.ToString();
        }

        #endregion
    }

}



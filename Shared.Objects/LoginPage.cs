using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Shared.Functions;
using System.Collections.Generic;

namespace Shared.Objects
{
    public class LoginPage
    {
        #region Web Elements and class attributes

        // Class attributes
        public SharedFunctions sharedFunctions;

        // Web Elements
        [FindsBy(How = How.Id, Using = "UserName")]
        public IWebElement userTextInput { get; set; }

        [FindsBy(How = How.Id, Using = "Password")]
        public IWebElement passwordTextInput { get; set; }

        [FindsBy(How = How.CssSelector, Using = @"#loginForm > form > fieldset > input.btn.btn-secondary")]
        public IWebElement loginButton { get; set; }

        #endregion

        // Constructor
        public LoginPage(IWebDriver Driver)
        {
            PageFactory.InitElements(Driver, this);
            ImplicitWait implicitWait = new ImplicitWait(Driver);
            sharedFunctions = new SharedFunctions();
        }

        #region Possible actions

        // Login method
        public bool login(string user, string password, IList<string> testLog)
        {
            testLog.Add("******** Login ********");

            if (!sharedFunctions.setInputText(userTextInput,
                                         "Username textbox",
                                         user,
                                         "Set " + user + " in username textbox.",
                                         "Failed to set " + user + " in username textbox.",
                                         10000,
                                         testLog)) { return false; }

            if (!sharedFunctions.setInputText(passwordTextInput,
                                         "Password textbox",
                                         password,
                                         "Set " + password + " in password textbox.",
                                         "Failed to set " + password + " in password textbox.",
                                         10000,
                                         testLog)) { return false; }


            if (!sharedFunctions.clickButton(loginButton,
                                            "Login button",
                                            "Click in login button.",
                                            "Failed when clicking login button.",
                                            loginButton,
                                            false,
                                            10000,
                                            testLog)) { return false; }

            return true;
        }

        #endregion
    }
}

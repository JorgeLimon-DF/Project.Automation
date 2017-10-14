using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using Shared.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Objects
{
    public class AboutUsPage
    {
      

        // Class Attributes
        public SharedFunctions sharedFunctions;
        
        // Constructor
        public AboutUsPage(IWebDriver Driver)
        {
            PageFactory.InitElements(Driver, this);
            ImplicitWait implicitWait = new ImplicitWait(Driver);
            sharedFunctions = new SharedFunctions();
         }
        #region Web Elements and class attributes

            // Web Elements
        [FindsBy(How = How.CssSelector, Using = "#nav-menu > li:nth-child(1) > a > span")]
        public IWebElement AboutUsLink { get; set; }

        [FindsBy(How = How.CssSelector, Using = "#column_main > h5.interiorSubtitle")]
        public IWebElement AboutUsText { get; set; }
        public bool AboutUsVerifyText()
        {
            if (AboutUsText.Text == "Exceptional commitment to client success")
            {
                return true;
            }
             
               else
        {
            return false;
        }
    }
    #endregion
}
    #region Possible actions
   
    #endregion
}

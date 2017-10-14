using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using System.Threading;
using Shared.Data;
using Shared.Functions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Extensions;

namespace Shared.Functions
{
    public class OpenBrowser
    {
        public IWebDriver Driver
        {
            get; set;
        }

        public OpenBrowser(string url, string browser = "Firefox")
        {
            browser = browser.ToString().ToUpper();
            switch (browser)
            {
                case "FIREFOX":
                    FirefoxProfile firefoxProfile = new FirefoxProfile();
                    firefoxProfile.AcceptUntrustedCertificates = true;
                    firefoxProfile.SetPreference("dom.max_script_run_time", 0);
                    Driver = new FirefoxDriver(firefoxProfile);
                    Driver.Navigate().GoToUrl(url);
                    break;
                case "CHROME":
                    Driver = new ChromeDriver();
                    Driver.Manage().Window.Maximize();
                    Driver.Manage().Cookies.DeleteAllCookies();
                    Driver.Navigate().GoToUrl(url);
                    break;
                case "IE":
                    InternetExplorerOptions IECapabilities = new InternetExplorerOptions();
                    IECapabilities.InitialBrowserUrl = url;
                    IECapabilities.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                    IECapabilities.IgnoreZoomLevel = true;
                    Driver = new InternetExplorerDriver(IECapabilities);
                    Driver.Manage().Cookies.DeleteAllCookies();
                    Driver.Manage().Window.Maximize();
                    break;
            }
            //Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
        }

    }
}

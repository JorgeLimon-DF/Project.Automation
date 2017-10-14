using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace Shared.Functions
{
    public class OpenBrowser
    {
        public IWebDriver Driver
        {
            get; set;
        }

        //public /*string ProfileDirectoryName*/{ get; set; }

        public OpenBrowser(string url, string browser = "Firefox", string profileDirectoryName = null)
        {
            browser = browser.ToString().ToUpper();
            switch (browser)
            {
                case "FIREFOX":
                    FirefoxProfile firefoxProfile = string.IsNullOrEmpty(profileDirectoryName)
                        ? new FirefoxProfile() : new FirefoxProfile(profileDirectoryName);
                    firefoxProfile.AcceptUntrustedCertificates = true;
                    firefoxProfile.SetPreference("dom.max_script_run_time", 0);
                    Driver = new FirefoxDriver(firefoxProfile);
                    Driver.Navigate().GoToUrl(url);
                    break;
                case "CHROME":
                    Driver = string.IsNullOrEmpty(profileDirectoryName)
                        ? new ChromeDriver() : new ChromeDriver(profileDirectoryName);
                    Driver.Manage().Window.Maximize();
                    Driver.Manage().Cookies.DeleteAllCookies();
                    Driver.Navigate().GoToUrl(url);
                    break;
                case "IE":
                    InternetExplorerOptions IECapabilities = new InternetExplorerOptions();
                    IECapabilities.InitialBrowserUrl = url;
                    IECapabilities.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                    IECapabilities.IgnoreZoomLevel = true;
                    Driver = string.IsNullOrEmpty(profileDirectoryName)
                        ? new InternetExplorerDriver(IECapabilities)
                        : new InternetExplorerDriver(profileDirectoryName, IECapabilities);
                    Driver.Manage().Cookies.DeleteAllCookies();
                    Driver.Manage().Window.Maximize();
                    break;
            }
            //Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
        }

    }
}

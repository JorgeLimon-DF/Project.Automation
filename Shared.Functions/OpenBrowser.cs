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

        public OpenBrowser(string url, string browser = "Firefox", string driverDirectoryLocation = null)
        {
            browser = browser.ToString().ToUpper();
            switch (browser)
            {
                case "FIREFOX":
                    var service = FirefoxDriverService.CreateDefaultService(driverDirectoryLocation);
                    var options = new FirefoxOptions();
                    options.Profile = new FirefoxProfile();
                    options.Profile.AcceptUntrustedCertificates = true;
                    options.Profile.SetPreference("dom.max_script_run_time", 0);
                    Driver = new FirefoxDriver(service, options, System.TimeSpan.FromSeconds(20));
                    Driver.Navigate().GoToUrl(url);
                    break;
                case "CHROME":
                    Driver = string.IsNullOrEmpty(driverDirectoryLocation)
                        ? new ChromeDriver() : new ChromeDriver(driverDirectoryLocation);
                    Driver.Manage().Window.Maximize();
                    Driver.Manage().Cookies.DeleteAllCookies();
                    Driver.Navigate().GoToUrl(url);
                    break;
                case "IE":
                    InternetExplorerOptions IECapabilities = new InternetExplorerOptions();
                    IECapabilities.InitialBrowserUrl = url;
                    IECapabilities.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                    IECapabilities.IgnoreZoomLevel = true;
                    Driver = string.IsNullOrEmpty(driverDirectoryLocation)
                        ? new InternetExplorerDriver(IECapabilities)
                        : new InternetExplorerDriver(driverDirectoryLocation, IECapabilities);
                    Driver.Manage().Cookies.DeleteAllCookies();
                    Driver.Manage().Window.Maximize();
                    break;
            }
        }
    }
}

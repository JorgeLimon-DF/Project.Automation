using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using OpenQA.Selenium.Chrome;

namespace Shared.Functions
{
    public class ImplicitWait
    {
        public ImplicitWait(IWebDriver _driver)
        {
            driver = _driver;
        }

        private IWebDriver driver { get; set; }

        // Method to find element
        public IWebElement FindElement(By by, int seconds = 5)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));

            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(by));
                var element = wait.Until(c =>
                {
                    var elements = driver.FindElements(by);
                    if (elements.Count > 0)
                        return elements[0];
                    return null;
                });

                return element;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Waiting for an element
        public IWebElement WaitForElement(IWebElement element, int timeoutInSeconds = 5)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => element);
            }
            return element;
        }
    }
}

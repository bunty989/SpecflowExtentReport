using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SpecFlowExtentReport.Framework
{
    public class DriverHelper
    {
        public IWebDriver Driver = null;
                
        public IWebDriver CreateDriver(string browser = null)
        {
            browser ??= "CHROME";

            switch (browser.ToUpperInvariant())
            {
                case "CHROME":
                    {
                        ChromeOptions option = new ChromeOptions();
                        option.AddArguments("start-maximized");
                        option.AddArguments("--disable-gpu");
                        option.AddExcludedArgument("enable-automation");
                        option.AddAdditionalCapability("useAutomationExtension", false);
                        option.AddUserProfilePreference("credentials_enable_service", false);
                        option.AddUserProfilePreference("profile.password_manager_enabled", false);
                        new DriverManager().SetUpDriver(new ChromeConfig());
                        Console.WriteLine("Setup");
                        Driver = new ChromeDriver(option);
                        return Driver;
                    }

                case "FIREFOX":
                    Driver = new FirefoxDriver();
                    return Driver;
                case "IE":
                    Driver = new InternetExplorerDriver();
                    return Driver;
                default:
                    throw new ArgumentException($"Browser not yet implemented: {browser}");
            }
        }

        public void NavigateTo(string url)
        {
            Driver.Manage()
                   .Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            Driver.Navigate().GoToUrl(url);
        }

        public void QuitDriver()
        {
            Driver.Quit();
        }

    }
}

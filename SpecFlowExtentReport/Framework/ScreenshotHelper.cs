using OpenQA.Selenium;
using System;
using System.IO;
using TechTalk.SpecFlow;

namespace SpecFlowExtentReport.Framework
{
    public class ScreenshotHelper
    {
        private DriverHelper _driverHelper;

        public ScreenshotHelper(DriverHelper driverHelper) { _driverHelper = driverHelper; }

        private IWebDriver Driver => _driverHelper.Driver;
        public string ReturnScreenCapturePath(ScenarioContext _scenario)
        {
            var ss = ((ITakesScreenshot)Driver).GetScreenshot();
            var scenarioName = _scenario.ScenarioInfo.Title.ToString();
            var screenShotName = _scenario.StepContext.StepInfo.Text.ToString();
            string assemblyFolder = Path.GetDirectoryName(Directory.GetParent(@"../../").FullName);
            string finalpth = assemblyFolder + "\\Reports\\Screenshots\\" + scenarioName;
            if (!Directory.Exists(finalpth)) { Directory.CreateDirectory(finalpth); }
            string screenshotDir = finalpth + "\\" + screenShotName;
            string localpath = new Uri(screenshotDir).LocalPath+"_" +DateTime.Now.ToString("ddMMyyyhhmmss")+ ".png";
            ss.SaveAsFile(localpath, ScreenshotImageFormat.Png);
            return localpath;
        }
    }
}

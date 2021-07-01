using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using NUnit.Framework;
using SpecFlowExtentReport.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlowExtentReport.Hooks
{
    [Binding]
    public sealed class SpecflowHooks
    {
        private DriverHelper _driverHelper;
        ScreenshotHelper _screenshotHelper;
        public SpecflowHooks(DriverHelper driverHelper) { 
            _driverHelper = driverHelper;
            _screenshotHelper = new ScreenshotHelper(_driverHelper);
        }
        private static ExtentTest feature, scenario, step;
        private static ExtentReports extent;
        private static string reportpath => Path.Combine(Directory.GetParent(@"../../../").FullName, "Extent.html");

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext context)
        {
            scenario = feature.CreateNode<Scenario>(context.ScenarioInfo.Title);
            _driverHelper.CreateDriver();
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            ExtentHtmlReporter htmlReport = new ExtentHtmlReporter(reportpath);
            htmlReport.LoadConfig(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName+"\\ExtentConfig.xml");
            extent = new ExtentReports();
            extent.AddSystemInfo("Host Name", Environment.MachineName);
            extent.AddSystemInfo("Environment", "YourQAEnvironment");
            extent.AddSystemInfo("Domain", Environment.UserDomainName);
            extent.AddSystemInfo("Username", Environment.UserName);
            extent.AttachReporter(htmlReport);
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext context)
        {
            feature = extent.CreateTest<Feature>(context.FeatureInfo.Title);
        }

        [BeforeStep]
        public void BeforeStep()
        {
            step = scenario;
        }

        [AfterStep]
        public void AfterStep(ScenarioContext context)
        {
            var stepType = context.StepContext.StepInfo.StepDefinitionType.ToString() + " ";
            if (context.TestError == null)
            {
                switch (stepType.ToUpper().Trim()) 
                {
                    case "GIVEN":
                        {
                            step.CreateNode<Given>(context.StepContext.StepInfo.Text).Pass("",AttachScreenshot(context));
                            break;
                        }
                    case "WHEN": 
                        {
                            step.CreateNode<When>(context.StepContext.StepInfo.Text).Pass("", AttachScreenshot(context));
                            break;
                        }
                    case "THEN":
                        {
                            step.CreateNode<Then>(context.StepContext.StepInfo.Text).Pass("", AttachScreenshot(context));
                            break;
                        }
                    case "AND":
                        {
                            step.CreateNode<And>(context.StepContext.StepInfo.Text).Pass("", AttachScreenshot(context));
                            break;
                        }
                }
                //step.Log(Status.Pass, stepType + context.StepContext.StepInfo.Text);
            }
            else if (context.TestError != null)
            {
                switch (stepType.ToUpper().Trim())
                {
                    case "GIVEN":
                        {
                            step.CreateNode<Given>(context.StepContext.StepInfo.Text).Fail(context.TestError.Message, AttachScreenshot(context));
                            break;
                        }
                    case "WHEN":
                        {
                            step.CreateNode<When>(context.StepContext.StepInfo.Text).Fail(context.TestError.Message, AttachScreenshot(context));
                            break;
                        }
                    case "THEN":
                        {
                            step.CreateNode<Then>(context.StepContext.StepInfo.Text).Fail(context.TestError.Message, AttachScreenshot(context));
                            break;
                        }
                    case "AND":
                        {
                            step.CreateNode<And>(context.StepContext.StepInfo.Text).Fail(context.TestError.Message, AttachScreenshot(context));
                            break;
                        }
                }
                //step.Log(Status.Pass, stepType + context.StepContext.StepInfo.Text);
            }
        }

        [AfterFeature]
        public static void AfterFeature()
        {
            extent.Flush();
        }
        [AfterScenario]
        public void AfterScenario()
        {
            _driverHelper.QuitDriver();
        }

        private MediaEntityModelProvider AttachScreenshot(ScenarioContext context)
        {
            var dirPath = _screenshotHelper.ReturnScreenCapturePath(context);
            return MediaEntityBuilder.CreateScreenCaptureFromPath(dirPath).Build();
        }
    }
}

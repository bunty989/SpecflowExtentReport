using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SpecFlowExtentReport.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        [ThreadStatic]
        private static ExtentTest feature, scenario, step;
        private static ExtentReports extent;
        private static string reportpath => Path.Combine(Directory.GetParent(@"../../../").FullName, "Extent.html");
        public static ConfigHelper Config; 
        private static string configpath => Path.Combine(Directory.GetParent(@"../../../").FullName, "appsettings.json");

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext context)
        {
            scenario = feature.CreateNode<Scenario>(context.ScenarioInfo.Title);
            _driverHelper.CreateDriver();
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            /*var _config = new ConfigurationBuilder()
                .AddJsonFile(configpath, optional: false, reloadOnChange: true)
                .Build();
            _config.Bind(Config);*/
            Config = new ConfigHelper();
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(configpath, optional: false, reloadOnChange: true);
            var configuration = builder.Build();
            configuration.Bind(Config);
            ExtentHtmlReporter htmlReport = new(reportpath);
            htmlReport.LoadConfig(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName+ Path.DirectorySeparatorChar+Config.ExtentConfigPath);
            extent = new ExtentReports();
            Dictionary<string, string> sysInfo = new()
            {
                {"Host Name", Environment.MachineName},
                {"Environment", "YourQAEnvironment" },
                {"Domain", Environment.UserDomainName },
                {"Username", Environment.UserName }
            };
            foreach (var info in sysInfo) { extent.AddSystemInfo(info.Key,info.Value); }
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
                            step.CreateNode<Given>(context.StepContext.StepInfo.Text).Pass(stepType.Trim(), AttachScreenshot(context));
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

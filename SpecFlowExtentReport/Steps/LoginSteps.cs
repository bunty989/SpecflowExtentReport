using NUnit.Framework;
using OpenQA.Selenium;
using SpecFlowExtentReport.Framework;
using SpecFlowExtentReport.Hooks;
using SpecFlowExtentReport.Pages;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
[assembly: LevelOfParallelism(4)]
[assembly: Parallelizable(ParallelScope.Children)]


namespace SpecFlowExtentReport.Steps
{
    [Binding]
    public sealed class LoginSteps
    {
        private readonly ScenarioContext _scenarioContext;
        HomePage homePage;
        LoginPage loginPage;
        private DriverHelper _driverHelper;
        private IWebDriver driver => _driverHelper.Driver;
        public LoginSteps(ScenarioContext scenarioContext, DriverHelper driverHelper)
        {
            _scenarioContext = scenarioContext;
            _driverHelper = driverHelper;
            homePage = new HomePage(driver);
            loginPage = new LoginPage(driver);
        }

        [Given(@"I navigate to application")]
        public void GivenINavigateToApplication()
        {
            _driverHelper.NavigateTo(SpecflowHooks.Config.ApplicationUrl);
        }

        [Given(@"I click the Login link")]
        public void GivenIClickTheLoginLink()
        {
            homePage.ClickLogin();
        }

        [Given(@"I enter username and password")]
        public void GivenIEnterUsernameAndPassword(Table table)
        {
            dynamic data = table.CreateDynamicInstance();
            loginPage.EnterUserNameAndPassword(data.UserName, data.Password);
        }

        [When(@"I click login")]
        public void WhenIClickLogin()
        {
            loginPage.ClickLogin();
        }

        [Then(@"I should see user is logged in to the application")]
        public void ThenIShouldSeeUserIsLoggedInToTheApplication()
        {
            Assert.IsTrue(homePage.IsLogOffExist(), "Log off button is not displayed");
        }



    }
}

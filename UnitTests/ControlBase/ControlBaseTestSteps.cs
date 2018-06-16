using Internal.Tester;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamControlium.Utilities;
using TechTalk.SpecFlow;

namespace TeamControlium.Controlium
{
    [Binding]
    public sealed class ControlBaseTestSteps
    {
        [Given(@"I am using local browser ""(?i)(.*)""")]
        public void GivenIAmUsingBrowser(string browser)
        {
            TestData.Repository["Selenium", "Browser"] = browser;
            TestData.Repository["Selenium", "Host"] = "localhost";
            TestData.Repository["Selenium", "SeleniumServerFolder"] = ".//..//..//TestSeleniumServer";
            TestData.Repository["Selenium", "DebugMode"] = "off";
        }

        [Given(@"I instantiate SeleniumDriver and browse to ""(.*)""")]
        public void WhenIInstantiateSeleniumDriver(string urlToBrowseTo)
        {
            var driver = new SeleniumDriver();
            driver.GotoURL(urlToBrowseTo);
            ScenarioContext.Current.Add("SeleniumDriver", driver);
        }

        [When(@"I find Textbox control with title ""(.*)""")]
        public void WhenIFindFKTextboxControlWithTitle(string controlTitle)
        {
            var driver = ScenarioContext.Current.Get<SeleniumDriver>("SeleniumDriver");
            var control = driver.SetControl(new ControlBaseTester(controlTitle));
            ScenarioContext.Current.Add("Control", control);
        }

        [When(@"enter text ""(.*)""")]
        public void WhenEnterText(string textToEnter)
        {
            ScenarioContext.Current.Get<ControlBaseTester>("Control").Text = textToEnter;
        }

        [Then(@"Control is found")]
        public void ThenControlIsFound()
        {
            Assert.IsNotNull(ScenarioContext.Current.Get<ControlBaseTester>("Control"));
        }

        [Then(@"I can read the text ""(.*)""")]
        public void ThenICanReadTheText(string stringExpected)
        {
            Assert.AreEqual(ScenarioContext.Current.Get<ControlBaseTester>("Control").Text, stringExpected);
        }
    }
}
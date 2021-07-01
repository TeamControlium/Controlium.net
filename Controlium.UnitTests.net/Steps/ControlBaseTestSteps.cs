using Internal.Tester;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamControlium.Utilities;
using TechTalk.SpecFlow;

namespace TeamControlium.Controlium
{
    [Binding]
    public sealed class ControlBaseTestSteps
    {
        /// <summary>
        /// Used to hold context information for current Scenario
        /// </summary>
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// Initialises a new instance of the <see cref="ControlBaseTestSteps" /> class.
        /// Used by Specflow supply Scenario context information.
        /// </summary>
        /// <param name="context">Scenario context data for use by class methods</param>
        public ControlBaseTestSteps(ScenarioContext context)
        {
            this.scenarioContext = context;
        }


        [Given(@"I am using local browser ""(?i)(.*)""")]
        public void GivenIAmUsingBrowser(string browser)
        {
            Repository.SetItemGlobal("Selenium", "Browser",browser);
            Repository.SetItemGlobal("Selenium", "Host","localhost");
            Repository.SetItemGlobal("Selenium", "SeleniumServerFolder",".//..//..//..//TestSeleniumServer");
            Repository.SetItemGlobal("Selenium", "DebugMode","off");
        }

        [Given(@"I instantiate SeleniumDriver and browse to ""(.*)""")]
        public void WhenIInstantiateSeleniumDriver(string urlToBrowseTo)
        {
            var driver = new SeleniumDriver();
            driver.GotoURL(urlToBrowseTo);
            scenarioContext.Add("SeleniumDriver", driver);
        }

        [When(@"I find Textbox control with title ""(.*)""")]
        public void WhenIFindFKTextboxControlWithTitle(string controlTitle)
        {
            var driver = scenarioContext.Get<SeleniumDriver>("SeleniumDriver");
            var control = driver.SetControl(new ControlBaseTester(controlTitle));
            scenarioContext.Add("Control", control);
        }

        [When(@"enter text ""(.*)""")]
        public void WhenEnterText(string textToEnter)
        {
            scenarioContext.Get<ControlBaseTester>("Control").Text = textToEnter;
        }

        [Then(@"Control is found")]
        public void ThenControlIsFound()
        {
            Assert.IsNotNull(scenarioContext.Get<ControlBaseTester>("Control"));
        }

        [Then(@"I can read the text ""(.*)""")]
        public void ThenICanReadTheText(string stringExpected)
        {
            Assert.AreEqual(scenarioContext.Get<ControlBaseTester>("Control").Text, stringExpected);
        }
    }
}
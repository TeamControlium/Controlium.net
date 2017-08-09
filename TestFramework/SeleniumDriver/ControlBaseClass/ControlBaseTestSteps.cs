using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace TeamControlium.TestFramework
{
    [Binding]
    public sealed class ControlBaseTestSteps
    {


        [Given(@"I am using local browser ""(?i)(.*)""")]
        public void GivenIAmUsingBrowser(string browser)
        {
            Utilities.TestData["Selenium", "Browser"] = browser;
            Utilities.TestData["Selenium", "Host"] = "localhost";
            Utilities.TestData["Selenium", "SeleniumServerFolder"] = ".//..//..//TestSeleniumServer";
            Utilities.TestData["Selenium", "DebugMode"] = "off";
        }

        [Given(@"I instantiate SeleniumDriver and browse to ""(.*)""")]
        public void WhenIInstantiateSeleniumDriver(string urlToBrowseTo)
        {
            SeleniumDriver sDriver = new SeleniumDriver();
            sDriver.GotoURL(urlToBrowseTo);
            ScenarioContext.Current.Add("SeleniumDriver", sDriver);
        }


        [When(@"I find Textbox control with title ""(.*)""")]
        public void WhenIFindFKTextboxControlWithTitle(string controlTitle)
        {
            SeleniumDriver sDriver = (SeleniumDriver)ScenarioContext.Current["SeleniumDriver"];
            Internal.Tester.ControlBaseTester control = sDriver.SetControl(new Internal.Tester.ControlBaseTester(controlTitle));
            ScenarioContext.Current.Add("Control", control);
        }

        [When(@"enter text ""(.*)""")]
        public void WhenEnterText(string textToEnter)
        {
            ((Internal.Tester.ControlBaseTester)ScenarioContext.Current["Control"]).Text = textToEnter;
        }

        [Then(@"Control is found")]
        public void ThenControlIsFound()
        {
            Assert.IsNotNull(ScenarioContext.Current["Control"], "Verify Control type is not null");
        }

        [Then(@"I can read the text ""(.*)""")]
        public void ThenICanReadTheText(string stringExpected)
        {
            var stringRead = ((Internal.Tester.ControlBaseTester)ScenarioContext.Current["Control"]).Text;
            Assert.AreEqual(stringRead, stringExpected, "String read from control is same as expected");
        }




    }
}

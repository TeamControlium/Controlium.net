using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace TeamControlium.Framework
{
    [Binding]
    public sealed class ControlBaseTestSteps
    {


        [Given(@"I am using local browser ""(i?)(.*)""")]
        public void GivenIAmUsingBrowser(string browser)
        {
            switch (browser)
            {
                case "chrome":
                    {
                        Utilities.TestData["Selenium", "Browser"] = "Chrome";
                        break;
                    }
            }

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


        [When(@"I find FK Textbox control with title ""(.*)""")]
        public void WhenIFindFKTextboxControlWithTitle(string p0)
        {
            SeleniumDriver sDriver = (SeleniumDriver)ScenarioContext.Current["SeleniumDriver"];
            sDriver.Set
        }

    }
}

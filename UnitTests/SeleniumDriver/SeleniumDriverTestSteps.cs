using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using TeamControlium.Utilities;
using TechTalk.SpecFlow;

namespace TeamControlium.Controlium
{
    [Binding]
    public sealed class SeleniumDriverTestSteps
    {
        [Given(@"There are no processes running named ""(.*)""")]
        public void GivenThereAreNoProcessesRunningNamed(string processNameToKill)
        {
            Process[] processes = Process.GetProcessesByName(processNameToKill);
            if (processes.Count() > 1)
            {
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                        if (!process.WaitForExit(10000))
                        {
                            Assert.Inconclusive($"Killing {processNameToKill}] (ID = {process.Id}) did not work within 10 seconds");
                        }
                    }
                    catch (Exception ex) { }
                }
            }
        }

        [Given(@"setting Category ""(.*)"", Option ""(.*)"" is ""(.*)""")]
        public void GivenSettingCategoryOptionIs(string category, string option, dynamic value)
        {
            TestData.Repository[category, option] = value;
        }

        [Given(@"I instantiate SeleniumDriver")]
        [When(@"I instantiate SeleniumDriver")]
        public void WhenIInstantiateSeleniumDriver()
        {
            SeleniumDriver sDriver = new SeleniumDriver();
            ScenarioContext.Current.Add("SeleniumDriver", sDriver);
        }

        [Given(@"I browse to ""(.*)""")]
        [When(@"I browse to ""(.*)""")]
        public void WhenIBrowseTo(string urlToBrowseTo)
        {
            ((SeleniumDriver)ScenarioContext.Current["SeleniumDriver"]).GotoURL(urlToBrowseTo);
        }

        [Then(@"a process exists named ""(.*)""")]
        public void ThenAProcessExistsNamed(string processname)
        {
            Assert.IsTrue(Process.GetProcessesByName(processname).Count() > 0, $"There must be more than one process named ({processname})");
        }

        [Then(@"I can read the page title ""(.*)""")]
        public void ThenICanReadThePageTitle(string expectedPageTitle)
        {
            string pageTitle = ((SeleniumDriver)ScenarioContext.Current["SeleniumDriver"]).PageTitle;
            Assert.AreEqual(expectedPageTitle, pageTitle);
        }

        [Given(@"I use FindElement to locate an element using XPath ""(.*)""")]
        [When(@"I use FindElement to locate an element using XPath ""(.*)""")]
        public void WhenIUseFindElementToLocateAnElementUsingXPath(string xPath)
        {
            Element foundElement;
            try
            {
                foundElement = ((SeleniumDriver)ScenarioContext.Current["SeleniumDriver"]).FindElement(new ObjectMappingDetails(xPath, "The XPath"));
                ScenarioContext.Current.Add("FoundElement", foundElement);
            }
            catch (Exception ex)
            {
                Assert.Inconclusive($"Error using FindElement method: {ex}");
            }
        }

        [Then(@"a valid element is found")]
        public void ThenAValidElementIsFound()
        {
            Assert.IsNotNull(ScenarioContext.Current["FoundElement"], "Found element must not be null");
        }
    }
}

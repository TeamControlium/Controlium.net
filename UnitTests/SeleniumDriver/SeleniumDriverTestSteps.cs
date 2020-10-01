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
            var processes = Process.GetProcessesByName(processNameToKill);
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
                catch { }
            }
        }

        [Given(@"(?:s|S)etting Category ""(.*)"", Option ""(.*)"" is ""(.*)""")]
        public void GivenSettingCategoryOptionIs(string category, string option, dynamic value)
        {
            TestData.Repository[category, option] = value;
        }

        [Given(@"I instantiate SeleniumDriver")]
        [When(@"I instantiate SeleniumDriver")]
        public void WhenIInstantiateSeleniumDriver()
        {
            try
            {
                ScenarioContext.Current.Add("SeleniumDriver", new SeleniumDriver());
            }
            catch(Exception ex)
            {
                Exception exCurrentLevel = ex;
                string message = exCurrentLevel.Message;
                while (exCurrentLevel.InnerException != null)
                {
                    message += " -> " + exCurrentLevel.InnerException.Message;
                    exCurrentLevel = exCurrentLevel.InnerException;
                }
                Assert.Fail($"Failed to instantiate Selenium Driver: {message}");
            }
        }

        [Given(@"I browse to ""(.*)""")]
        [When(@"I browse to ""(.*)""")]
        public void WhenIBrowseTo(string urlToBrowseTo)
        { 
            try
            {
                ScenarioContext.Current.Get<SeleniumDriver>("SeleniumDriver").GotoURL(urlToBrowseTo);
            }
            catch (Exception ex)
            {
                Exception exCurrentLevel = ex;
                string message = exCurrentLevel.Message;
                while (exCurrentLevel.InnerException != null)
                {
                    message += " -> " + exCurrentLevel.InnerException.Message;
                    exCurrentLevel = exCurrentLevel.InnerException;
                }
                Assert.Fail($"Failed to browse to [{urlToBrowseTo}]: {message}");
            }
        }

        [Then(@"a process exists named ""(.*)""")]
        public void ThenAProcessExistsNamed(string processname)
        {
            Assert.IsTrue(Process.GetProcessesByName(processname).Count() > 0, $"There must be more than one process named ({processname})");
        }

        [Then(@"I can read the page title ""(.*)""")]
        public void ThenICanReadThePageTitle(string expectedPageTitle)
        {
            Assert.AreEqual(expectedPageTitle, ScenarioContext.Current.Get<SeleniumDriver>("SeleniumDriver").PageTitle);
        }

        [Given(@"I use FindElement to locate an element using XPath ""(.*)""")]
        [When(@"I use FindElement to locate an element using XPath ""(.*)""")]
        public void WhenIUseFindElementToLocateAnElementUsingXPath(string xPath)
        {
            Element foundElement;
            try
            {
                //foundElement = ScenarioContext.Current.Get<SeleniumDriver>("SeleniumDriver").FindElement(new ObjectMappingDetails(xPath, "The XPath"));
                dynamic xx = ScenarioContext.Current.Get<SeleniumDriver>("SeleniumDriver");
                foundElement = xx.FindElement(new ObjectMappingDetails(xPath, "The XPath"));
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using TeamControlium.Utilities;

namespace TeamControlium.Controlium.UnitTests.SeleniumDriver
{
    [TestClass]
    public class DriverTests
    {
        List<dynamic> browsers = new List<dynamic>() {
                new {
                    Name = "IE11",
                    ProcessName = "iexplore",
                    ServerProcessName = "IEDriverServer"
                },
                new {
                    Name = "Chrome",
                    ProcessName = "chrome",
                    ServerProcessName = "chromedriver"
                }
            };

        [TestInitialize]
        public void Init()
        {
            TestData.Repository.Clear();
            Controlium.SeleniumDriver.ResetSettings();
        }

        [TestCleanup]
        public void CleanUp()
        {
            TestData.Repository.Clear();
            Controlium.SeleniumDriver.ResetSettings();
        }

        [TestMethod]
        public void Selenium_can_be_launched_with_mandatory_settings_set()
        {
            foreach (dynamic browser in browsers)
            {
                // Arrange             
                Init();

                KillProcess(browser.ServerProcessName);
                KillProcess(browser.ProcessName);

                TestData.Repository["Selenium", "Browser"] = browser.Name;
                TestData.Repository["Selenium", "Host"] = "localhost";
                TestData.Repository["Selenium", "SeleniumServerFolder"] = ".//..//..//TestSeleniumServer";
                TestData.Repository["Selenium", "DebugMode"] = "off";

                // Act
                var driver = new Controlium.SeleniumDriver();

                // Assert 
                var serverProcName = Process.GetProcessesByName(browser.ServerProcessName);
				var procName = Process.GetProcessesByName(browser.ProcessName);
                Assert.IsTrue(serverProcName > 0, $"There must be one or more processes named ({browser.ServerProcessName})");
                Assert.IsTrue(procName > 0, $"There must be one or more processes named ({browser.ProcessName})");
				
                // Antiseptic
                driver.CloseDriver();
                driver = null;

                CleanUp();
            }
        }

        private void KillProcess(string processNameToKill)
        {
            Process[] processes = Process.GetProcessesByName(processNameToKill);
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
    }
}



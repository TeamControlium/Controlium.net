﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace TeamControlium.TestFramework
{
    [Binding]
    public sealed class SpecflowHooks
    {
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks

        [BeforeScenario]
        public void BeforeScenario()
        {
            Utilities.TestData.Clear();
            SeleniumDriver.ResetSettings();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            ((SeleniumDriver)ScenarioContext.Current["SeleniumDriver"]).CloseDriver();

        }
    }
}

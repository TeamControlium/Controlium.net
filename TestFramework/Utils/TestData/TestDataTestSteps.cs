﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace TeamControlium.TestFramework
{
    [Binding]
    public sealed class TestDataTestSteps
    {
        [BeforeFeature("Utilities_TestData")]
        public static void BeforeAppFeature()
        {
            Logger.LoggingLevel = Logger.LogLevels.FrameworkDebug;
            Logger.TestToolLog = (s) => { Debug.WriteLine(s); };
            Logger.WriteToConsole = false;
        }


        [Given(@"I have saved Test Data item (\d) ""(.*)"" in item named ""(.*)"" in category ""(.*)""")]
        public void GivenIHaveSavedTestDataInItemNamedInCategory(int itemIndex, string item, string name, string category)
        {
            ScenarioContext.Current[$"Saved-{itemIndex}"] = item;
            Utilities.TestData[category, name] = item;
        }

        [Given(@"I have saved Test Data item (\d) (\d*) in item named ""(.*)"" in category ""(.*)""")]
        public void GivenIHaveSavedTestDataInItemNamedInCategory(int itemIndex, int item, string name, string category)
        {
            ScenarioContext.Current[$"Saved-{itemIndex}"] = item;
            Utilities.TestData[category, name] = item;
        }

        [Given(@"I have a Test data item ""(.*)"" in item named ""(.*)""")]
        public void GivenIHaveATestDataItemInItemNamed(string item, string name)
        {
            try
            {
                var dummy = ScenarioContext.Current[$"Dictionary"];
            }
            catch
            {
                ScenarioContext.Current[$"Dictionary"] = new Dictionary<string, dynamic>();
            }
            ((Dictionary<string, dynamic>)ScenarioContext.Current[$"Dictionary"]).Add(name,item);
        }

        [Given(@"I have saved items in category ""(.*)""")]
        public void GivenIHaveSavedItemsInCategory(string category)
        {
            int index = 1;
            foreach (KeyValuePair<string, dynamic> keyvalue in ((Dictionary<string, dynamic>)ScenarioContext.Current[$"Dictionary"]))
            {
                ScenarioContext.Current[$"Saved-{index++}"] = keyvalue.Value;
            }
            Utilities.TestData[category] = ((Dictionary<string, dynamic>)ScenarioContext.Current[$"Dictionary"]);
        }


        [When(@"I recall the Test Data item (\d) named ""(.*)"" in category ""(.*)""")]
        public void WhenIRecallTheTestDataNamedInCategory(string itemIndex,string name, string category)
        {
            dynamic value;
            if (Utilities.TestData.TryGetItem<dynamic>(category, name, out value))
                ScenarioContext.Current[$"Recalled-{itemIndex}"] = value;
            else
                ScenarioContext.Current[$"Recalled-{itemIndex}"] = Utilities.TestData.TryException;
        }

        [When(@"I change Test Data item (\d) named ""(.*)"" in category ""(.*)"" to ""(.*)""")]
        public void WhenIChangeTestDataItemNamedInCategoryTo(int itemIndex, string name, string category, string newValue)
        {
            ScenarioContext.Current[$"Saved-{itemIndex}"] = newValue;
            Utilities.TestData[category, name] = newValue;
        }

        [When(@"I recall the Test Data item (\d) named ""(.*)"" in category ""(.*)"" as an integer")]
        public void WhenIRecallTheTestDataNamedInCategoryAsAnInteger(int itemIndex, string name, string category)
        {
            int value;
            if (Utilities.TestData.TryGetItem<int>(category, name, out value))
                ScenarioContext.Current[$"Recalled-{itemIndex}"] = value;
            else
                ScenarioContext.Current[$"Recalled-{itemIndex}"] = Utilities.TestData.TryException;
        }

        [When(@"I clear the test data")]
        public void WhenIClearTheTestData()
        {
            Utilities.TestData.Clear();
        }


        [Then(@"the recalled (.*) value matches the saved (.*) value")]
        public void ThenTheRecalledValueMatchesTheSavedValue(int savedIndex,int recalledIndex)
        {
            if (!ScenarioContext.Current.ContainsKey($"Saved-{savedIndex}"))
                Assert.Inconclusive($"No [Saved-{savedIndex}] scenario context key");
            if (!ScenarioContext.Current.ContainsKey($"Recalled-{recalledIndex}"))
                Assert.Inconclusive($"No [Recalled-{recalledIndex}] scenario context key");

            Assert.AreEqual(ScenarioContext.Current[$"Saved-{savedIndex}"], ScenarioContext.Current[$"Recalled-{recalledIndex}"]);
        }

        [Then(@"the recalled (.*) value is an exception with innermost exception message ""(.*)""")]
        public void ThenTheRecalledValueIsError(int recalledIndex, string errorText)
        {
            object dynError = ScenarioContext.Current[$"Recalled-{recalledIndex}"];

            Assert.IsInstanceOfType(dynError, typeof(Exception));

            Exception exception = (Exception)ScenarioContext.Current[$"Recalled-{recalledIndex}"];
            while (exception.InnerException != null) exception = exception.InnerException;

            Assert.AreEqual((new string(errorText.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray())).Replace(" ","").Trim(), (new string(exception.Message.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray())).Replace(" ", "").Trim());

        }


    }
}

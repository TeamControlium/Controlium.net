using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using TeamControlium.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TeamControlium.Framework
{
    [Binding]
    public sealed class LoggerTestSteps
    {
        [Given(@"I have configured Logger to write to a string")]
        public void GivenIHaveConfiguredLoggerToWriteToAString()
        {
            Logger.TestToolLog = (s) => { ScenarioContext.Current.Add("LoggerOutputString",s); };
            Logger.WriteToConsole = false;
        }

        [Given(@"I set Logger to level ""(.*)""")]
        public void WhenISetLoggerToLevel(string logLevel)
        {
            Logger.SetLoggingLevel(logLevel);
        }

        [When(@"I call Logger with level ""(.*)"" and string ""(.*)""")]
        public void WhenICallLoggerWithLevelAndString(string logLevel, string stringToWrite)
        {
            Logger.WriteLine(Logger.LogLevels.FrameworkDebug, stringToWrite);
        }

        [Then(@"the string written to by Logger should end with ""(.*)""")]
        public void ThenTheStringWrittenToByLoggerShouldEndWith(string expectedToEndWith)
        {
            string outputString = (string)ScenarioContext.Current["LoggerOutputString"];
            Assert.IsTrue(outputString.EndsWith(expectedToEndWith));
        }


    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace TeamControlium.Framework
{
    [Binding]
    public sealed class DetokenizerTestSteps
    {
        [Given(@"I have a string ""(.*)""")]
        public void GivenIHaveAStringWithToken(string stringToBeProcessed)
        {
            ScenarioContext.Current["StringToBeProcessed"] = stringToBeProcessed;
        }

        [When(@"I process the token")]
        public void WhenIProcessTheToken()
        {
            string toBeProcessed = (string)ScenarioContext.Current["StringToBeProcessed"];
            string processed = Detokenizer.ProcessTokensInString(toBeProcessed);
            ScenarioContext.Current["ProcessedString"] = processed;
        }

        [Then(@"the string is today's date in the format ""(.*)""")]
        public void ThenTheStringIsTodaySDateInTheFormat(string requiredFormatOfDate)
        {
            string requiredDate = DateTime.Now.ToString(requiredFormatOfDate);
            string actualDate = (string)ScenarioContext.Current["ProcessedString"];
            Assert.AreEqual(requiredDate, actualDate, "Dates and formats match");
        }

        [Then(@"the string is yesterday's date in the format ""(.*)""")]
        public void ThenTheStringIsYesterdaySDateInTheFormat(string requiredFormatOfDate)
        {
            string requiredDate = DateTime.Now.AddDays(-1).ToString(requiredFormatOfDate);
            string actualDate = (string)ScenarioContext.Current["ProcessedString"];
            Assert.AreEqual(requiredDate, actualDate, "Dates and formats match");
        }

        [Then(@"the string is tomorrows's date in the format ""(.*)""")]
        public void ThenTheStringIsTomorrowsDateInTheFormat(string requiredFormatOfDate)
        {
            string requiredDate = DateTime.Now.AddDays(1).ToString(requiredFormatOfDate);
            string actualDate = (string)ScenarioContext.Current["ProcessedString"];
            Assert.AreEqual(requiredDate, actualDate, "Dates and formats match");
        }


        [Then(@"the string is the date (.*) ""(.*)"" in the format ""(.*)""")]
        public void ThenTheStringIsTheDateInTheFormat(int offset, string offsetType, string requiredFormatOfDate)
        {
            DateTime requiredDate;
            switch (offsetType.ToLower().Trim())
            {
                case "days":
                    requiredDate = DateTime.Now.AddDays(offset); break;
                case "months":
                    requiredDate = DateTime.Now.AddMonths(offset); break;
                case "years":
                    requiredDate = DateTime.Now.AddYears(offset); break;
                default:
                    throw new ArgumentException("Unknown Offset Type. Expect days, months or years.", "offsetType");
            }
            string actualDate = (string)ScenarioContext.Current["ProcessedString"];
            Assert.AreEqual(requiredDate.ToString(requiredFormatOfDate), actualDate, "Dates and formats match");

        }

    }
}

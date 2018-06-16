using TeamControlium.Utilities;
using TechTalk.SpecFlow;

namespace TeamControlium.Controlium
{
    [Binding]
    public sealed class SpecflowHooks
    {
        [BeforeScenario]
        public void BeforeScenario()
        {
            TestData.Repository.Clear();
            SeleniumDriver.ResetSettings();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            try
            {
                ScenarioContext.Current.Get<SeleniumDriver>("SeleniumDriver").CloseDriver();
            }
            catch { }
        }
    }
}
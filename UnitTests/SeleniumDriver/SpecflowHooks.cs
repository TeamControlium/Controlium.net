using TeamControlium.Utilities;
using TechTalk.SpecFlow;

namespace TeamControlium.Controlium
{
    [Binding]
    public sealed class SpecflowHooks
    {
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks

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
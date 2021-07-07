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
            Repository.ClearRepositoryAll();
        }

        [AfterScenario]
        public void AfterScenario(ScenarioContext context)
        {
            try
            {
                context.Get<SeleniumDriver>("SeleniumDriver").CloseDriver();
            }
            catch { }
        }
    }
}
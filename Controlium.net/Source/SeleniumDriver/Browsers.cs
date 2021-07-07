using System;
using System.Linq;
using System.Text.RegularExpressions;
using TeamControlium.Utilities;

namespace TeamControlium.Controlium
{
    public partial class SeleniumDriver
    {
        /// <summary>
        /// Browser Selenium script executing against
        /// </summary>
        /// <seealso cref="Browsers">Lists all possible Browser &amp; versions than can be returned.</seealso>
        public static string TestBrowser { get; private set; } = String.Empty;

        /// <summary>
        /// Returns true if target browser is any version of Chrome
        /// </summary>
        public static bool IsChrome { get { return TestBrowser.StartsWith("chrome"); } }

        /// <summary>
        /// Returns true if target browser is any version of Internet Explorer
        /// </summary>
        public static bool IsInternetExplorer { get { return TestBrowser.StartsWith("ie"); } }

        /// <summary>
        /// Returns true if target browser is any version of Edge
        /// </summary>
        public static bool IsEdge { get { return TestBrowser.StartsWith("edge"); } }

        /// <summary>
        /// Returns true if target browser is any version of Safari browser
        /// </summary>
        public static bool IsSafari { get { return TestBrowser.StartsWith("safari"); } }

        /// <summary>
        /// Returns true if target browser is any version of Safari browser
        /// </summary>
        public static bool IsFirefox { get { return TestBrowser.StartsWith("firefox"); } }

        private static bool TestBrowserHasBeenSet { get { return TestBrowser != String.Empty; } }

        /// <summary>
        /// Sets the Browser being used for testing.  Expects RunSetting "Selenium", "Browser" to be set
        /// </summary>
        public static void SetTestBrowser()
        {
            string browser;
            if (!Repository.TryGetItemGlobal<string>(ConfigBrowser[0], ConfigBrowser[1], out browser))
            {
                throw new InvalidSettings(string.Format("Error getting setting [{0}],[{1}].", ConfigBrowser[0], ConfigBrowser[1]), Repository.RepositoryLastTryException());
            }

            if (string.IsNullOrEmpty(browser)) throw new RunOptionOrCategoryDoesNotExist(ConfigBrowser[0], ConfigBrowser[1], "Unable to set Selenium host Browser.");

            browser = browser.ToLower().Replace(" ", "");

            if (Regex.IsMatch(browser, "^chrome\\d*$") ||
                Regex.IsMatch(browser, "^edge\\d*$") ||
                Regex.IsMatch(browser, "^safari\\d*$") ||
                Regex.IsMatch(browser, "^firefox\\d*$")
                )
            {
                SeleniumDriver.TestBrowser = browser;
            } else if (Regex.IsMatch(browser, "^ie\\d*$") || Regex.IsMatch(browser, "^internetexplorer\\d*$"))
            {
                SeleniumDriver.TestBrowser = "ie" + Regex.Match(browser, "\\d+$");
            }
        }
    }
}
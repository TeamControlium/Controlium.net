using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.Remoting;

namespace TeamControlium.Framework
{
    public partial class SeleniumDriver
    {
        /// <summary>
        /// Supported Browsers.  Browser is set when SeleniumDriver is instantiated.  SeleniumDriver gets the Browser to be used from the run configuration
        /// option "Browser" in category "Selenium".
        /// <para/><para/>
        /// When running remote Selenium the browser/version is used in the Desired Capabilities to request
        /// that browser and version.
        /// <para/><para/>
        /// When running locally only Internet Explorer and Chrome are supported.  Any version can be stipulated but the version will actually be dependant on the installed version on the machine and no check is made.
        /// </summary>
        public enum Browsers
        {
            /// <summary>
            /// No Browser has been selected
            /// </summary>
            NoneSelected,
            /// <summary>
            /// Chrome browser version 26.
            /// <para/>
            /// Valid Config options; 'Chrome 26', '537.31' and 'WebKit537.31'
            /// </summary>
            Chrome26,
            /// <summary>
            /// Chrome browser version 27.
            /// <para/>
            /// Valid Config options; 'Chrome 27', '537.36' and 'WebKit537.36'
            /// </summary>
            Chrome27,
            /// <summary>
            /// Chrome browser version 28.
            /// <para/>
            /// Valid Config options; 'Chrome 28'
            /// </summary>
            Chrome28,
            /// <summary>
            /// Chrome browser version 29.
            /// <para/>
            /// Valid Config options; 'Chrome 29'
            /// </summary>
            Chrome29,
            /// <summary>
            /// Chrome browser version 30.
            /// <para/>
            /// Valid Config options; 'Chrome 30'
            /// </summary>
            Chrome30,
            /// <summary>
            /// Chrome browser version 31.
            /// <para/>
            /// Valid Config options; 'Chrome 31'
            /// </summary>
            Chrome31,
            /// <summary>
            /// Chrome browser version 32.
            /// <para/>
            /// Valid Config options; 'Chrome 32'
            /// </summary>
            Chrome32,
            /// <summary>
            /// Chrome browser version 33.
            /// <para/>
            /// Valid Config options; 'Chrome 33'
            /// </summary>
            Chrome33,
            /// <summary>
            /// Chrome browser version 34.
            /// <para/>
            /// Valid Config options; 'Chrome 34'
            /// </summary>
            Chrome34,
            /// <summary>
            /// Chrome browser version 35.
            /// <para/>
            /// Valid Config options; 'Chrome 35'
            /// </summary>
            Chrome35,
            /// <summary>
            /// Chrome browser version 36.
            /// <para/>
            /// Valid Config options; 'Chrome 36'
            /// </summary>
            Chrome36,
            /// <summary>
            /// Chrome browser version 37.
            /// <para/>
            /// Valid Config options; 'Chrome 37'
            /// </summary>
            Chrome37,
            /// <summary>
            /// Chrome browser version 38.
            /// <para/>
            /// Valid Config options; 'Chrome 38'
            /// </summary>
            Chrome38,
            /// <summary>
            /// Chrome browser version 39.
            /// <para/>
            /// Valid Config options; 'Chrome 39'
            /// </summary>
            Chrome39,
            /// <summary>
            /// Chrome browser version 40.
            /// <para/>
            /// Valid Config options; 'Chrome 40'
            /// </summary>
            Chrome40,
            /// <summary>
            /// Chrome browser version 41 (Default Chrome version if no version defined).
            /// <para/>
            /// Valid Config options; 'Chrome', 'Chrome 28'
            /// </summary>
            Chrome41,
            /// <summary>
            /// Internet Explorer 8
            /// <para/>
            /// Valid Config options; 'IE8', 'Internet Explorer 8'
            /// </summary>
            IE8,
            /// <summary>
            /// Internet Explorer 9
            /// <para/>
            /// Valid Config options; 'IE9', 'Internet Explorer 9'
            /// </summary>
            IE9,
            /// <summary>
            /// Internet Explorer 10
            /// <para/>
            /// Valid Config options; 'IE10', 'Internet Explorer 10'
            /// </summary>
            IE10,
            /// <summary>
            /// Internet Explorer 11 (Default Internet Explorer if no version defined).
            /// <para/>
            /// Valid Config options; 'IE11', 'Internet Explorer 11', 'IE', 'Internet Explorer'
            /// </summary>
            IE11,
            /// <summary>
            /// Edge.
            /// <para/>
            /// Valid Config options; 'Edge'
            /// </summary>
            Edge,
            /// <summary>
            /// Safari 1
            /// <para/>
            /// Valid Config options; 'Safari 1', 'Safari'
            /// <para/>
            /// Note. This version is only available on MAC's running Mac OS X v10.2 (Jaguar)
            /// </summary>
            Safari1,
            /// <summary>
            /// Safari 2
            /// <para/>
            /// Valid Config options; 'Safari 2',
            /// <para/>
            /// Note. This version is only available on MAC's running Mac OS X v10.3 (Panther)
            /// </summary>
            Safari2,
            /// <summary>
            /// Safari 3
            /// <para/>
            /// Valid Config options; 'Safari 3',
            /// <para/>
            /// Note. 
            /// </summary>
            Safari3,
            /// <summary>
            /// Safari 4
            /// <para/>
            /// Valid Config options; 'Safari 4',
            /// <para/>
            /// Note. Used on a number of iPhone and iPad versions
            /// </summary>
            Safari4,
            /// <summary>
            /// Safari 5
            /// <para/>
            /// Valid Config options; 'Safari 5',
            /// <para/>
            /// Note. Used on a number of iPhone and iPad versions
            /// </summary>
            Safari5,
            /// <summary>
            /// Safari 6
            /// <para/>
            /// Valid Config options; 'Safari 6',
            /// <para/>
            /// Note. Used on a number of iPhone and iPad versions
            /// </summary>
            Safari6,
            /// <summary>
            /// Safari 7 (Default Safari if no version defined).
            /// <para/>
            /// Valid Config options; 'Safari 7',
            /// <para/>
            /// Note. Used on a number of iPhone and iPad versions
            /// </summary>
            Safari7
        }

        /// <summary>
        /// Browser Selenium script executing against
        /// </summary>
        /// <seealso cref="Browsers">Lists all possible Browser &amp; versions than can be returned.</seealso>
        public static Browsers TestBrowser { get; private set; }

        /// <summary>
        /// Returns true if target browser is any version of Chrome
        /// </summary>
        public static bool IsChrome { get; private set; }

        /// <summary>
        /// Returns true if target browser is any version of Internet Explorer
        /// </summary>
        public static bool IsInternetExplorer { get; private set; }

        /// <summary>
        /// Returns true if target browser is any version of Edge
        /// </summary>
        public static bool IsEdge { get; private set; }

        /// <summary>
        /// Returns true if target browser is any version of Safari browser
        /// </summary>
        public static bool IsSafari { get; private set; }

        private static bool TestBrowserHasBeenSet = false;
        /// <summary>
        /// Sets the Browser being used for testing.  Expects RunSetting "Selenium", "Browser" to be set
        /// </summary>
        public static void SetTestBrowser()
        {
            IsChrome = false;
            IsInternetExplorer = false;
            IsSafari = false;
            IsEdge = false;

            string browser = Utilities.TestData[ConfigBrowser[0], ConfigBrowser[1]];
            if (string.IsNullOrEmpty(browser)) throw new SeleniumExceptions.RunOptionOrCategoryDoesNotExist(ConfigBrowser[0], ConfigBrowser[1], "Unable to set Selenium host Browser.");

            switch (browser.ToLower().Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty))
            {
                case "chrome26":
                case "537.31":
                case "webkit537.31":
                    TestBrowser = Browsers.Chrome26; IsChrome = true; break;
                case "chrome27":
                case "537.36":
                case "webkit537.36":
                    TestBrowser = Browsers.Chrome27; IsChrome = true; break;
                case "chrome28":
                    TestBrowser = Browsers.Chrome28; IsChrome = true; break;
                case "chrome29":
                    TestBrowser = Browsers.Chrome29; IsChrome = true; break;
                case "chrome30":
                    TestBrowser = Browsers.Chrome30; IsChrome = true; break;
                case "chrome31":
                    TestBrowser = Browsers.Chrome31; IsChrome = true; break;
                case "chrome32":
                    TestBrowser = Browsers.Chrome32; IsChrome = true; break;
                case "chrome33":
                    TestBrowser = Browsers.Chrome33; IsChrome = true; break;
                case "chrome34":
                    TestBrowser = Browsers.Chrome34; IsChrome = true; break;
                case "chrome35":
                    TestBrowser = Browsers.Chrome35; IsChrome = true; break;
                case "chrome36":
                    TestBrowser = Browsers.Chrome36; IsChrome = true; break;
                case "chrome37":
                    TestBrowser = Browsers.Chrome37; IsChrome = true; break;
                case "chrome38":
                    TestBrowser = Browsers.Chrome38; IsChrome = true; break;
                case "chrome39":
                    TestBrowser = Browsers.Chrome39; IsChrome = true; break;
                case "chrome40":
                    TestBrowser = Browsers.Chrome40; IsChrome = true; break;
                case "chrome":                                   // DEFAULT, IF NO CHROME VERSION GIVEN!
                case "chrome41":
                    TestBrowser = Browsers.Chrome41; IsChrome = true; break;
                case "ie8":
                case "internetexplorer8":
                    TestBrowser = Browsers.IE8; IsInternetExplorer = true; break;
                case "ie9":
                case "internetexplorer9":
                    TestBrowser = Browsers.IE9; IsInternetExplorer = true; break;
                case "ie10":
                case "internetexplorer10":
                    TestBrowser = Browsers.IE10; IsInternetExplorer = true; break;
                case "ie11":
                case "internetexplorer11":
                case "ie":                                     // Default if no IE version given.
                case "internetexplorer":                       //
                    TestBrowser = Browsers.IE11; IsInternetExplorer = true; break;
                case "edge":
                    TestBrowser = Browsers.Edge; IsEdge = true; break;
                case "safari1":
                    TestBrowser = Browsers.Safari1; IsSafari = true; break;
                case "safari2":
                    TestBrowser = Browsers.Safari2; IsSafari = true; break;
                case "safari3":
                    TestBrowser = Browsers.Safari3; IsSafari = true; break;
                case "safari4":
                    TestBrowser = Browsers.Safari4; IsSafari = true; break;
                case "safari5":
                    TestBrowser = Browsers.Safari5; IsSafari = true; break;
                case "safari6":
                    TestBrowser = Browsers.Safari6; IsSafari = true; break;
                case "safari7":
                case "safari":                              // Default if no Safari version given
                    TestBrowser = Browsers.Safari7; IsSafari = true; break;
                default:
                    throw new SeleniumExceptions.UnsupportedBrowser(browser);
            }
            TestBrowserHasBeenSet = true;
        }
    }
}
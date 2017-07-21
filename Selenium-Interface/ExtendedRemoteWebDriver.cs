using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamControlium.Framework
{
    /// <summary>Extends the standard Selenium RemoteWebDriver iomplementing ITakesScreenshot.  This enables tests to take remote and local browser screenshots if required.</summary>
    /// <remarks>Some tools automatically take screenshots on a test failure or abort but these usually only work locally.  This enables allows tests to take screenshots at any point in test execution.</remarks>
    public class ExtendedRemoteWebDriver : RemoteWebDriver, ITakesScreenshot
    {
        /// <summary>
        /// Instantiates T1RemoteWebDriver
        /// </summary>
        /// <param name="RemoteAdress">URI of remote Selenium</param>
        /// <param name="capabilities">Desired capabilities (and authentication information if required)</param>
        /// <param name="Timeout">Selenium command timeout (in Seconds, expessed as a string). Default - if null - 60 Seconds.</param>
        public ExtendedRemoteWebDriver(Uri RemoteAdress, ICapabilities capabilities, string Timeout)
            : base(RemoteAdress, capabilities, TimeSpan.FromSeconds((string.IsNullOrEmpty(Timeout)) ? 60 : int.Parse(Timeout)))
        {
        }

        /// <summary>
        /// Instantiates T1RemoteWebDriver
        /// </summary>
        /// <param name="RemoteAdress">URI of remote Selenium</param>
        /// <param name="capabilities">Desired capabilities (and authentication information if required)</param>
        /// <param name="Timeout">Selenium command timeout (in Seconds). Default - if null - 60 Seconds.</param>
        public ExtendedRemoteWebDriver(Uri RemoteAdress, ICapabilities capabilities, int Timeout)
            : base(RemoteAdress, capabilities, TimeSpan.FromSeconds(Timeout))
        {
        }

        /// <summary>
        /// Gets a Screenshot object representing the image of the page on the remote Selenium Server Screen.
        /// </summary>
        /// <returns>A <see cref="Screenshot"/> object containing the image in Base64 encoding</returns>
        public new Screenshot GetScreenshot()
        {
            // Get the screenshot as base64.  The 'take screenshot' command is sent to the Remote Selenium server (that is driving the browser)
            // which responds with a Base64 encoding of the screenshot.  We take the encoding and pass it back to the calling method for use.
            Response screenshotResponse = this.Execute(DriverCommand.Screenshot, null);
            string base64 = screenshotResponse.Value.ToString();

            // ... and convert it.
            return new Screenshot(base64);
        }
    }
}

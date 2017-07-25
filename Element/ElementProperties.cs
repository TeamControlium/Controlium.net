using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TeamControlium.Framework
{
    public partial class Element
    {
        /// <summary>
        /// Indicates if Element if visible and is in viewport
        /// </summary>
        /// <seealso cref="seleniumDriver.IsElementVisible()"/>
        public bool IsDisplayed
        {
            get
            {
                return Visible(true);
            }
        }

        public bool IsHeightStable { get { return !AttributeChanging("offsetHeight", TimeSpan.FromMilliseconds(200)); } }
        public bool IsWidthStable { get { return !AttributeChanging("offsetWidth", TimeSpan.FromMilliseconds(200)); } }

        public bool IsSizeStable { get { return !AttributeChanging(new string[] { "offsetWidth", "offsetHeight" }, TimeSpan.FromMilliseconds(200)); } }

        public bool IsPositionStable
        {
            get
            {
                string first = seleniumDriver.ExecuteJavaScriptReturningString("var rect = arguments[0].getBoundingClientRect(); return '' + rect.left + ',' + rect.top + ',' + rect.right + ',' + rect.bottom", WebElement);
                Thread.Sleep(200);
                string second = seleniumDriver.ExecuteJavaScriptReturningString("var rect = arguments[0].getBoundingClientRect(); return '' + rect.left + ',' + rect.top + ',' + rect.right + ',' + rect.bottom", WebElement);
                return (first == second);
            }
        }

        /// <summary>Tests is element is enabled (and so will accept text and/or click events)
        /// </summary>
        /// <param name="Element">Element to test</param>
        /// <returns>True if element is enabled, false if not</returns>
        public bool IsEnabled
        {
            get
            {
                if (WebElement != null)
                    return seleniumDriver.Enabled(WebElement);
                else
                {
                    Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element enabled = [Web element is NULL]");
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the element size using the java clientHeight and clientWidth attributes
        /// </summary>
        public Size ElementSize
        {
            get
            {
                string Height = seleniumDriver.ExecuteJavaScriptReturningString("return arguments[0].clientHeight", WebElement);
                string Width = seleniumDriver.ExecuteJavaScriptReturningString("return arguments[0].clientWidth", WebElement);
                return new Size(int.Parse(Width), int.Parse(Height));
            }


        }


        /// <summary>Tests if element is currently visible to the user.</summary>
        /// <param name="Element">Element to test</param>
        /// <returns>True if visible, false if not (or if element is null)</returns>
        /// <remarks>
        /// This uses the Selenium Displayed boolean property.
        /// </remarks>
        public static bool Visible(Element element, bool CheckIfElementIsInViewport = false)
        {
            return element.Visible(CheckIfElementIsInViewport);
        }

        /// <summary>Tests if element is currently visible to the user.</summary>
        /// <param name="Element">Element to test</param>
        /// <param name="CheckIfElementIsInViewport">If true, checks to see if element is in Viewport.  If false uses builtin Selenium element property</param>
        /// <returns>True if visible, false if not (or if element is null)</returns>
        /// <remarks>
        /// This uses the Selenium Displayed boolean property but ANDs it with a Javascript result that discovers if the element is in the viewport.  This
        /// fixes the Selenium issue whereby it returns a false TRUE (IE. Element is not actually in the Displayport....)
        /// </remarks>
        public bool Visible(bool CheckIfElementIsInViewport = false)
        {
            if (this.WebElement != null)
            {
                if (CheckIfElementIsInViewport)
                {
                    bool SeleniumDisplayed = false;
                    bool MyDisplayed = false;
                    string sResult = null;
                    if (!seleniumDriver.TryExecuteJavaScript("var rect = arguments[0].getBoundingClientRect(); return ( rect.top >= 0 && rect.left >= 0 && rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) && rect.right <= (window.innerWidth || document.documentElement.clientWidth));", out sResult, this.WebElement))
                    {
                        throw new Exception(string.Format("LGIASuper Selenium IsElementVisible javascript error: {0}", sResult));
                    }
                    SeleniumDisplayed = this.WebElement.Displayed;
                    MyDisplayed = (sResult.ToLower() == "true");
                    Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element in Viewport = {0} ({1})", sResult, MyDisplayed ? "Yes" : "No");
                    return SeleniumDisplayed && MyDisplayed;
                }
                else
                    return this.WebElement.Displayed;
            }
            else
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element displayed = [Web or LGIASuper element is NULL]");
                return false;
            }
        }

        public bool Displayed(IWebElement webElement)
        {
            bool result = webElement.Displayed;
            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element displayed = [{0}]", result ? "TRUE" : "FALSE");
            return result;
        }

        private bool AttributeChanging(string AttribName, TimeSpan timeDelta)
        {
            string First = seleniumDriver.ExecuteJavaScriptReturningString(string.Format("return arguments[0].{0};", AttribName), WebElement);
            Thread.Sleep(timeDelta);
            string Second = seleniumDriver.ExecuteJavaScriptReturningString(string.Format("return arguments[0].{0};", AttribName), WebElement);
            return (!First.Equals(Second));
        }

        private bool AttributeChanging(string[] AttribNames, TimeSpan timeDelta)
        {
            List<string[]> Measures = new List<string[]>();
            try
            {
                foreach (string attrib in AttribNames)
                {
                    string[] measure = new string[2];
                    measure[0] = attrib;
                    measure[1] = seleniumDriver.ExecuteJavaScriptReturningString(string.Format("return arguments[0].{0};", measure[0]), WebElement);
                    Measures.Add(measure);
                }
                Thread.Sleep(timeDelta);

                foreach (string[] measure in Measures)
                {
                    string newMeasure = seleniumDriver.ExecuteJavaScriptReturningString(string.Format("return arguments[0].{0};", measure[0]), WebElement);
                    if (measure[1] != newMeasure)
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining and comparing named attributes", ex);
            }
        }

    }
}
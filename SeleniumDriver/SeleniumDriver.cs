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
using HtmlAgilityPack;
using System.Diagnostics;

namespace TeamControlium.Framework
{
    public partial class SeleniumDriver
    {
        // ENUMS
        /// <summary>Defines possible types of visibility for an element.</summary>
        public enum Visibility
        {
            /// <summary>
            /// Element is Visible - would be able to be seen by a user.
            /// <para/><para/>
            /// Does not take into account text/background colours etc..
            /// </summary>
            Visible,
            /// <summary>
            /// Element is Hidden - would not be able to be seen by a user
            /// </summary>
            Hidden,
            /// <summary>
            /// Elements visibility is unknown.
            /// </summary>
            Unknown
        };

        // PROPERTIES
        /// <summary>Reference to last Try method's exception, if thrown.</summary>
        /// <remarks>When a call to a TryMethod is performed this is set to null.  If the TryMethod throws an exception a boolean false is returned and this property set
        /// to reference it.</remarks>
        public Exception TryException { get; private set; }  // Try methods set any exception they get so that non-try's can abort test if needed.

        /// <summary>
        /// Sets or Gets the current Selenium timeout used, if not overriden, when locating elements as part of a Find or SetControl.
        /// </summary>
        public TimeSpan ElementFindTimeout
        {
            get
            {
                return elementFindTimings.Timeout;
            }
            set
            {
                elementFindTimings.Timeout = value;
            }
        }

        /// <summary>
        /// Sets or Gets the current Selenium polling interval used, if not overriden, when locating elements as part of a Find or SetControl.
        /// </summary>
        public TimeSpan ElementFindPollingInterval
        {
            get
            {
                return elementFindTimings.PollingInterval;
            }
            set
            {
                elementFindTimings.PollingInterval = value;
            }
        }

        /// <summary>
        /// Key codes understood by Selenium
        /// </summary>
        public static class Keys
        {
            /// <summary>
            /// Represents the NUL keystroke.
            /// </summary>
            public static readonly string Null = OpenQA.Selenium.Keys.Null;

            /// <summary>
            /// Represents the Cancel keystroke.
            /// </summary>
            public static readonly string Cancel = OpenQA.Selenium.Keys.Cancel;

            /// <summary>
            /// Represents the Help keystroke.
            /// </summary>
            public static readonly string Help = OpenQA.Selenium.Keys.Help;

            /// <summary>
            /// Represents the Backspace key.
            /// </summary>
            public static readonly string Backspace = OpenQA.Selenium.Keys.Backspace;

            /// <summary>
            /// Represents the Tab key.
            /// </summary>
            public static readonly string Tab = OpenQA.Selenium.Keys.Tab;

            /// <summary>
            /// Represents the Clear keystroke.
            /// </summary>
            public static readonly string Clear = OpenQA.Selenium.Keys.Clear;

            /// <summary>
            /// Represents the Return key.
            /// </summary>
            public static readonly string Return = OpenQA.Selenium.Keys.Return;

            /// <summary>
            /// Represents the Enter key.
            /// </summary>
            public static readonly string Enter = OpenQA.Selenium.Keys.Enter;

            /// <summary>
            /// Represents the Shift key.
            /// </summary>
            public static readonly string Shift = OpenQA.Selenium.Keys.Shift;

            /// <summary>
            /// Represents the Shift key.
            /// </summary>
            public static readonly string LeftShift = OpenQA.Selenium.Keys.LeftShift;

            /// <summary>
            /// Represents the Control key.
            /// </summary>
            public static readonly string Control = OpenQA.Selenium.Keys.Control;

            /// <summary>
            /// Represents the Control key.
            /// </summary>
            public static readonly string LeftControl = OpenQA.Selenium.Keys.LeftControl;

            /// <summary>
            /// Represents the Alt key.
            /// </summary>
            public static readonly string Alt = OpenQA.Selenium.Keys.Alt;

            /// <summary>
            /// Represents the Alt key.
            /// </summary>
            public static readonly string LeftAlt = OpenQA.Selenium.Keys.LeftAlt;

            /// <summary>
            /// Represents the Pause key.
            /// </summary>
            public static readonly string Pause = OpenQA.Selenium.Keys.Pause;

            /// <summary>
            /// Represents the Escape key.
            /// </summary>
            public static readonly string Escape = OpenQA.Selenium.Keys.Escape;

            /// <summary>
            /// Represents the Spacebar key.
            /// </summary>
            public static readonly string Space = OpenQA.Selenium.Keys.Space;

            /// <summary>
            /// Represents the Page Up key.
            /// </summary>
            public static readonly string PageUp = OpenQA.Selenium.Keys.PageUp;

            /// <summary>
            /// Represents the Page Down key.
            /// </summary>
            public static readonly string PageDown = OpenQA.Selenium.Keys.PageDown;

            /// <summary>
            /// Represents the End key.
            /// </summary>
            public static readonly string End = OpenQA.Selenium.Keys.End;

            /// <summary>
            /// Represents the Home key.
            /// </summary>
            public static readonly string Home = OpenQA.Selenium.Keys.Home;

            /// <summary>
            /// Represents the left arrow key.
            /// </summary>
            public static readonly string Left = OpenQA.Selenium.Keys.Left;

            /// <summary>
            /// Represents the left arrow key.
            /// </summary>
            public static readonly string ArrowLeft = OpenQA.Selenium.Keys.ArrowLeft;

            /// <summary>
            /// Represents the up arrow key.
            /// </summary>
            public static readonly string Up = OpenQA.Selenium.Keys.Up;

            /// <summary>
            /// Represents the up arrow key.
            /// </summary>
            public static readonly string ArrowUp = OpenQA.Selenium.Keys.ArrowUp;

            /// <summary>
            /// Represents the right arrow key.
            /// </summary>
            public static readonly string Right = OpenQA.Selenium.Keys.Right;

            /// <summary>
            /// Represents the right arrow key.
            /// </summary>
            public static readonly string ArrowRight = OpenQA.Selenium.Keys.ArrowRight;

            /// <summary>
            /// Represents the Left arrow key.
            /// </summary>
            public static readonly string Down = OpenQA.Selenium.Keys.Down;

            /// <summary>
            /// Represents the Left arrow key.
            /// </summary>
            public static readonly string ArrowDown = OpenQA.Selenium.Keys.ArrowDown;

            /// <summary>
            /// Represents the Insert key.
            /// </summary>
            public static readonly string Insert = OpenQA.Selenium.Keys.Insert;

            /// <summary>
            /// Represents the Delete key.
            /// </summary>
            public static readonly string Delete = OpenQA.Selenium.Keys.Delete;

            /// <summary>
            /// Represents the semi-colon key.
            /// </summary>
            public static readonly string Semicolon = OpenQA.Selenium.Keys.Semicolon;

            /// <summary>
            /// Represents the equal sign key.
            /// </summary>
            public static readonly string Equal = OpenQA.Selenium.Keys.Equal;

            // Number pad keys

            /// <summary>
            /// Represents the number pad 0 key.
            /// </summary>
            public static readonly string NumberPad0 = OpenQA.Selenium.Keys.NumberPad0;

            /// <summary>
            /// Represents the number pad 1 key.
            /// </summary>
            public static readonly string NumberPad1 = OpenQA.Selenium.Keys.NumberPad1;

            /// <summary>
            /// Represents the number pad 2 key.
            /// </summary>
            public static readonly string NumberPad2 = OpenQA.Selenium.Keys.NumberPad2;

            /// <summary>
            /// Represents the number pad 3 key.
            /// </summary>
            public static readonly string NumberPad3 = OpenQA.Selenium.Keys.NumberPad3;

            /// <summary>
            /// Represents the number pad 4 key.
            /// </summary>
            public static readonly string NumberPad4 = OpenQA.Selenium.Keys.NumberPad4;

            /// <summary>
            /// Represents the number pad 5 key.
            /// </summary>
            public static readonly string NumberPad5 = OpenQA.Selenium.Keys.NumberPad5;

            /// <summary>
            /// Represents the number pad 6 key.
            /// </summary>
            public static readonly string NumberPad6 = OpenQA.Selenium.Keys.NumberPad6;

            /// <summary>
            /// Represents the number pad 7 key.
            /// </summary>
            public static readonly string NumberPad7 = OpenQA.Selenium.Keys.NumberPad7;

            /// <summary>
            /// Represents the number pad 8 key.
            /// </summary>
            public static readonly string NumberPad8 = OpenQA.Selenium.Keys.NumberPad8;

            /// <summary>
            /// Represents the number pad 9 key.
            /// </summary>
            public static readonly string NumberPad9 = OpenQA.Selenium.Keys.NumberPad9;

            /// <summary>
            /// Represents the number pad multiplication key.
            /// </summary>
            public static readonly string Multiply = OpenQA.Selenium.Keys.Multiply;

            /// <summary>
            /// Represents the number pad addition key.
            /// </summary>
            public static readonly string Add = OpenQA.Selenium.Keys.Add;

            /// <summary>
            /// Represents the number pad thousands separator key.
            /// </summary>
            public static readonly string Separator = OpenQA.Selenium.Keys.Separator;

            /// <summary>
            /// Represents the number pad subtraction key.
            /// </summary>
            public static readonly string Subtract = OpenQA.Selenium.Keys.Subtract;

            /// <summary>
            /// Represents the number pad decimal separator key.
            /// </summary>
            public static readonly string Decimal = OpenQA.Selenium.Keys.Decimal;

            /// <summary>
            /// Represents the number pad division key.
            /// </summary>
            public static readonly string Divide = OpenQA.Selenium.Keys.Divide;

            // Function keys

            /// <summary>
            /// Represents the function key F1.
            /// </summary>
            public static readonly string F1 = OpenQA.Selenium.Keys.F1;

            /// <summary>
            /// Represents the function key F2.
            /// </summary>
            public static readonly string F2 = OpenQA.Selenium.Keys.F2;

            /// <summary>
            /// Represents the function key F3.
            /// </summary>
            public static readonly string F3 = OpenQA.Selenium.Keys.F3;

            /// <summary>
            /// Represents the function key F4.
            /// </summary>
            public static readonly string F4 = OpenQA.Selenium.Keys.F4;

            /// <summary>
            /// Represents the function key F5.
            /// </summary>
            public static readonly string F5 = OpenQA.Selenium.Keys.F5;

            /// <summary>
            /// Represents the function key F6.
            /// </summary>
            public static readonly string F6 = OpenQA.Selenium.Keys.F6;

            /// <summary>
            /// Represents the function key F7.
            /// </summary>
            public static readonly string F7 = OpenQA.Selenium.Keys.F7;

            /// <summary>
            /// Represents the function key F8.
            /// </summary>
            public static readonly string F8 = OpenQA.Selenium.Keys.F8;

            /// <summary>
            /// Represents the function key F9.
            /// </summary>
            public static readonly string F9 = OpenQA.Selenium.Keys.F9;

            /// <summary>
            /// Represents the function key F10.
            /// </summary>
            public static readonly string F10 = OpenQA.Selenium.Keys.F10;

            /// <summary>
            /// Represents the function key F11.
            /// </summary>
            public static readonly string F11 = OpenQA.Selenium.Keys.F11;

            /// <summary>
            /// Represents the function key F12.
            /// </summary>
            public static readonly string F12 = OpenQA.Selenium.Keys.F12;

            /// <summary>
            /// Represents the function key META.
            /// </summary>
            public static readonly string Meta = OpenQA.Selenium.Keys.Meta;

            /// <summary>
            /// Represents the function key COMMAND.
            /// </summary>
            public static readonly string Command = OpenQA.Selenium.Keys.Command;
        }

        /// <summary>Instructs Selenium to browse to the URL passed and not to throw any exception.</summary>
        /// <remarks>Executes the Selenium WebDriver .Navigate().GoToURL(FullPath) command.  If there is any exception catch it and return false.
        /// <para/><para/>
        /// Exception thrown can be seen by looking at the <see cref="TryException"/> property which is set to the exception thrown.
        /// </remarks>
        /// <seealso cref="TryException">Property referencing exception if thrown</seealso>
        /// <seealso cref="GotoURL"/>
        /// /// <param name="FullURLPath">Full URL of website to navigate to</param>
        /// <returns>True if not Selenium exception thrown, or false if exception thrown</returns>
        /// <example>Browse to Integ MemberAdmin ignoring any error but logging the exception message if any:
        /// <code lang="C#">
        /// if (!SeleniumDriver.GotoURL(@"https://www.google.com)"))
        /// {
        ///   // Oooops
        /// }
        /// </code></example>

        public bool TryGotoURL(string FullURLPath)
        {
            try
            {
                webDriver.Navigate().GoToUrl(FullURLPath);
                return true;
            }
            catch (Exception ex)
            {
                TryException = ex;
                return false;
            }
        }
        /// <summary>Instructs Selenium to browse to the URL passed.  If Selenium throws an error, test is aborted.</summary>
        /// <remarks>Executes the Selenium WebDriver .Navigate().GoToURL(FullPath) command.  If there is any exception the underlying tool is
        /// instructed to abort the test.
        /// <para/><para/>
        /// It is the responsibility of the tool (or wrapper) to gracefully handle the abort and throw an exception.  The test should have the steps wrapped to enable catching
        /// the abort exception to ensure graceful closure of the test.
        /// </remarks>
        /// <seealso cref="TryGotoURL"/>
        /// <param name="FullURLPath">Full URL of website to navigate to</param>
        /// <example>Try browsing to Integ MemberAdmin:
        /// <code lang="C#">
        /// try
        /// {
        ///   SeleniumDriver.GotoURL(@"https://www.google.com");
        /// }
        /// catch (Exception ex)
        /// {
        ///   // Test aborted!!
        ///   // Do stuff...
        /// }
        /// }</code></example>
        public void GotoURL(string FullURLPath)
        {
            if (!TryGotoURL(FullURLPath))
                throw new SeleniumExceptions.InvalidHostURI(TryException.Message);
            //   Alert alert = wait.until(ExpectedConditions.alertIsPresent());
            //   alert.authenticateUsing(new UserAndPassword("USERNAME", "PASSWORD"));

        }

        /// <summary>
        /// Clears the browser local storage and cookies for the current domain
        /// </summary>
        /// <remarks>This executes the Javascript localStorage.clear()</remarks>
        public void ClearBrowser()
        {
            ExecuteJavaScriptNoReturnData("window.localStorage.clear();");
            webDriver.Manage().Cookies.DeleteAllCookies();
        }

        /// <summary>Tests if element is currently visible to the user.</summary>
        /// <param name="Element">Element to test</param>
        /// <returns>True if visible, false if not (or if element is null)</returns>
        /// <remarks>
        /// This uses the Selenium Displayed boolean property.
        /// </remarks>
        public bool IsElementVisible(Element Element)
        {
            return IsElementVisible(Element, false);

        }

        /// <summary>Tests if element is currently visible to the user.</summary>
        /// <param name="Element">Element to test</param>
        /// <param name="CheckIfElementIsInViewport">If true, checks to see if element is in Viewport.  If false uses builtin Selenium element property</param>
        /// <returns>True if visible, false if not (or if element is null)</returns>
        /// <remarks>
        /// This uses the Selenium Displayed boolean property but ANDs it with a Javascript result that discovers if the element is in the viewport.  This
        /// fixes the Selenium issue whereby it returns a false TRUE (IE. Element is not actually in the Displayport....)
        /// </remarks>
        public bool IsElementVisible(Element Element, bool CheckIfElementIsInViewport)
        {
            if ((Element != null) && Element.WebElement != null)
            {
                if (CheckIfElementIsInViewport)
                {
                    bool SeleniumDisplayed = false;
                    bool MyDisplayed = false;
                    string sResult = null;
                    if (!TryExecuteJavaScript("var rect = arguments[0].getBoundingClientRect(); return ( rect.top >= 0 && rect.left >= 0 && rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) && rect.right <= (window.innerWidth || document.documentElement.clientWidth));", out sResult, Element.WebElement))
                    {
                        throw new Exception(string.Format("Selenium IsElementVisible javascript error: {0}", sResult));
                    }
                    SeleniumDisplayed = Element.WebElement.Displayed;
                    MyDisplayed = (sResult.ToLower() == "true");
                    Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element in Viewport = {0} ({1})", sResult, MyDisplayed ? "Yes" : "No");
                    return SeleniumDisplayed && MyDisplayed;
                }
                else
                    return Element.WebElement.Displayed;
            }
            else
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element displayed = [Web or element is NULL]");
                return false;
            }
        }

        public bool Displayed(IWebElement webElement)
        {
            bool result = webElement.Displayed;
            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element displayed = [{0}]", result ? "TRUE" : "FALSE");
            return result;
        }




        /// <summary>Tests is element is enabled (and so will accept text and/or click events)
        /// </summary>
        /// <param name="Element">Element to test</param>
        /// <returns>True if element is enabled, false if not</returns>
        public bool IsElementEnabled(Element Element)
        {
            if ((Element != null) && Element.WebElement != null)
                return Enabled(Element.WebElement);
            else
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element enabled = [Web or element is NULL]");
                return false;
            }
        }


        public bool Enabled(IWebElement webElement)
        {
            bool result = webElement.Enabled;
            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element enabled = [{0}]", result ? "TRUE" : "FALSE");
            return result;
        }

        /// <summary>Clears the text contents of the element if clearable</summary>
        /// <remarks>Sets the element's value property to an empty string then fires the onchange event</remarks>
        /// <param name="WebElement">IWebElement to clear</param>
        public void Clear(IWebElement WebElement)
        {
            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Clearing element");
            WebElement.Clear();
        }

        /// <summary>
        /// Sets the Selected state of a selectable element (IE. Checkbox, Radiobuttoin etc) as required
        /// </summary>
        /// <param name="WebElement">IWebElement to set selected status of</param>
        /// <param name="State">State to be set to.</param>
        public void SetSelected(IWebElement WebElement, bool State)
        {
            if ((!IsSelected(WebElement) && State) || (IsSelected(WebElement) && !State))
            {
                Click(WebElement);
            }
        }

        /// <summary>
        /// Returns boolean indicating if the element is selected or not
        /// </summary>
        /// <remarks>Assumes element is selectable</remarks>
        /// <param name="WebElement">Element to get selectable status from</param>
        /// <returns>True if selected, false if not</returns>
        public bool IsSelected(IWebElement WebElement)
        {
            bool result = WebElement.Selected;
            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Is Element Selected = {0}", result ? "TRUE" : "FALSE");
            return result;
        }


        /// <summary>Wraps Click event to enable us to make Clicking more clever if needed (IE. If Element may have an overlay)</summary>
        /// <param name="WebElement">IWebElement to click</param>
        /// <example>clicks MyButton
        /// <code lang="C#">
        /// SeleniumDriver.Click(myButton,true);
        /// </code>
        /// </example>
        public void Click(IWebElement WebElement)
        {
            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Clicking element (Selenium click action)");
            try
            {
                WebElement.Click();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Other element would receive the click"))
                {
                    //
                    // This is an old chestnut... Have had many an argument with the Selenium team about the 'isClickable'
                    // property.  It simply doesnt work and they wont change it 'for historical reasons' Eeeesh (See 
                    // https://code.google.com/p/selenium/issues/detail?id=6804 - Dickheads).
                    // So we need to throw a slightly more intelligent exception here so the tester, or underlying
                    // tool can fathom out what is going on...
                    //
                    // We'll use Javascript to find out what the topmost element is at the location the click is firing at.  At least we
                    // will then know what element is getting that there click...
                    //
                    int start = ex.Message.IndexOf("point (");
                    if (start > -1)
                    {
                        start += "point (".Length;
                        string loc = ex.Message.Substring(start, ex.Message.Length - start);
                        if (loc.Length > start && loc.Contains(')'))
                        {
                            loc = loc.Substring(0, loc.IndexOf(')')).Trim();
                            if (loc.Contains(','))
                            {
                                string[] xy = loc.Split(',');
                                IWebElement df;
                                bool TryResult = TryExecuteJavaScript("return document.elementFromPoint(arguments[0],arguments[1]);", out df, xy[0], xy[1]);
                                if (TryResult && df != null)
                                {
                                    string OffendingElement = df.GetAttribute("outerHTML");
                                    throw new SeleniumExceptions.ElementCannotBeClicked(string.Format("Element could not be clicked as another element would get the click.  The offending element is (Element and all children shown):\r\n{0}", OffendingElement), ex);
                                }
                            }
                        }
                    }
                }
                // No idea what it is - so whatever, rethrow...
                throw ex;
            }
        }

        /// <summary>Enters text in the passed IWebElement.  
        /// </summary>
        /// <param name="WebElement">Element to enter text in</param>
        /// <param name="Text">Text to be enetered</param>
        /// <remarks>This may need some work as test scripts mature and additional platforms/browsers targeted.  Selenium SendKeys is used to enter the text but there may be issues with hidden
        /// elements and with telling iOS/OSX systems (possibly with Appium as well) text entry has completed.</remarks>
        /// <example>
        /// <code lang="C#">
        /// SeleniumDriver.SetText(myTextBox,"Text to be entered");
        /// </code></example>
        public void SetText(IWebElement WebElement, string Text)
        {
            //
            // This method will probably start to expand as different browsers/devices highlight different issues with entering text....
            //
            // A possible need is to wait until text can be entered:-
            //IWebElement aa = ElementFindTimeout.Until((b) => { if (IsElementVisible(WebElement) && IsElementEnabled(WebElement)) return WebElement; else { Logger.WriteLn(this, "SetText", "Polling until Text can be entered"); return null; } });
            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Entering textusing Selenium SendKeys: [{0}].", Text);
            WebElement.SendKeys(Text);
        }

        /// <summary>Gets text from IWebElement using either innerText attribute or Selenium standard Text property, optionally scrolling element into view first.</summary>
        /// <param name="WebElement">IWebElement to get text from</param>
        /// <param name="IncludeDescendentsText">If true text from element and all descendents is used. If false UseInnerTextAttribute is ignored</param>
        /// <param name="ScrollIntoViewFirst">Indicate if eelement should be scrolled into view first</param>
        /// <param name="UseInnerTextAttribute">If true, uses innerText attribute, uses Selenium Text property if false</param>
        /// <returns>Text showing in element</returns>
        /// <remarks>
        /// Getting text from an element has two issues; do we want just the text the user can see in the passed element, or all text including possibly hidden text from
        /// child elements.  If UseInnerTextAttribute is set true, only text that is not hidden (by CSS) and in targeted element is returned.  If false, all text (including
        /// child elements) is returned.
        /// </remarks>
        /// <example> Get text from myTextbox element after scrolling into view and using Selenium standard Text property;
        /// <code lang="C#">
        /// string text = SeleniumDriver.GetText(myTextBox,true,false);
        /// </code></example>
        public string GetText(IWebElement WebElement,bool IncludeDescendentsText, bool ScrollIntoViewFirst, bool UseInnerTextAttribute)
        {
            //
            // This is a bit odd and there are two issues involved but which boil down to a single one.  GetText MUST only return text the user can see (or what
            // is the point of what we are trying to achive...).  So we must ensure we can see the text we are returning.  Note that there is one point being
            // deliberatly ignored; foreground/background text colour.  In an ideal world we should only return text where the colours are sufficiently different
            // for a human to read.  But, what is sufficient?  Should we take into account common colour-blindness issues etc etc etc....  So, just stay simple for
            // now. 
            //
            ///////
            //
            // We may want to scroll into view first; Because
            //          (a) A user would not be able to get/read the text if hidden so if they cannot see it then it would be a bad test that returned hidden text.
            //          (b) If the element is outside the viewport getText will return an empty string.
            // 
            if (ScrollIntoViewFirst) ScrollIntoView(WebElement);
            ///////
            //
            // We may use innerText rather than TextContent (as the Text propert does).  InnerText returns the text being presented to the user
            // whereas TextContent returns the raw text in all nodes within the element - irrelevant of whether hidden or not.
            //
            string text = string.Empty;
            if (IncludeDescendentsText)
            {
                if (UseInnerTextAttribute)
                {
                    text = WebElement.GetAttribute("innerText");
                    Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Get element text using element innerText attribute: [{0}]", text);
                }
                else
                {
                    text = WebElement.Text;
                    Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Get element text using Selenium Text property: [{0}]", text);
                }
            }
            else
            {
                string outerHTML = WebElement.GetAttribute("outerHTML");
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(outerHTML);
                HtmlNodeCollection a = htmlDocument.DocumentNode.SelectNodes("/*/text()");
                text = string.Join("", a.Select(o => o.InnerText));
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Get element text without descendants: [{0}]", text);
            }
            return text;
        }

        /// <summary>Applies Javascript to scroll element into view.
        /// </summary>
        /// <param name="WebElement">WebElement to be scrolled into view</param>
        /// <remarks>
        /// Executes javascript .scrollIntoView() to scroll element</remarks>
        /// <example>Scroll the Save button into view and click it;
        /// <code lang="C#">
        /// SeleniumDriver.ScrollIntoView(mySaveButton);
        /// SeleniumDriver.Click(mySaveButton);</code></example>
        public void ScrollIntoView(IWebElement WebElement)
        {
            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Scrolling element in to view (JavaScript inject - [Element].scrollIntoView()");
            ExecuteJavaScriptNoReturnData("arguments[0].scrollIntoView();", WebElement);
        }



        public bool WaitForElement(Element ElementToWaitFor, Visibility RequiredVisibility, TimeSpan? Timeout = null, TimeSpan? PollInterval = null)
        {
            bool DidReachRequiredVisibilityBeforeTimeout = false;
            int Itterations = 0;
            WebDriverWait actualTimeout = GetPollAndTimeout(elementFindTimings, Timeout, PollInterval);

            if (ElementToWaitFor==null)
            {
                if (RequiredVisibility == Visibility.Hidden) return true;
                throw new Exception("Element does not exist - it is null!");
            }
            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Wait {0}ms for element {1} to become {2}. Poll interval = {3}ms", actualTimeout.Timeout.TotalMilliseconds, ElementToWaitFor.MappingDetails.FriendlyName, RequiredVisibility.ToString(), actualTimeout.PollingInterval.TotalMilliseconds);

            // Stopwatch is only used so that we can log the time to the console....
            Stopwatch timeWaited = Stopwatch.StartNew();
            try
            {
                DidReachRequiredVisibilityBeforeTimeout = actualTimeout.Until<bool>((d) =>
                {
                    bool HaveWeSuccess = false;
                    try
                    {
                        Itterations++;
                        bool isDisplayed = (ElementToWaitFor != null && ElementToWaitFor.IsDisplayed);
                        Logger.WriteLine(Logger.LogLevels.FrameworkDebug,$"[{timeWaited.ElapsedMilliseconds}mS] Element displayed (exists and isDisplayed true): {isDisplayed}");
                        if (isDisplayed)
                        {
                            if (RequiredVisibility == Visibility.Visible)
                            {
                                HaveWeSuccess = true;
                            }
                        }
                        else
                        {
                            if (RequiredVisibility == Visibility.Hidden)
                            {
                                HaveWeSuccess = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(Logger.LogLevels.TestInformation, $"Unexpected exception waiting for element to be visible or not visible: {ex}");
                        throw;
                    }
                    return HaveWeSuccess;
                });
            }
            catch (WebDriverTimeoutException ex)
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "WebDriverTimeoutException after {0}ms ({1} itterations)", timeWaited.Elapsed.TotalSeconds.ToString(), Itterations.ToString());
                DidReachRequiredVisibilityBeforeTimeout = false;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Exception after {0}ms ({1} itterations)", timeWaited.Elapsed.TotalSeconds.ToString(), Itterations.ToString());
                throw new SeleniumExceptions.WaitTimeout(ElementToWaitFor.ParentOfThisElement,
                    ElementToWaitFor,
                    RequiredVisibility.ToString(),
                    timeWaited.Elapsed.TotalSeconds.ToString(),
                    actualTimeout.PollingInterval.TotalMilliseconds.ToString());
            }
            return DidReachRequiredVisibilityBeforeTimeout;
        }

        /// <summary>Waits specific time for IWebElement to be visible or hidden.  Throws Execption if timed out
        /// </summary>
        /// <param name="ParentElement">Element to find element under (if null find is from DOM level)</param>
        /// <param name="objectDetails">Find logic and Friendly name for element to be waited for</param>
        /// <param name="RequiredVisibility">True if wait until visible, False if wait until hidden</param>
        /// <param name="Timeout">Optional (Default: Selenium Timeout from config - 60 Seconds if not defined) - Timeout</param>
        /// <param name="PollInterval">Optional (Default: Selenium PollInterval from config - 500mS if not defined) - Polling interval while waiting</param>
        /// <remarks>
        /// Built-in waits (duration spinner, overlay etc...) should be used for standard C2 busy indicators.
        /// </remarks>
        /// <returns>True if success before timeout, or false if timedout</returns>
        /// <example>Wait for spinner to go away:
        /// <code lang="C#">
        /// if (!WaitForElement(mySpinner,"My very own spinner",true)) Messagebox.Show("Spinner did not go away!");</code>
        /// </example>
        /// <exception cref="Exceptions.WaitTimeout">Thrown if timeout occurs</exception>
        public bool WaitForElement(Element ParentElement, ObjectMappingDetails objectDetails, Visibility RequiredVisibility, TimeSpan? Timeout = null, TimeSpan? PollInterval = null)
        {
            return WaitForElement(ParentElement, objectDetails, RequiredVisibility, true, Timeout, PollInterval);
        }


        /// <summary>Waits specific time for IWebElement to be visible or hidden.  Throws Execption if timed out
        /// </summary>
        /// <param name="ParentElement">Element to find element under (if null find is from DOM level)</param>
        /// <param name="Mapping">Find logic and Friendly name for element to be waited for</param>
        /// <param name="RequiredVisibility">True if wait until visible, False if wait until hidden</param>
        /// <param name="CheckIsDisplayedStatus">If False, only the presence of the element is checked, <see cref="IsDisplayed"></see> status is not checked</param>
        /// <param name="Timeout">Optional (Default: Selenium Timeout from config - 60 Seconds if not defined) - Timeout</param>
        /// <param name="PollInterval">Optional (Default: Selenium PollInterval from config - 500mS if not defined) - Polling interval while waiting</param>
        /// <remarks>
        /// Built-in waits (duration spinner, overlay etc...) should be used for standard C2 busy indicators.
        /// </remarks>
        /// <returns>True if success before timeout, or false if timedout</returns>
        /// <example>Wait for spinner to go away:
        /// <code lang="C#">
        /// if (!WaitForElement(mySpinner,"My very own spinner",true)) Messagebox.Show("Spinner did not go away!");</code>
        /// </example>
        /// <exception cref="Exceptions.WaitTimeout">Thrown if timeout occurs</exception>
        public bool WaitForElement(Element ParentElement, ObjectMappingDetails Mapping, Visibility RequiredVisibility, bool CheckIsDisplayedStatus, TimeSpan? Timeout = null, TimeSpan? PollInterval = null)
        {
            bool DidReachRequiredVisibilityBeforeTimeout = false;
            Element WebElementFound = null;
            int Itterations = 0;
            WebDriverWait actualTimeout = GetPollAndTimeout(elementFindTimings, Timeout, PollInterval);

            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Wait {0}ms for element {1} to become {2} (Check displayed status = {3}). Poll interval = {4}ms", actualTimeout.Timeout.TotalMilliseconds, Mapping.FriendlyName, RequiredVisibility.ToString(), CheckIsDisplayedStatus, actualTimeout.PollingInterval.TotalMilliseconds);

            // Stopwatch is only used so that we can log the time to the console....
            Stopwatch timeWaited = Stopwatch.StartNew();
            try
            {
                DidReachRequiredVisibilityBeforeTimeout = actualTimeout.Until((d) =>
                {
                    bool HaveWeSuccess = false;
                    try
                    {
                        // Call the Find Element (driver or element based). Set the timeout to zero as we will deal with polling and time here (we may be looking for element to be NOT there!...)
                        try
                        {
                            WebElementFound = FindElement(ParentElement, Mapping, TimeSpan.MinValue, TimeSpan.MinValue);
                        }
                        catch (SeleniumExceptions.FindLogicReturnedNoElements)
                        {
                            WebElementFound = null;
                        }
                        Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "[{0}mS]: Element {1} ",timeWaited.ElapsedMilliseconds, (WebElementFound==null)?"not found":$"found (Displayed - {WebElementFound.IsDisplayed})");
                        Itterations++;

                        if (WebElementFound==null)
                        {
                            // Element was NOT found.  If we wanted the element to be not there or not displayed (same thing) all good.
                            if (RequiredVisibility == Visibility.Hidden) HaveWeSuccess = true;
                        }
                        else
                        {
                            if (CheckIsDisplayedStatus)
                            {
                                bool isDisplayed = WebElementFound.IsDisplayed;
                                if (isDisplayed && RequiredVisibility == Visibility.Visible)
                                {
                                    // If checking display status, element is displayed and want it to be visible all good
                                    HaveWeSuccess = true;
                                }
                                if (!isDisplayed && RequiredVisibility == Visibility.Hidden)
                                {
                                    // If checking display status, element is NOT displayed and we want it not visible all good
                                    HaveWeSuccess = true;
                                }
                            }
                            else
                            {
                                // Element WAS found.  We are not checking visibiklity so all good
                                if (RequiredVisibility == Visibility.Visible) HaveWeSuccess = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (RequiredVisibility == Visibility.Hidden)
                        {
                            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Exception thrown but element not visible wanted so success. Exception: {0}", (WebElementFound == null) ? "Element not found" : "Element found but not displayed", ex.Message);
                            HaveWeSuccess = true;
                        }
                    }
                    return HaveWeSuccess;
                });
            }
            catch (WebDriverTimeoutException ex)
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "WebDriver Timeout Exception after {0}ms ({1} itterations)", timeWaited.Elapsed.TotalMilliseconds.ToString(), Itterations.ToString());
                DidReachRequiredVisibilityBeforeTimeout = false;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Exception after {0}ms ({1} itterations)", timeWaited.Elapsed.TotalSeconds.ToString(), Itterations.ToString());
                throw;
            }
            return DidReachRequiredVisibilityBeforeTimeout;
        }
    }
}
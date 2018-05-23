using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using TeamControlium.Utilities;

namespace TeamControlium.Controlium
{
    public partial class Element
    {
        private ObjectMappingDetails mappingDetails;
        private IWebElement webElement; // This is the actual Selenium element.  To use this element, this must be set.... 
        private dynamic _ParentOfThisElement; //Parent of this element.  Must be either another element OR SeleniumDriver

        /// <summary>
        /// Sets or Get the instance of the Selenium Web-Element being used.
        /// </summary>
        public IWebElement WebElement { get { return webElement; } set { webElement = value; } }


        //PROPERTIES
        /// <summary>
        /// Returns find logic used (or to be used) to locate this element either from the top level (See <see cref="Element(SeleniumDriver,IWebElement,string,FindBy)"/>) or from the parent element (See <see cref="Element(Element,IWebElement,string,FindBy)"/>)
        /// </summary>
        public ObjectMappingDetails MappingDetails { get { return mappingDetails; } set { if ((mappingDetails != null) && (WebElement != null)) throw new Exception("Mapping Details cannot be changed after binding to Selenium WebElement"); else mappingDetails = value; } }


        /// <summary>
        /// Last Exception thrown from a Try method
        /// </summary>
        public Exception TryException { get; private set; }

        /// <summary>
        /// Returns the instance of SeleniumDriver we are using
        /// </summary>
        public SeleniumDriver seleniumDriver
        {
            get
            {
                // We can only do this if this element has a Parent (DOM or another element)
                if (ParentOfThisElement == null)
                    throw new Exception("Cannot get an instance of the Selenium Driver as this element (or a Parent of) does not have a Parent!");
                //
                // parent is an Element all the way up the tree until the top level, when it is the SeleniumDriver....  We just
                // itterate up the tree recursivley until we hit the Selenium Driver
                //
                dynamic parentTally = ParentOfThisElement;
                if (ParentOfThisElement.GetType() == typeof(SeleniumDriver))
                    return ((SeleniumDriver)ParentOfThisElement);
                else
                    return ((Element)ParentOfThisElement).seleniumDriver;
            }
        }

        public bool WaitForElementHeightStable(TimeSpan? Timeout = null)
        {
            ThrowIfUnbound();
            bool didStabilzeBeforeTimeout = false;
            int Itterations = 0;
            TimeSpan actualTimeout = Timeout ?? seleniumDriver.ElementFindTimeout;

            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Wait {0}ms for element {1} to become height stable", actualTimeout.TotalMilliseconds, MappingDetails.FriendlyName);

            Stopwatch timeWaited = Stopwatch.StartNew();
            while (timeWaited.ElapsedMilliseconds < actualTimeout.TotalMilliseconds)
            {
                //
                // We dont have a poll delay as the Height stabalization monitor uses a time delta to see if the height is stable.  That in effect
                // is a poll interval (which will also work on the cloud)
                //
                try
                {
                    Itterations++;
                    if (IsHeightStable)
                    {
                        didStabilzeBeforeTimeout = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Cannot determine if element [{MappingDetails.FriendlyName}] is height-stable: {ex}");
                }
            }

            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element height stable after {0}ms ({1} itterations)", timeWaited.Elapsed.TotalSeconds.ToString(), Itterations.ToString());
            return didStabilzeBeforeTimeout;
        }

        public bool WaitForElementPositionStable(TimeSpan? Timeout = null)
        {
            ThrowIfUnbound();
            bool didStabilzeBeforeTimeout = false;
            int Itterations = 0;
            TimeSpan actualTimeout = Timeout ?? seleniumDriver.ElementFindTimeout;

            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Wait {0}ms for element {1} to become position stable", actualTimeout.TotalMilliseconds, MappingDetails.FriendlyName);


            Stopwatch timeWaited = Stopwatch.StartNew();
            while (timeWaited.ElapsedMilliseconds < actualTimeout.TotalMilliseconds)
            {
                //
                // We dont have a poll delay as the Height stabalization monitor uses a time delta to see if the height is stable.  That in effect
                // is a poll interval (which will also work on the cloud)
                //
                try
                {
                    Itterations++;
                    if (IsPositionStable)
                    {
                        didStabilzeBeforeTimeout = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Cannot determine if element [{MappingDetails.FriendlyName}] is position-stable: {ex}");
                }
            }

            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Element position stable after {0}ms ({1} itterations)", timeWaited.Elapsed.TotalSeconds.ToString(), Itterations.ToString());
            return didStabilzeBeforeTimeout;
        }


        /// <summary>
        /// Binds the Element to the WebElement
        /// </summary>
        /// <returns></returns>
        public Element BindWebElement()
        {
            // We can only do this if this element has a Parent (DOM or another element) and has Mapping details
            if (HasAParent)
                throw new Exception("Cannot find a WebElement without having a parent (SeleniumDriver or Element)!");
            if (HasMappingDetails)
            {
                throw new Exception("Cannot find a WebElement if mapping details (find logic) are unknown!");
            }

            Element foundElement;
            try
            {
                if (ParentOfThisElement.GetType() == typeof(SeleniumDriver))
                {
                    foundElement = ((SeleniumDriver)ParentOfThisElement).FindElement(MappingDetails);
                }
                else
                {
                    foundElement = ((Element)ParentOfThisElement).FindElement(MappingDetails);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Unable to bind to element {0} (Find Logic ({1}): [{2}])", MappingDetails.FriendlyName, MappingDetails.FindType.ToString(), MappingDetails.FindLogic), ex);
            }
            WebElement = foundElement.WebElement;
            MappingDetails = foundElement.MappingDetails;
            return this;
        }

        /// <summary>
        /// Returns the selected item if a Select type element
        /// </summary>
        /// <returns></returns>
        public Element SelectedItem()
        {
            ThrowIfUnbound();
            try
            {
                SelectElement selectElement = new SelectElement(this.WebElement);
                IWebElement selectedElement = selectElement.SelectedOption;
                Element select = new Element(this);
                select.WebElement = selectedElement;
                return select;
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot get selected item.", ex);
            }
        }



        /// <summary>Performs a clear action on the element</summary>
        public Element Clear()
        {
            ThrowIfUnbound();
            seleniumDriver.Clear(WebElement);
            return this;
        }

        /// <summary>
        /// Gets the selected status for the element
        /// </summary>
        /// <remarks>Assumes element is a type that can have a selected status (IE. Checkbox, list item etc...)</remarks>
        /// <returns>True if selected, false if not</returns>
        public bool Selected()
        {
            ThrowIfUnbound();
            return seleniumDriver.IsSelected(WebElement);
        }

        public void SetSelected(bool State)
        {
            ThrowIfUnbound();
            seleniumDriver.SetSelected(WebElement, State);
        }

        /// <summary>
        /// Performs a mouse hover-over the element
        /// </summary>
        public void Hover()
        {
            ThrowIfUnbound();
            (new Actions(seleniumDriver.WebDriver)).MoveToElement(webElement).Build().Perform();
        }

        /// <summary>Performs a left-mouse-button click in the element
        /// </summary>
        /// <exception cref="ElementCannotBeClicked">Thrown if element cannot be clicked or has not received the click event</exception>
        public void Click()
        {
            ThrowIfUnbound();
            seleniumDriver.Click(WebElement);
        }
        /// <summary>Performs a left-mouse-button click in the element.  Any exception is caught and not re-thrown.
        /// </summary>
        /// <returns>False in an exception was thrown.  See <see cref="Element.TryException"/> for actual exception if required.</returns>
        public bool TryClick()
        {
            ThrowIfUnbound();
            try
            {
                Click();
                TryException = null;
                return true;
            }
            catch (Exception ex)
            {
                TryException = ex;
                return false;
            }
        }

        /// <summary>Clears field then enters text, using keyboard, into element</summary>
        /// <param name="Text">Text to enter</param>
        /// <seealso cref="seleniumDriver.SetText(IWebElement,string)"/>
        public void SetText(string Text,int maxretries=3,TimeSpan? retryInterval=null)
        {
            ThrowIfUnbound();

            int retryIndex = 0;
            Exception lastException=null;
            TimeSpan interval = retryInterval ?? TimeSpan.FromMilliseconds(100);
            try
            {
                while (++retryIndex <= maxretries || maxretries == 0)
                {
                    try
                    {
                        seleniumDriver.Clear(this.WebElement);
                        EnterText(Text);
                        if (retryIndex > 1)
                            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "{0} attempts with InvalidElementStateException.  Last attempt good.)", retryIndex);
                        return;
                    }
                    catch (OpenQA.Selenium.InvalidElementStateException ex)
                    {
                        Thread.Sleep(interval);
                        lastException = ex;
                    }
                }
                throw lastException;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "{0} failed attempts to set [{1}] to text [{2}]", retryIndex,this.MappingDetails?.FriendlyName??"???!",Text);
                throw new UnableToSetOrGetText(MappingDetails, Text, ex);
            }
        }

        /// <summary>Enters text, using keyboard, into element</summary>
        /// <param name="Text">Text to enter</param>
        /// <seealso cref="seleniumDriver.SetText(IWebElement,string)"/>
        public void EnterText(string Text,int maxretries= 3, TimeSpan? retryInterval = null)
        {
            ThrowIfUnbound();
            int retryIndex = 0;
            Exception lastException = null;
            TimeSpan interval = retryInterval ?? TimeSpan.FromMilliseconds(100);
            try
            {
                while (++retryIndex <= maxretries || maxretries == 0)
                {
                    try
                    {
                        seleniumDriver.SetText(this.WebElement, Text ?? string.Empty);
                        if (retryIndex > 1)
                            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "{0} attempts with InvalidElementStateException.  Last attempt good.)", retryIndex);
                        return;
                    }
                    catch (OpenQA.Selenium.InvalidElementStateException ex)
                    {
                        Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Attempt {0} failed (InvalidElementStateException)", retryIndex);
                        Thread.Sleep(interval);
                        lastException = ex;
                    }
                }
                throw lastException;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "{0} failed attempts to set [{1}] to text [{2}]", retryIndex, this.MappingDetails?.FriendlyName ?? "???!", Text);
                throw new UnableToSetOrGetText(MappingDetails, Text, ex);
            }
        }

        /// <summary>Enters text, using keyboard, into element, pressing Enter Key at the end</summary>
        /// <param name="Text">Text to enter</param>
        /// <seealso cref="seleniumDriver.SetText(IWebElement,string)"/>
        public void SetTextAndPressEnter(string Text)
        {
            ThrowIfUnbound();
            SetText((Text ?? string.Empty) + Keys.Enter);
        }


        /// <summary>Clears field then enters text, using keyboard, into element.  No exception is thrown.</summary>
        /// <param name="Text">Text to enter</param>
        /// <returns>False in any exception was thrown</returns>
        /// <seealso cref="seleniumDriver.SetText(IWebElement,string)"/>
        /// <seealso cref="seleniumDriver.TryException"/>
        public bool TrySetText(string Text)
        {
            ThrowIfUnbound();
            try
            {
                SetText(Text);
                TryException = null;
                return true;
            }
            catch (Exception ex)
            {
                TryException = ex;
                return false;
            }
        }

        /// <summary>
        /// Scroll the element into view
        /// </summary>
        /// <remarks>Uses Javascript inject to perform scroll</remarks>
        public void ScrollIntoView()
        {
            ThrowIfUnbound();
            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Scrolling [{0}] element into view", MappingDetails.FriendlyName);
            seleniumDriver.ScrollIntoView(WebElement);
        }

        /// <summary>Scrolls window so that target is in view and then gets visible text from element
        /// </summary>
        /// <remarks>Element is scrolled into view before text is harvested.  See <see cref="seleniumDriver.GetText(IWebElement,bool,bool)"/> for details.</remarks>
        /// <returns>Text from element</returns>
        public string ScrollIntoViewAndGetText()
        {
            ThrowIfUnbound();
            return ScrollIntoViewAndGetText(true);
        }


        /// <summary>Scrolls window so that target is in view and then gets visible text from element
        /// </summary>
        /// <param name="IncludeDesendants">If true all text is returned. If false only text from current element is returned.</param>
        /// <remarks>Element is scrolled into view before text is harvested.  See <see cref="seleniumDriver.GetText(IWebElement,bool,bool,bool)"/> for details.</remarks>
        /// <returns>Text from element</returns>
        public string ScrollIntoViewAndGetText(bool IncludeDesendants)
        {
            ThrowIfUnbound();
            //
            // Only get the visible text - ensure what IS visible by scrolling into view (but NOT if using IE8 as IE8 has an issue with scrolling if there is overflow into the x-axis)...
            //
            if (SeleniumDriver.TestBrowser == SeleniumDriver.Browsers.IE8)
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Not scrolling into view as Browser IE8!");
            return seleniumDriver.GetText(this.WebElement, IncludeDesendants, (SeleniumDriver.TestBrowser != SeleniumDriver.Browsers.IE8), false);
        }

        /// <summary>Gets visible text from element
        /// </summary>
        /// <remarks>Element is scrolled into view before text is harvested.  See <see cref="seleniumDriver.GetText(IWebElement,bool,bool)"/> for details.</remarks>
        /// <returns>Text from element</returns>
        public string GetText()
        {
            ThrowIfUnbound();
            return GetText(true);
        }

        public string GetText(bool IncludeDesendants)
        {
            ThrowIfUnbound();
            //
            // Only get the visible text - ensure what IS visible by scrolling into view (NOT if using IE)...
            //
            return seleniumDriver.GetText(WebElement, IncludeDesendants, false, false);
        }

        /// <summary>Gets visible text from element</summary>
        /// <param name="Text">Text from element</param>
        /// <returns>False if exception was thrown.</returns>
        /// <seealso cref="seleniumDriver.TryException"/>
        public bool TryGetText(out string Text)
        {
            ThrowIfUnbound();
            return TryGetText(true, out Text);
        }

        public bool TryGetText(bool IncludeDesendants, out string Text)
        {
            ThrowIfUnbound();
            try
            {
                Text = GetText();
                TryException = null;
                return true;
            }
            catch (Exception ex)
            {
                Text = string.Empty;
                TryException = ex;
                return false;
            }
        }
        /// <summary>Get all text from element (and descendant elements)
        /// </summary>
        /// <remarks>Element is scrolled into view before text is harvested.  See <see cref="seleniumDriver.GetText(IWebElement,bool,bool,bool)"/> for details.</remarks>
        /// <returns>Text from element (and descendants)</returns>
        public string GetAllText()
        {
            ThrowIfUnbound();

            // Just do the simple Text attribute thing.  Stuff if the user cant see the text!  However, still scroll into view as Selenium has an issue with that and
            // sometimes it would return a blank string if not in the viewport....
            return seleniumDriver.GetText(WebElement, true, true, false);
        }
        /// <summary>Get all text from element (and descendant elements)
        /// </summary>
        /// <param name="Text">Text from element</param>
        /// <returns>False if exception was thrown.</returns>
        /// <seealso cref="seleniumDriver.TryException"/>
        public bool TryGetAllText(out string Text)
        {
            ThrowIfUnbound();
            try
            {
                Text = GetAllText();
                TryException = null;
                return true;
            }
            catch (Exception ex)
            {
                Text = string.Empty;
                TryException = ex;
                return false;
            }
        }
        /// <summary>Get attribute text from current element
        /// </summary>
        /// <param name="Attribute">Attribute to get</param>
        /// <returns>Text in named attribute</returns>
        public string GetAttribute(string Attribute)
        {
            ThrowIfUnbound();

            string returnValue = WebElement.GetAttribute(Attribute);
            if ((returnValue == null)) throw new AttributeReturnedNull(MappingDetails, Attribute);
            return returnValue;
        }
        /// <summary>Get attribute test from current element.  Does not throw exception.
        /// </summary>
        /// <param name="Attribute">Attribute to get</param>
        /// <param name="AttributeText">Text in named attribute</param>
        /// <returns>True if attribute text got or false in an exception thrown</returns>
        public bool TryGetAttribute(string Attribute, out string AttributeText)
        {
            ThrowIfUnbound();
            try
            {
                AttributeText = GetAttribute(Attribute);
                TryException = null;
                return true;
            }
            catch (Exception ex)
            {
                AttributeText = string.Empty;
                TryException = ex;
                return false;
            }
        }
        /// <summary>
        /// Throws exception if Element is not bound to a Selenium IWebElement.
        /// </summary>
        private void ThrowIfUnbound()
        {
            if (!HasAParent)
                throw new Exception("Cannot identify Selenium Driver as no parent set (SeleniumDriver or Element)!");
            if (!IsBoundToAWebElement)
                throw new Exception("Not bound to a Selenium Web Element");
        }
    }
}

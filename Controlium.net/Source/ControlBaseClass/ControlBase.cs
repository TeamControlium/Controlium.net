﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TeamControlium.Utilities;

namespace TeamControlium.Controlium
{
    /// <summary>An abstract Core control all other Core controls inherit from.  Contains all methods and properties that are common to TeamControlium controls.
    /// <para/><para/>It should be noted that it is possible to perform an illegal action against a control.  As an example, calling SetText (See <see cref="SetText(FindBy, string, string)"/> or <see cref="SetText(string)"/>) against a TeamControlium button
    /// control will result in an exception.  It is the responsibility of the Test code to call only legal actions against a TeamControlium control.  Controls may extend these methods and properties
    /// depending on the functionality of the control; for example, a Dropdown control driver may have a SelectItem(Iten identification) method to select a dropdown item.</summary>
    public abstract partial class ControlBase
    {
        // DELEGATES
        private delegate Element ControlFindElement(ObjectMappingDetails findLogic);

        /// <summary>Actual top level element of this control</summary>
        protected Element _RootElement { get; set; }

        /// <summary>Find logic used to locate this control from the root or Parent control</summary>
        public ObjectMappingDetails Mapping { get { return _RootElement.Mapping; } }

        /// <summary>
        /// Tests is controls root element is stale
        /// </summary>
        public bool IsStale
        {
            get
            {
                try
                {
                    //
                    // We do a dummy action to force Selenium to report any stagnantation....
                    //
                    bool dummy = RootElement.IsEnabled;
                    return false;
                }
                catch (Exception ex)
                {
                    Log.LogException(ex);
                    return true;
                }
            }
        }


        /// <summary>
        /// Refreshes current control to clear Stale elements (all way to DOM) if needed.
        /// </summary>
        /// <remarks>
        /// Only the control's RootElement is updated.  Complex/dynamic find-logics may break the refresh system....
        /// 
        /// In a Control an example may be:-
        /// 
        /// if (IsStale)
        /// {
        ///   Refresh();
        /// }
        /// 
        /// Note. Do not use ALL the time, only when required as network overhead in Selenese is high and will slow tests....
        /// </remarks>
        public void Refresh()
        {
            if ((this.ParentControl != null) && this.ParentControl.IsStale)
            {
                Log.LogWriteLine(Log.LogLevels.TestInformation, "Parent control is stale. Refreshing");
                ParentControl.RootElement.WebElement = null;
                ControlBase RefreshedParentControl = ControlBase.SetControl(ParentControl.SeleniumDriver, ParentControl.ParentControl, ParentControl);
                ParentControl = RefreshedParentControl;
            }
            RootElement = FindControlRootElement(FindToUse(SeleniumDriver, ParentControl), this.Mapping);
        }


        // PROPERTIES
        /// <summary>This controls Parent.  If a root control this is null</summary>
        public ControlBase ParentControl { get; private set; }

        /// <summary>Root element of this control</summary>
        public Element RootElement { get { if (_RootElement == null) throw new Exception($"Control [{Mapping?.FriendlyName ?? "Not Set!!?"}] root element NULL.  Has control been located on the page (IE. SetControl(...)?"); else { return _RootElement; } } protected set { _RootElement = value; } }

        /// <summary>Reference to the SeleniumDriver</summary>
        public SeleniumDriver SeleniumDriver { get; private set; }

        /// <summary>
        /// Clear the control cache
        /// </summary>
        public static void ClearCache()
        {
            ControlCache.ClearCache();
        }

        protected void SetRootElement(ObjectMappingDetails mappingDetails)
        {
            RootElement = new Element(mappingDetails);
        }

        /// <summary>Sets on a child control with the childs find logic being applied this controls root element.  Method can be overriden by control implementations.
        /// </summary>
        /// <param name="NewControl">Control to be located</param>
        /// <returns>Reference to located control (or cached reference.)</returns>
        public virtual T SetControl<T>(T NewControl) where T : ControlBase
        {
            return SetControl(this, NewControl);
        }

        /// <summary>Instantiates a new TeamControlium Control as a child of the passed ParentControl. If no parent, Control is top-level.
        /// </summary>
        /// <param name="SeleniumDriver">Reference to TeamControlium Driver</param>
        /// <param name="ParentControl">Control the instantiated control is a child of. Can be null if new control is a top-level Control (IE. A function/pattern)</param>
        /// <param name="CustomRootElement">A dynamic Element the child control may be referenced from.  Can be null if child's find logic is from the parents root</param>
        /// <param name="NewControl">New control instance with find logic and friendly name set</param>
        /// <remarks>If control was sucessfully located, caching is not disabled and the control is cached, the cached reference is returned instead of the located.  If a cache miss the reference is
        /// added.</remarks>
        /// <returns>Reference to found control (or cached - see Remarks)</returns>
        public static T SetControl<T>(SeleniumDriver SeleniumDriver, ControlBase ParentControl, T NewControl) where T : ControlBase
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            try
            {
                //Log.LogWriteLine(Log.LogLevels.TestInformation, "Setting on control [{0}] from parent [{1}]", NewControl == null ? "<No control>" : NewControl.Mapping?.FriendlyName ?? NewControl.Mapping?.FindLogic ?? "No find logic!!", (ParentControl == null) ? "<No parent - so top level>" : ParentControl.Mapping?.FriendlyName ?? ParentControl.Mapping?.FindLogic ?? "Paraent has no find logic!!");
                // Find the element - we can assume it will be good as any issue will have chucked an exception
                if ((ParentControl != null) && ParentControl.IsStale)
                {
                    Log.LogWriteLine(Log.LogLevels.TestInformation, "Parent control is stale. Refreshing");
                    ParentControl.RootElement.WebElement = null;
                    ControlBase RefreshedParentControl = ControlBase.SetControl(ParentControl.SeleniumDriver, ParentControl.ParentControl, ParentControl);
                    ParentControl = RefreshedParentControl;
                }

                //
                // We may just be wrapping an Element in a Control that has already been found.  In which case, dont bother
                // to do a find for it....
                //
                if (NewControl?._RootElement?.WebElement == null)
                {
                    Log.LogWriteLine(Log.LogLevels.TestDebug, $"New control Root is null (It has not yet been found), so finding (Find logic: [{NewControl?.Mapping?.FindLogic ?? "Null!!!"}]");
                    ControlFindElement finder = FindToUse(SeleniumDriver, ParentControl);

                    Element Element = FindControlRootElement(finder, NewControl.Mapping);

                    NewControl.RootElement = Element;
                }

                //
                // Populate new Control object.
                //
                NewControl.SeleniumDriver = SeleniumDriver;
                NewControl.ParentControl = ParentControl; // This may be null.  So, new control is top level....

                // Put the control through the cache - either adds it to the cache (miss) or references the cache version (hit)
                // Inform the control we have performed a 'set' on it letting it know whether this is the first or subsequent times.  Note
                // that caching has been DISABLED for development of the framework purposes. Check will return false if the control EXISTS OR NOT!
                switch (ControlCache.Check(ref NewControl))
                {
                    case ControlCache.ControlCacheStates.CacheHit:
                        {
                            Log.LogWriteLine(Log.LogLevels.TestInformation, "Using cached control");
                            NewControl.ControlBeingSet(false);
                            break;
                        }
                    case ControlCache.ControlCacheStates.CacheMiss:
                        {
                            Log.LogWriteLine(Log.LogLevels.TestInformation, "Control not cached - new control used");
                            NewControl.ControlBeingSet(true);
                            break;
                        }
                    case ControlCache.ControlCacheStates.CachedControlWasStale:
                        {
                            Log.LogWriteLine(Log.LogLevels.TestInformation, "Control stale - all cached controls invalidated and new control used");
                            NewControl.ControlBeingSet(true);
                            break;
                        }
                    default:
                        {
                            Log.LogWriteLine(Log.LogLevels.TestInformation, "Control caching disabled");
                            NewControl.ControlBeingSet(true);
                            break;
                        }
                }
                return NewControl;
            }
            catch (Exception ex)
            {
                // Be worth sticking a log in here
                throw ex;
            }
        }

        /// <summary>Instantiates a new TeamControlium Control as a child of the passed ParentControl. If no parent, Control is top-level.
        /// </summary>
        /// <param name="SeleniumDriver">Reference to TeamControlium Driver</param>
        /// <param name="ParentControl">Control the instantiated control is a child of. Can be null if new control is a top-level Control (IE. A function/pattern)</param>
        /// <param name="NewControl">New control instance with find logic and friendly name set</param>
        /// <remarks>If control was sucessfully located, caching is not disabled and the control is cached, the cached reference is returned instead of the located.  If a cache miss the reference is
        /// added.</remarks>
        /// <returns>Reference to found control (or cached - see Remarks)</returns>
        public static T SetControl<T>(ControlBase ParentControl, T NewControl)
            where T : ControlBase
        {
            return SetControl(ParentControl.SeleniumDriver, ParentControl, NewControl);
        }


        /// <summary>Instantiates a new root TeamControlium Control
        /// </summary>
        /// <param name="SeleniumDriver">SeleniumDriver reference</param>
        /// <remarks>If control was sucessfully located, caching is not disabled and the control is cached, the cached reference is returned instead of the located.  If a cache miss the reference is
        /// added.</remarks>
        /// <returns>Reference to found control (or cached - see Remarks)</returns>     
        public static T SetControl<T>(SeleniumDriver SeleniumDriver, T NewControl) where T : ControlBase
        {
            return SetControl(SeleniumDriver, null, NewControl);
        }

        /// <summary>Sends a clear event to an element within the control.  Find logic must work from the top-element of the control.
        /// </summary>
        /// <param name="findLogic">Find logic to element that is to be cleared</param>
        /// <param name="friendlyName">Name of element</param>
        public void ClearElement(ObjectMappingDetails findLogic, string friendlyName)
        {
            FindElement(findLogic).Clear();
        }

        /// <summary>Send a click (or Tap) event to the root element of the control.
        /// <para/><para/>
        /// Implementors may override to route click event to the appropriate element in the control. If not overridden,
        /// click event is targeted to the controls root element.
        /// </summary>
        public virtual void Click()
        {
            Log.LogWriteLine(Log.LogLevels.TestDebug, "Clicking element [{0}]", RootElement?.Mapping?.FriendlyName ?? RootElement?.Mapping?.FindLogic ?? "Dunno!");
            RootElement.Click();
        }

        /// <summary>Sends a click event to an element within the control.  Find logic must work from the top-element of the control.
        /// </summary>
        /// <param name="findLogic">Find logic for element to be clicked</param>
        /// <param name="friendlyName">Name of element to clicked</param>
        public void ClickElement(ObjectMappingDetails findLogic)
        {
            FindElement(findLogic).Click();
        }

        /// <summary>Sends a click event to an element within the control if that element is exists.  Find logic must work from the top-element of the control.
        /// Does not throw an error if the element does not exist or is not visible.  If element does not exist, nothing is done and no error thrown.
        /// </summary>
        /// <param name="findLogic">Find logic to element</param>
        /// <param name="friendlyName">Name of element</param>
        public void ClickElementIfExists(ObjectMappingDetails findLogic)
        {
            Element element;
            if (RootElement.TryFindElement(findLogic, out element))
            {
                element.Click();
            }
            else
            {
                Log.LogWriteLine(Log.LogLevels.TestInformation, $"[{findLogic.FindLogic}] didnt find any match.  No click.");
            }
        }

        // METHODS
        /// <summary>A virtual method that Controls inheriting this CoreLib_Control can override.  The method is called
        /// when SetControl has identified the new control's root element and has populated the control's metadata.
        /// </summary>
        /// <param name="IsFirstSetting">
        /// Indicates if this is the first time the control has been located and instantiated (true)
        /// Or if the control has been used before in this test and has just had the IWebElement refreshed.
        /// </param>
        public virtual void ControlBeingSet(bool IsFirstSetting)
        {
        }

        /// <summary>Gets the attribute from an element within the control.  If element does not exist a NotFound exception is thrown. If attribute does not exist an empty string is returned
        /// </summary>
        /// <param name="findLogic">Find logic reletive to the root element of the control</param>
        /// <param name="friendlyName">Friendly name of the element the text is coming from</param>
        /// <param name="attributeName">Attribute to get contents of</param>
        /// <returns>Contenst of attribute</returns>
        public string GetAttribute(ObjectMappingDetails findLogic, string attributeName)
        {
            Element element = FindElement(findLogic);
            try
            {
                return element.GetAttribute(attributeName);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetAttribute(string attributeName)
        {
            try
            {
                return RootElement.GetAttribute(attributeName);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>Gets the text from an element within the control.  If element does not exist, exception is thrown.
        /// </summary>
        /// <param name="findLogic">Find logic reletive to the root element of the control</param>
        /// <param name="friendlyName">Friendly name of the element the text is coming from</param>
        /// <returns>text in identified element (uses Element.GetText)</returns>
        public string GetText(ObjectMappingDetails findLogic, string friendlyName)
        {
            Element element = FindElement(findLogic);
            return element.GetText();
        }

        /// <summary>Gets the text from root element of control.
        /// </summary>
        /// <returns>text in control root element (uses Element.GetText)</returns>
        public string GetText(bool IncludeDesendants = true, bool ScrollIntoViewFirst = false, bool UseInnerTextAttribute = false)
        {
            return RootElement.GetText(IncludeDesendants,ScrollIntoViewFirst,UseInnerTextAttribute);
        }

        /// <summary>Returns true if element has defined attribute and false if not.  If element does not exist an exception is thrown
        /// </summary>
        /// <param name="findLogic">Find logic of the element, reletive to the root element of the control</param>
        /// <param name="friendlyName">Friendly name of the element to be checked</param>
        /// <param name="attributeName">Attribute to see if exists</param>
        /// <returns>true if attributeName exists, false if not</returns>
        public bool HasAttribute(ObjectMappingDetails findLogic, string friendlyName, string attributeName)
        {
            Element element = FindElement(findLogic);
            try
            {
                element.FindElement(new ObjectMappingDetails(string.Format(".[@{0}]", $"Attribute [{attributeName}] of {findLogic.FriendlyName ?? findLogic.FindLogic}")));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool HasAttribute(string attributeName)
        {
            try
            {
                RootElement.FindElement(new ObjectMappingDetails(string.Format(".[@{0}]", $"Attribute [{attributeName}] of {RootElement.Mapping.FriendlyName ?? RootElement.Mapping.FindLogic}")));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Set the text from an element within the control
        /// </summary>
        /// <param name="findLogic">Find logic reletive to the root element of the control</param>
        /// <param name="friendlyName">Friendly name of the element the text is coming from</param>
        /// <param name="text">text to set</param>
        public virtual void SetText(ObjectMappingDetails mapping, string friendlyName, string text)
        {
            FindElement(mapping).SetText(text);
        }

        /// <summary>
        /// Set the text in the root element of the control
        /// </summary>
        /// <param name="text">text to set</param>
        public virtual void SetText(string Text)
        {
            RootElement.SetText(Text);
        }

        private static Element FindControlRootElement(ControlFindElement FindMechanism, ObjectMappingDetails FindLogic)
        {
            Element returnValue;
            returnValue = FindMechanism(FindLogic);
            if (returnValue == null)
            {
                throw new Exception("Unexected error!  Find element returned null element.  See Debug log.");
            }
            return returnValue;
        }

        private static ControlFindElement FindToUse(SeleniumDriver Driver, ControlBase ParentControl)
        {
            string DebugText = string.Empty;
            ControlFindElement findToUse;
            if (ParentControl == null)
            {
                // We have no parent.  So we call the SeleniumDriver FindElement to search the whole DOM.
                DebugText = "Top Level (IE. DOM) so using driver find";
                findToUse = Driver.FindElement;
            }
            else
            {
                // Parent control is good and not using Custom root element.  So FindElement from the parent's root
                DebugText = "Has parent control so Root element of Parent Control";
                findToUse = ParentControl.RootElement.FindElement;
            }
            Log.LogWriteLine(Log.LogLevels.FrameworkDebug, "Find to use: " + DebugText);
            return findToUse;
        }

        public Element FindElement(ObjectMappingDetails findLogic)
        {
            return RootElement.FindElement(findLogic);
        }


        public bool Exists(ObjectMappingDetails mapping, bool checkIfVisible=false)
        {
            var elements = RootElement.FindAllElements(mapping);
            bool nonVisibleElementFound = false;
            if (elements.Count > 0)
            {
                if (checkIfVisible)
                {
                    foreach (Element element in elements)
                    {
                        if (!element.IsDisplayed)
                        {
                            Log.LogWriteLine(Log.LogLevels.TestInformation,$"Element [{element.Mapping.FriendlyName}] ({element.Mapping.FindLogicUsed}) exists but not visible");
                            nonVisibleElementFound = true;
                        }
                    }
                    return !nonVisibleElementFound;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public bool Exists(ControlBase control, bool checkIfVisible=false)
        {
            return Exists(control.Mapping, checkIfVisible);
        }

        public bool Exists(Element element, bool checkIfVisible=false)
        {
            return Exists(element.Mapping, checkIfVisible);
        }



        public bool WaitUntilStable(int timeoutMilliseconds = 5000)
        {
            return RootElement.WaitForElementPositionStable(TimeSpan.FromMilliseconds(timeoutMilliseconds));
        }

        /// <summary>
        /// Indicates if control's root element is visible.  Will return true even if control is scrolled out
        /// of view or visually obscured by another element
        /// </summary>
        public bool IsVisible
        {
            get
            {
                if (RootElement != null && RootElement.WebElement != null)
                {
                    return RootElement.Visible();
                }
                else
                {
                    throw new Exception(string.Format("Cannot get visibility status of [{0}] as Root Element has not be found!", Mapping.FriendlyName ?? Mapping.FindLogic ?? "find logic null!"));
                }
            }
        }

        private bool IsToken(string PossibleToken)
        {
            // Tokens start and end with & - this is the usual with tools as well....
            return (PossibleToken.StartsWith("&") && PossibleToken.EndsWith("&"));
        }
    }
}

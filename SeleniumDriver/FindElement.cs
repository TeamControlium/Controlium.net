using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TeamControlium.Framework
{
    public partial class SeleniumDriver
    {

        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="ParentName">Human readable name of Root Element</param>
        /// <param name="RootElement">Element to search from.  If null, search from DOM level</param>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="Timeout">Overrides Find timeout (default uses set Element Find timout)</param>
        /// <param name="PollInterval">Overrides Find poll interval (default uses set Element Find polling interval)</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, TimeSpan Timeout, TimeSpan PollInterval)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, false, Timeout, PollInterval,true, out Element))
                throw TryException;
            return Element;
        }
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, TimeSpan Timeout, TimeSpan PollInterval,bool WaitUntilStable)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, false, Timeout, PollInterval,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }
        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="ParentName">Human readable name of Root Element</param>
        /// <param name="RootElement">Element to search from.  If null, search from DOM level</param>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="AllowMultipleMatches">Indicates if multiple matches are allowed. If true the first match will be used.</param>
        /// <param name="Timeout">Overrides Find timeout (default uses set Element Find timout)</param>
        /// <param name="PollInterval">Overrides Find poll interval (default uses set Element Find polling interval)</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, bool AllowMultipleMatches, TimeSpan Timeout, TimeSpan PollInterval)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, AllowMultipleMatches, Timeout, PollInterval,true, out Element))
                throw TryException;
            return Element;
        }
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, bool AllowMultipleMatches, TimeSpan Timeout, TimeSpan PollInterval,bool WaitUntilStable)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, AllowMultipleMatches, Timeout, PollInterval,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }
        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="ParentName">Human readable name of Root Element</param>
        /// <param name="RootElement">Element to search from.  If null, search from DOM level</param>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="Timeout">Overrides Find timeout (default uses set Element Find timout)</param>
        /// <param name="PollInterval">Overrides Find poll interval (default uses set Element Find polling interval)</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, TimeSpan Timeout)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, false, Timeout, null,true, out Element))
                throw TryException;
            return Element;
        }
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, TimeSpan Timeout,bool WaitUntilStable)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, false, Timeout, null, WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }
        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="ParentName">Human readable name of Root Element</param>
        /// <param name="RootElement">Element to search from.  If null, search from DOM level</param>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="AllowMultipleMatches">Indicates if multiple matches are allowed. If true the first match will be used.</param>
        /// <param name="Timeout">Overrides Find timeout (default uses set Element Find timout)</param>
        /// <param name="PollInterval">Overrides Find poll interval (default uses set Element Find polling interval)</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, bool AllowMultipleMatches, TimeSpan Timeout)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, AllowMultipleMatches, Timeout, null,true, out Element))
                throw TryException;
            return Element;
        }

        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, bool AllowMultipleMatches, TimeSpan Timeout,bool WaitUntilStable)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, AllowMultipleMatches, Timeout, null,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }

        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="ParentName">Human readable name of Root Element</param>
        /// <param name="RootElement">Element to search from.  If null, search from DOM level</param>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, false, null, null,true, out Element))
                throw TryException;
            return Element;
        }
        public Element FindElement(Element ParentElement, bool WaitUntilStable, ObjectMappingDetails Mapping)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, false, null, null,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }
        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="ParentName">Human readable name of Root Element</param>
        /// <param name="RootElement">Element to search from.  If null, search from DOM level</param>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="AllowMultipleMatches">Indicates if multiple matches are allowed. If true the first match will be used.</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, bool AllowMultipleMatches)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, AllowMultipleMatches, null, null,true, out Element))
                throw TryException;
            return Element;
        }
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, bool AllowMultipleMatches,bool WaitUntilStable)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, AllowMultipleMatches, null, null,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }
        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="Timeout">Overrides Find timeout (default uses set Element Find timout)</param>
        /// <param name="PollInterval">Overrides Find poll interval (default uses set Element Find polling interval)</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(ObjectMappingDetails Mapping, TimeSpan Timeout, TimeSpan PollInterval)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, false, Timeout, PollInterval,true, out Element))
                throw TryException;
            return Element;
        }
        public Element FindElement(ObjectMappingDetails Mapping, TimeSpan Timeout, TimeSpan PollInterval,bool WaitUntilStable)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, false, Timeout, PollInterval,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }
        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="AllowMultipleMatches">Indicates if multiple matches are allowed. If true the first match will be used.</param>
        /// <param name="Timeout">Overrides Find timeout (default uses set Element Find timout)</param>
        /// <param name="PollInterval">Overrides Find poll interval (default uses set Element Find polling interval)</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(ObjectMappingDetails Mapping, bool AllowMultipleMatches, TimeSpan Timeout, TimeSpan PollInterval)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, AllowMultipleMatches, Timeout, PollInterval,true, out Element))
                throw TryException;
            return Element;
        }
        public Element FindElement(ObjectMappingDetails Mapping, bool AllowMultipleMatches, TimeSpan Timeout, TimeSpan PollInterval,bool WaitUntilStable)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, AllowMultipleMatches, Timeout, PollInterval,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }
        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="Timeout">Overrides Find timeout (default uses set Element Find timout)</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(ObjectMappingDetails Mapping, TimeSpan Timeout)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, false, Timeout, null,true, out Element))
                throw TryException;
            return Element;
        }

        public Element FindElement(ObjectMappingDetails Mapping, TimeSpan Timeout,bool WaitUntilStable)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, false, Timeout, null,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }

        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="AllowMultipleMatches">Indicates if multiple matches are allowed. If true the first match will be used.</param>
        /// <param name="Timeout">Overrides Find timeout (default uses set Element Find timout)</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(ObjectMappingDetails Mapping, bool AllowMultipleMatches, TimeSpan Timeout)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, AllowMultipleMatches, Timeout, null,true, out Element))
                throw TryException;
            return Element;
        }

        public Element FindElement(ObjectMappingDetails Mapping, bool AllowMultipleMatches,bool WaitUntilStable, TimeSpan Timeout)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, AllowMultipleMatches, Timeout, null,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }

        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(ObjectMappingDetails Mapping)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, false, null, null,true, out Element))
                throw TryException;
            return Element;
        }
        public Element FindElement(bool WaitUntilStable, ObjectMappingDetails Mapping)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, false, null, null,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }
        /// <summary>Applies Find Logic to DOM and locates an Element.  Throws exception if find logic does not locate an element or locates more than one.
        /// </summary>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="AllowMultipleMatches">Indicates if multiple matches are allowed. If true the first match will be used.</param>
        /// <returns>Reference to element found</returns>
        public Element FindElement(ObjectMappingDetails Mapping, bool AllowMultipleMatches)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, AllowMultipleMatches, null, null,true, out Element))
                throw TryException;
            return Element;
        }

        public Element FindElement(ObjectMappingDetails Mapping,bool WaitUntilStable, bool AllowMultipleMatches)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, AllowMultipleMatches, null, null,WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }


 

    }
}

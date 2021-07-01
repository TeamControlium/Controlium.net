using System;

namespace TeamControlium.Controlium
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
        public Element FindElement(Element ParentElement, ObjectMappingDetails Mapping, bool AllowMultipleMatches = false, TimeSpan ? Timeout = null, TimeSpan? PollInterval = null, bool WaitUntilStable = true)
        {
            Element Element;
            if (!TryFindElement(ParentElement, Mapping, AllowMultipleMatches, Timeout, PollInterval, WaitUntilStable, out Element))
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
        public Element FindElement(ObjectMappingDetails Mapping, bool AllowMultipleMatches = false, TimeSpan? Timeout = null, TimeSpan? PollInterval = null, bool WaitUntilStable = true)
        {
            Element Element;
            if (!TryFindElement(null, Mapping, AllowMultipleMatches, Timeout, PollInterval, WaitUntilStable, out Element))
                throw TryException;
            return Element;
        }

        // required for delegate match 
        public Element FindElement(ObjectMappingDetails Mapping)
        {
            return FindElement(Mapping, false);
        }
    }
}

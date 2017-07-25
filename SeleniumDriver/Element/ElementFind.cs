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
        /// <summary>Returns a list of all elements matching find logic.
        /// </summary>
        /// <param name="ByFind">Find logic to locate required elements</param>
        /// <param name="FriendlyName">Meaningful name to describe elements</param>
        /// <returns>List of all matching elements (Note.  This should be kept to as few elements as possible to reduce IP traffic!)</returns>
        /// <remarks>All elements are targeted by find logic irrespective of visibility</remarks>
        public List<Element> FindAllElements(ObjectMappingDetails MappingDetails)
        {
            // We can only do this if this element has a Parent (so we can get a reference to the Selenium Driver) and it is bound to an IWebElement so that we can search from it....
            if (!HasAParent)
                throw new Exception("Cannot find WebElements without having a parent (SeleniumDriver or Element)!");
            if (!IsBoundToAWebElement)
                throw new Exception("Cannot find child WebElements without being bound to a Selenium WebElement to search from");
            return seleniumDriver.FindElements(this, MappingDetails);
        }
        /// <summary>Locates an element using find logic applied to the current element returning
        /// a child Element.
        /// </summary>
        /// <param name="ByFind">Find logic to locate element</param>
        /// <param name="FriendlyName">Meaningful name of element that can be used in errors, results and automated documentation (if used)</param>
        /// <param name="FirstMatching">Indicates if multiple matches are allowed. If true the first match will be used.</param>
        /// <remarks>
        /// When searching for matching elements, the visibility of elements is ignored.  This could, therfore, return a hidden element</remarks>
        /// <returns>Reference to child Element</returns>
        /// <exception cref="SeleniumExceptions.FindLogicReturnedMultipleElements">Found multiple elements</exception>
        /// <exception cref="SeleniumExceptions.FindLogicReturnedNoElements">Found no elements</exception>
        public Element FindElement(ObjectMappingDetails MappingDetails, bool FirstMatching)
        {
            // We can only do this if this element has a Parent (so we can get a reference to the Selenium Driver) and it is bound to an IWebElement so that we can search from it....
            if (!HasAParent)
                throw new Exception("Cannot find WebElements without having a parent (SeleniumDriver or Element)!");
            if (!IsBoundToAWebElement)
                throw new Exception("Cannot find child WebElements without being bound to a Selenium WebElement to search from");
            return seleniumDriver.FindElement(this, MappingDetails, FirstMatching, true);
        }
        public Element FindElement(ObjectMappingDetails MappingDetails, bool FirstMatching, bool WaitUntilStable)
        {
            // We can only do this if this element has a Parent (so we can get a reference to the Selenium Driver) and it is bound to an IWebElement so that we can search from it....
            if (!HasAParent)
                throw new Exception("Cannot find WebElements without having a parent (SeleniumDriver or Element)!");
            if (!IsBoundToAWebElement)
                throw new Exception("Cannot find child WebElements without being bound to a Selenium WebElement to search from");
            return seleniumDriver.FindElement(this, MappingDetails, FirstMatching, WaitUntilStable);
        }

        /// <summary>Locates an element using find logic applied to the current element returning
        /// a child Element.
        /// </summary>
        /// <param name="ByFind">Find logic to locate element</param>
        /// <param name="FriendlyName">Meaningful name of element that can be used in errors, results and automated documentation (if used)</param>
        /// <remarks>
        /// When searching for matching elements, the visibility of elements is ignored.  This could, therfore, return a hidden element</remarks>
        /// <returns>Reference to child Element</returns>
        /// <exception cref="SeleniumExceptions.FindLogicReturnedMultipleElements">Found multiple elements</exception>
        /// <exception cref="SeleniumExceptions.FindLogicReturnedNoElements">Found no elements</exception>
        public Element FindElement(ObjectMappingDetails MappingDetails)
        {
            // We can only do this if this element has a Parent (so we can get a reference to the Selenium Driver) and it is bound to an IWebElement so that we can search from it....
            if (!HasAParent)
                throw new Exception("Cannot find WebElements without having a parent (SeleniumDriver or Element)!");
            if (!IsBoundToAWebElement)
                throw new Exception("Cannot find child WebElements without being bound to a Selenium WebElement to search from");
            return seleniumDriver.FindElement(this, true, MappingDetails);
        }

        public Element FindElement(bool WaitUntilStable, ObjectMappingDetails MappingDetails)
        {
            // We can only do this if this element has a Parent (so we can get a reference to the Selenium Driver) and it is bound to an IWebElement so that we can search from it....
            if (!HasAParent)
                throw new Exception("Cannot find WebElements without having a parent (SeleniumDriver or Element)!");
            if (!IsBoundToAWebElement)
                throw new Exception("Cannot find child WebElements without being bound to a Selenium WebElement to search from");
            return seleniumDriver.FindElement(this, WaitUntilStable, MappingDetails);
        }


        /// <summary>Locates an element using find logic applied to the current element returning
        /// a child Element.
        /// </summary>
        /// <param name="ByFind">Find logic to locate element</param>
        /// <param name="FriendlyName">Meaningful name of element that can be used in errors, results and automated documentation (if used)</param>
        /// <param name="FirstMatching">Indicates if multiple matches are allowed. If true the first match will be used.</param>
        /// <remarks>
        /// When searching for matching elements, the visibility of elements is ignored.  This could, therfore, return a hidden element</remarks>
        /// <returns>Reference to child Element</returns>
        /// <exception cref="SeleniumExceptions.FindLogicReturnedMultipleElements">Found multiple elements</exception>
        /// <exception cref="SeleniumExceptions.FindLogicReturnedNoElements">Found no elements</exception>
        public Element FindElement(ObjectMappingDetails Mapping, bool FirstMatching, TimeSpan Timeout, TimeSpan PollInterval)
        {
            // We can only do this if this element has a Parent (so we can get a reference to the Selenium Driver) and it is bound to an IWebElement so that we can search from it....
            if (!HasAParent)
                throw new Exception("Cannot find WebElements without having a parent (SeleniumDriver or Element)!");
            if (!IsBoundToAWebElement)
                throw new Exception("Cannot find child WebElements without being bound to a Selenium WebElement to search from");
            return seleniumDriver.FindElement(this, Mapping, FirstMatching, Timeout, PollInterval, true);
        }
        public Element FindElement(ObjectMappingDetails Mapping, bool FirstMatching, TimeSpan Timeout, TimeSpan PollInterval, bool WaitUntilStable)
        {
            // We can only do this if this element has a Parent (so we can get a reference to the Selenium Driver) and it is bound to an IWebElement so that we can search from it....
            if (!HasAParent)
                throw new Exception("Cannot find WebElements without having a parent (SeleniumDriver or Element)!");
            if (!IsBoundToAWebElement)
                throw new Exception("Cannot find child WebElements without being bound to a Selenium WebElement to search from");
            return seleniumDriver.FindElement(this, Mapping, FirstMatching, Timeout, PollInterval, WaitUntilStable);
        }


        /// <summary>Locates an element using find logic applied to the current element returning
        /// a child Element.
        /// </summary>
        /// <param name="ByFind">Find logic to locate element</param>
        /// <param name="FriendlyName">Meaningful name of element that can be used in errors, results and automated documentation (if used)</param>
        /// <remarks>
        /// When searching for matching elements, the visibility of elements is ignored.  This could, therfore, return a hidden element</remarks>
        /// <returns>Reference to child Element</returns>
        /// <exception cref="SeleniumExceptions.FindLogicReturnedMultipleElements">Found multiple elements</exception>
        /// <exception cref="SeleniumExceptions.FindLogicReturnedNoElements">Found no elements</exception>
        public Element FindElement(ObjectMappingDetails Mapping, TimeSpan Timeout, TimeSpan PollInterval)
        {
            // We can only do this if this element has a Parent (so we can get a reference to the Selenium Driver) and it is bound to an IWebElement so that we can search from it....
            if (!HasAParent)
                throw new Exception("Cannot find WebElements without having a parent (SeleniumDriver or Element)!");
            if (!IsBoundToAWebElement)
                throw new Exception("Cannot find child WebElements without being bound to a Selenium WebElement to search from");
            return seleniumDriver.FindElement(this, Mapping, Timeout, PollInterval, true);
        }

        public Element FindElement(ObjectMappingDetails Mapping, TimeSpan Timeout, TimeSpan PollInterval, bool WaitUntilStable)
        {
            // We can only do this if this element has a Parent (so we can get a reference to the Selenium Driver) and it is bound to an IWebElement so that we can search from it....
            if (!HasAParent)
                throw new Exception("Cannot find WebElements without having a parent (SeleniumDriver or Element)!");
            if (!IsBoundToAWebElement)
                throw new Exception("Cannot find child WebElements without being bound to a Selenium WebElement to search from");
            return seleniumDriver.FindElement(this, Mapping, Timeout, PollInterval, WaitUntilStable);
        }

    }
}

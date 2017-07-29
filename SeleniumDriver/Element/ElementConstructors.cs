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
        /// Create an instance of an Element.
        /// </summary>
        /// <remarks>
        /// Element has no parent set, is not bound to a Selemenium WebElement and has no mapping details.
        /// </remarks>
        public Element()
        {
            WebElement = null;
            ParentOfThisElement = null;
            this.MappingDetails = null;
        }


        /// <summary>
        /// Create an instance of an Element.
        /// </summary>
        /// <remarks>
        /// Element has no parent set, is not bound to a Selemenium WebElement and has no mapping details.
        /// </remarks>
        public Element(ObjectMappingDetails mappingDetails)
        {
            WebElement = null;
            ParentOfThisElement = null;
            MappingDetails = mappingDetails;
        }


        /// <summary>
        /// Create an instance of an Element. Element will be child of the DOM, and will have no parent element.  It has not yet been bound to a Selenium WebElement
        /// </summary>
        /// <remarks>
        /// Element has no parent set, is not bound to a Selemenium WebElement and has no mapping details.
        /// </remarks>
        /// <param name="seleniumDriver">The Selenium Driver instance this element will be located in</param>
        public Element(SeleniumDriver seleniumDriver)
        {
            WebElement = null;
            ParentOfThisElement = seleniumDriver;
            this.MappingDetails = null;
        }


        /// <summary>
        /// Create an instance of an Element. Element will be child of another element.  It has not yet been bound to a Selenium WebElement
        /// </summary>
        /// <remarks>
        /// Element will have a parent element, but is not yet bound to a Selemenium WebElement and has no mapping details.
        /// </remarks>
        /// <param name="seleniumDriver">The Selenium Driver instance this element will be located in</param>
        public Element(Element parentElement)
        {
            WebElement = null;
            ParentOfThisElement = parentElement;
            this.MappingDetails = null;
        }

        /// <summary>
        /// Create an instance of an Element. Element will be child of the DOM, and will have no parent element.  It has not yet been bound to a Selenium WebElement
        /// </summary>
        /// <remarks>
        /// Element has no parent set, is not bound to a Selemenium WebElement but has been passed its Mapping Details (Find Logic).
        /// </remarks>
        /// <param name="seleniumDriver">The Selenium Driver instance this element will be located in</param>
        /// <param name="MappingDetails">Find Logic and friendly name of element when bound to a Selenium WebElement</param>
        public Element(SeleniumDriver seleniumDriver, ObjectMappingDetails MappingDetails)
        {
            WebElement = null;
            ParentOfThisElement = seleniumDriver;
            this.MappingDetails = MappingDetails;
        }


        /// <summary>
        /// Create an instance of an Element. Element will be child of another element.  It has not yet been bound to a Selenium WebElement
        /// </summary>
        /// <remarks>
        /// Element will have a parent element, but is not yet bound to a Selemenium WebElement but has been passed its Mapping Details (Find Logic).
        /// </remarks>
        /// <param name="seleniumDriver">The Selenium Driver instance this element will be located in</param>
        /// <param name="MappingDetails">Find Logic and friendly name of element when bound to a Selenium WebElement</param>
        public Element(Element parentElement, ObjectMappingDetails MappingDetails)
        {
            WebElement = null;
            ParentOfThisElement = parentElement;
            this.MappingDetails = MappingDetails;
        }


        /// <summary>
        /// Instatiates a new Element as the top-level element in the DOM. Usually called from within the SeleniumDriver <see cref="seleniumDriver.FindElement(FindBy,string)"/> method.
        /// </summary>
        /// <param name="SeleniumDriver">Instantiated and fully initialised Driver</param>
        /// <param name="iWebElement">Raw Selenium element for this element</param>
        /// <param name="FriendlyName">Meaningful text description of element used in error reporting and results</param>
        /// <param name="FindLogic">Logic used to locate this element</param>
        /// <remarks>
        /// All Elements in a test can trace their origin back to a root element.  This enables cacheing (to reduce IP wire traffic
        /// when executing remotely) while enabling the automatic Stale Element resolution, simplifying find logic, etc...
        /// <para/><para/>
        /// Therefore, when a new window has been instantiated the first call should be to the SeleniumDriver.FindElement to establish
        /// the root element.
        /// </remarks>
        public Element(SeleniumDriver SeleniumDriver, IWebElement iWebElement, ObjectMappingDetails MappingDetails)
        {
            WebElement = iWebElement;
            ParentOfThisElement = SeleniumDriver;
            this.MappingDetails = MappingDetails;
        }

        /// <summary>
        /// Instatiates a new Element as a child of an existing Element.
        /// </summary>
        /// <param name="Parent">Parent of this elementr</param>
        /// <param name="iWebElement">Raw Selenium element for this element</param>
        /// <param name="FriendlyName">Meaningful text description of element used in error reporting and results</param>
        /// <param name="FindLogic">Logic used to locate this element</param>
        /// <remarks>
        /// All Elements in a test can trace their origin back to a root element.  This enables cacheing (to reduce IP wire traffic
        /// when executing remotely) while enabling the automatic Stale Element resolution, simplifying find logic, etc...
        /// <para/><para/>
        /// Therefore, when a new window has been instantiated the first call should be to the SeleniumDriver.FindElement to establish
        /// the root element.
        /// </remarks>
        public Element(Element Parent, IWebElement iWebElement, ObjectMappingDetails MappingDetails)
        {
            WebElement = iWebElement;
            ParentOfThisElement = Parent;
            this.MappingDetails = MappingDetails;
        }
    }
}

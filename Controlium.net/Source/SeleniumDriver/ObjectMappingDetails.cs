using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using TeamControlium.Utilities;

namespace TeamControlium.Controlium
{
    public class ObjectMappingDetails
    {
        public enum ByType { Id, Class, Css, LinkText, Name, Partial, Tag, XPath, Unknown };

        /// <summary>
        /// Find logic type of mapped object
        /// </summary>
        /// <example><code language="C#" title="Using XPath"> mappedObject.FindLogic = ".\div[@class='MyClass'];"</code>
        /// </example>
        public ByType FindType { get; private set; }

        /// <summary>
        /// Find logic of mapped object
        /// </summary>
        /// <example><code language="C#" title="Using XPath"> mappedObject.FindLogic = ".\div[@class='MyClass'];"</code>
        /// </example>
        public string FindLogic { get; set; }

        /// <summary>
        /// Actual Find logic of mapped object
        /// </summary>
        /// <example><code language="C#" title="Using XPath"> mappedObject.FindLogic = ".\div[@class='MyClass'];"</code>
        /// </example>
        /// <remarks>Find logic used may not be actual Find Logic defined. This may occure when multiple
        /// elements are found and an indexed one is used.  The actual find logic can be used to locate the indexed
        /// element if wanted to traverse up the find tree.
        /// </remarks>
        public string FindLogicUsed { get; set; }

        /// <summary>
        /// Friendly Name of object defined by find logic
        /// </summary>
        /// <remarks>
        /// Objects are defined by their friendly names in object-related errors reported to the user rather than
        /// only by their find logic.
        /// </remarks>
        public string FriendlyName { get; set; }

        public By SeleniumBy { get; private set; }

        /// <summary>
        /// If Findlogic has parameters (ResolveParameters method called with parameters resolved) store original FindLogic here for future use).
        /// </summary>
        private string FindLogicWithParameters { get; set; }

        /// <summary>
        /// If FriendlyName has parameters (ResolveParameters method called with parameters resolved) store original FriendlyName here for future use).
        /// </summary>
        private string FriendlyNameWithParameters { get; set; }

        public ObjectMappingDetails Copy()
        {
            return (ObjectMappingDetails)MemberwiseClone();
        }

        /// <summary>
        /// Defines an unspecified object.  Type and Name are empty strings and Logic is null
        /// </summary>
        public ObjectMappingDetails()
        {
            FindType = ByType.Unknown;
            FindLogic = null;
            FindLogicWithParameters = null;
            FriendlyName = string.Empty;
            FriendlyNameWithParameters = null;
        }

        /// <summary>
        /// Defines an object using string-defined logic (default xpath) and with a friendly name of the Find logic
        /// </summary>
        /// <example>
        /// <code language="C#">
        /// // myObject points to HTML link with text View PDF Documents
        /// MappedObject myObject = new MappedObject("linktext=View PDF Documents");
        /// </code></example>
        /// <param name="FindLogic">Logic to use.  If no type defaults to XPath</param>
        public ObjectMappingDetails(string FindLogic)
        {
            this.FindLogic = FindLogic;
            FindLogicWithParameters = null;
            FriendlyName = FindLogic;
            FriendlyNameWithParameters = null;
            SeleniumBy = ProcessFindLogic(FindLogic);
        }

        /// <summary>
        /// Defines an object using string-defined logic (default xpath) with passed Freindly name
        /// </summary>
        /// <example>
        /// <code language="C#">
        /// // myObject points to HTML link with text View PDF Documents
        /// MappedObject myObject = new MappedObject("linktext=View PDF Documents","PDF Document link");
        /// </code></example>
        /// <param name="FindLogic">Logic to use.  If no type defaults to XPath</param>
        /// <param name="FriendlyName">Text to use in logs and error reporting</param>
        public ObjectMappingDetails(string FindLogic, string FriendlyName)
        {
            this.FindLogic = FindLogic;
            FindLogicWithParameters = null;
            this.FriendlyName = FriendlyName;
            FriendlyNameWithParameters = null;
            SeleniumBy = ProcessFindLogic(FindLogic);
        }

        /// <summary>
        /// Resolves parameters if used
        /// </summary>
        /// <param name="args">arguments for resolution</param>
        /// <returns>Instance</returns>
        public ObjectMappingDetails ResolveParameters(params string[] args)
        {
            string newFindLogic = null;
            string newFriendlyName = null;
            try
            {
                newFindLogic = string.Format(FindLogicWithParameters ?? FindLogic, args);
            }
            catch (Exception ex)
            {
                Log.LogWriteLine(Log.LogLevels.TestInformation, "Error resolving parameters in find logic [{0}]: {1}", FindLogic ?? "<Null!>", ex);
            }

            try
            {
                newFriendlyName = string.Format(FriendlyNameWithParameters ?? FriendlyName, args);
            }
            catch (Exception ex)
            {
                Log.LogWriteLine(Log.LogLevels.TestInformation, "Error resolving parameters in find logic [{0}]: {1}", FriendlyName ?? "<Null!>", ex);
            }
            if (newFindLogic != null)
            {
                if (FindLogicWithParameters == null)
                    FindLogicWithParameters = FindLogic;
                FindLogic = newFindLogic;
                SeleniumBy = ProcessFindLogic(FindLogic);
            }
            if (newFriendlyName != null)
            {
                if (FriendlyNameWithParameters == null)
                    FriendlyNameWithParameters = FriendlyName;
                FriendlyName = newFriendlyName;
            }
            SeleniumBy = ProcessFindLogic(FindLogic);
            return this;
        }

        private By ProcessFindLogic(string property)
        {
            By returnValue;
            KeyValuePair<string, string> ByValue;

            if (property.Contains("="))
                ByValue = new KeyValuePair<string, string>(property.Split('=')[0].Trim().ToLower(), property.Substring(property.IndexOf('=') + 1));
            else
                ByValue = new KeyValuePair<string, string>("xpath", property);

            switch (ByValue.Key)
            {
                case "id": returnValue = By.Id(ByValue.Value); FindType = ByType.Id; break;
                case "class": returnValue = By.ClassName(ByValue.Value); FindType = ByType.Class; break;
                case "css": returnValue = By.CssSelector(ByValue.Value); FindType = ByType.Css; break;
                case "linktext": returnValue = By.LinkText(ByValue.Value); FindType = ByType.LinkText; break;
                case "name": returnValue = By.Name(ByValue.Value); FindType = ByType.Name; break;
                case "partial": returnValue = By.PartialLinkText(ByValue.Value); FindType = ByType.Partial; break;
                case "tag": returnValue = By.TagName(ByValue.Value); FindType = ByType.Tag; break;
                case "xpath": returnValue = By.XPath(ByValue.Value); FindType = ByType.XPath; break;
                default: returnValue = By.XPath(property); FindType = ByType.XPath; break;
            }
            return returnValue;
        }
    }
}
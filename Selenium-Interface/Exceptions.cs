using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamControlium.Framework.SeleniumExceptions
{
    /// <summary>Thrown when the named device does not match a device in the test-automation library</summary>
    public class UnsupportedDevice : Exception
    {
        static private string FormatMessage(string Device)
        {
            string Message = string.Format("Device unsupported within TeamControlium Test Automation - [{0}]", Device);
            return Message;
        }

        /// <summary>
        /// Thrown when the named device does not match a device in the test-automation library
        /// </summary>
        /// <param name="Device">Device string</param>
        public UnsupportedDevice(string Device)
            : base(UnsupportedDevice.FormatMessage(Device))
        {
        }

    }

    /// <summary>Thrown when the Selenium server folder does not exist or cannot be accessed</summary>
    public class SeleniumFolderError : Exception
    {
        static private string FormatMessage(string Path)
        {
            string CurrentFolder = Directory.GetCurrentDirectory();
            string Message = string.Format("Test Automation cannot locate Selenium Server folder or error accessing - [{0}] in [{1}]", Path, CurrentFolder);
            return Message;
        }

        /// <summary>
        /// Thrown when the Selenium server folder does not exist or cannot be accessed
        /// </summary>
        /// <param name="Path">Path string</param>
        public SeleniumFolderError(string Path)
            : base(SeleniumFolderError.FormatMessage(Path))
        {
        }
        /// <summary>
        /// Thrown when the Selenium server folder does not exist or cannot be accessed
        /// </summary>
        /// <param name="Path">Path string</param>
        /// <param name="InnerException">Exception thrown when trying to instantiate Selenium</param>
        public SeleniumFolderError(string Path, Exception InnerException)
            : base(SeleniumFolderError.FormatMessage(Path), InnerException)
        {
        }
    }

    /// <summary>Thrown when the Selenium WebDriver failed to initialise</summary>
    public class SeleniumWebDriverInitError : Exception
    {
        static private string LocalFormatMessage(string Path, string Browser)
        {
            string Message = string.Format("Test Automation cannot start WebDriver - Browser [{1}] in [{0}]", Path, Browser);
            return Message;
        }
        /// <summary>
        /// Thrown when the Selenium WebDriver failed to initialise
        /// </summary>
        /// <param name="Path">Path string</param>
        /// <param name="Browser">Browser Webdriver has been requesred for</param>
        public SeleniumWebDriverInitError(string Path, string Browser)
            : base(SeleniumWebDriverInitError.LocalFormatMessage(Path, Browser))
        {
        }
        /// <summary>
        /// Thrown when the Selenium WebDriver failed to initialise
        /// </summary>
        /// <param name="Path">Path string to local Web Server</param>
        /// <param name="Browser">Browser Webdriver has been requesred for</param>
        /// <param name="InnerException">Exception thrown by Selenium</param>
        public SeleniumWebDriverInitError(string Path, string Browser, Exception InnerException)
            : base(SeleniumWebDriverInitError.LocalFormatMessage(Path, Browser), InnerException)
        {
        }

    }

    /// <summary>Thrown when the browser does not match a browser in the test-automation library</summary>
    public class UnsupportedBrowser : Exception
    {
        static private string FormatMessage(string Browser)
        {
            string Message = string.Format("Browser unsupported within Test Automation - [{0}]", Browser);
            return Message;
        }

        /// <summary>
        /// Thrown when the browser does not match a browser in the test-automation library
        /// </summary>
        /// <param name="Browser">Browser string</param>
        public UnsupportedBrowser(string Browser)
            : base(UnsupportedBrowser.FormatMessage(Browser))
        {
        }

    }

    /// <summary>URI Is not in a valid format</summary>
    public class InvalidHostURI : Exception
    {
        static private string FormatMessage(string uri)
        {
            string Message = string.Format("Test Automation cannot parse the URI - [{0}]", uri);
            return Message;
        }

        /// <summary>Thrown when the Selenium server folder does not exist or cannot be accessed</summary>
        /// <param name="URI">URI Requested</param>
        public InvalidHostURI(string URI)
            : base(InvalidHostURI.FormatMessage(URI))
        {
        }
    }

    /// <summary>Invalid settings</summary>
    public class InvalidSettings : Exception
    {
        static private string FormatMessage(string settings)
        {
            string Message = string.Format("Invalid settings - [{0}]", settings);
            return Message;
        }

        /// <summary>
        /// Invalid settings
        /// </summary>
        /// <param name="settings">Settings string</param>
        /// <param name="ex">Exception thrown by tool when rejecting the settings</param>
        public InvalidSettings(string settings, Exception ex)
            : base(InvalidSettings.FormatMessage(settings), ex)
        {
        }
    }

    /// <summary>Tool has no reference to Selelium WEeb Driver and it neeeds it.</summary>
    public class ToolDoesNotReferenceWebDriver : Exception
    {
        /// <summary>
        /// Thrown when the Selenium server folder does not exist or cannot be accessed
        /// </summary>
        public ToolDoesNotReferenceWebDriver()
            : base("Tool has not had SeleniumDriver set - still null")
        {
        }
    }

    /// <summary>Tool has rejected one or more run options</summary>
    public class ToolDoesNotSupportRunOptions : Exception
    {
        /// <summary>
        /// Tool has rejected one or more run options
        /// </summary>
        /// <param name="Message">Message to show string</param>
        /// <param name="ex">Exception thrown by tool</param>
        public ToolDoesNotSupportRunOptions(string Message, Exception ex)
            : base(Message, ex)
        {
        }
    }

    /// <summary>Requested run option rejected. Category or Name with that category does not exist</summary>
    public class RunOptionOrCategoryDoesNotExist : Exception
    {
        static private string FormatMessage(string Category, string Name)
        {
            if (string.IsNullOrEmpty(Category))
                return string.Format("Option name [{0}] does not exist (no category)", Name);
            else
                return string.Format("Option name [{0}] in category [{1}] does not exist", Name, Category);
        }
        static private string FormatMessage(string Category, string Name, string Message)
        {
            if (string.IsNullOrEmpty(Category))
                return string.Format("Option name [{0}] does not exist (no category)", Name);
            else
                return string.Format("{0} Option name [{1}] in category [{2}] does not exist", Message, Name, Category);
        }

        /// <summary>
        /// Requested run option rejected. Category or Name with that category does not exist
        /// </summary>
        /// <param name="Name">Name of option being requested</param>
        public RunOptionOrCategoryDoesNotExist(string Name)
            : base(RunOptionOrCategoryDoesNotExist.FormatMessage(null, Name))
        {
        }
        /// <summary>
        /// Requested run option rejected. Category or Name with that category does not exist
        /// </summary>
        /// <param name="Category">Name of Category for option being requested</param>
        /// <param name="Name">Name of option being requested</param>
        public RunOptionOrCategoryDoesNotExist(string Category, string Name)
            : base(RunOptionOrCategoryDoesNotExist.FormatMessage(Category, Name))
        {
        }
        /// <summary>
        /// Requested run option rejected. Category or Name with that category does not exist
        /// </summary>
        /// <param name="Message">Message preceding standard text</param>
        /// <param name="Category">Name of Category for option being requested</param>
        /// <param name="Name">Name of option being requested</param>
        public RunOptionOrCategoryDoesNotExist(string Category, string Name, string Message)
            : base(RunOptionOrCategoryDoesNotExist.FormatMessage(Category, Name, Message))
        {
        }
    }

    /// <summary>Tool has thrown an unknown and/or unexpected error</summary>
    public class ToolError : Exception
    {
        /// <summary>
        /// Thrown when the Selenium server folder does not exist or cannot be accessed
        /// </summary>
        /// <param name="InnerException">Exception thrown by tool</param>
        public ToolError(Exception InnerException)
            : base("Unexpected Tool error.", InnerException)
        {
        }
    }

    /// <summary>Cannot perform a logging activity as no tool logging is active (maybe not even supported)</summary>
    public class NoToolLoggingActive : Exception
    {
        /// <summary>
        /// Cannot perform a logging activity as no tool logging is active (maybe not even supported)
        /// </summary>
        /// <param name="InnerException">Exception thrown by the tool</param>
        public NoToolLoggingActive(Exception InnerException)
            : base("Tool does not support logging or logging not active.", InnerException)
        {
        }
    }

    /// <summary>Test has tried to perform a Data Driven Testing activity but it is unsupported by the tool</summary>
    public class DataDrivenTestingUnsupportedOrError : Exception
    {
        /// <summary>
        /// Test has tried to perform a Data Driven Testing activity but it is unsupported by the tool
        /// </summary>
        /// <param name="InnerException">Exception thrown by tool</param>
        public DataDrivenTestingUnsupportedOrError(Exception InnerException)
            : base("Tool does not support Data Driven Testing or error thrown.", InnerException)
        {
        }
    }

    /// <summary>Error while attempting to save local test data</summary>
    public class ToolDataSaveError : Exception
    {
        /// <summary>
        /// Error while attempting to save local test data
        /// </summary>
        /// <param name="Details">Details of error</param>
        /// <param name="InnerException">Exception thrown by tool</param>
        public ToolDataSaveError(string Details, Exception InnerException)
            : base(Details, InnerException)
        {
        }
    }

    /// <summary>Element trying to be clicked cannot receive the clicks as another element would get them</summary>
    public class ElementCannotBeClicked : Exception
    {
        /// <summary>
        /// Element trying to be clicked cannot receive the clicks as another element would get them
        /// </summary>
        /// <param name="Details">Details of error</param>
        /// <param name="InnerException">Exception thrown by tool</param>
        public ElementCannotBeClicked(string Details, Exception InnerException)
            : base(Details, InnerException)
        {
        }
    }

    /// <summary>Multiple elements could found using given find logic</summary>
    public class FindLogicReturnedMultipleElements : Exception
    {
        static private string FormatMessage(string ParentName, string RawFindLogic, string FriendlyName, int NumberOfElements)
        {
            return string.Format("Multiple Elements Found Exception: Parent [{0}], Find Logic [{1}] returned {2} elements! Required Element = [{3}]", ParentName, RawFindLogic, NumberOfElements.ToString(), FriendlyName);
        }

        /// <summary>Multiple Elements returned with find logic</summary>
        /// <param name="ParentName">Parent element</param>
        /// <param name="RawFindLogic">Find logic applied to parent</param>
        /// <param name="FriendlyName">Name of element we wanted</param>
        /// <param name="NumberOfElements">Number of elements returned</param>
        public FindLogicReturnedMultipleElements(string ParentName, string RawFindLogic, string FriendlyName, int NumberOfElements)
            : base(FindLogicReturnedMultipleElements.FormatMessage(ParentName, RawFindLogic, FriendlyName, NumberOfElements))
        {
        }
    }

    /// <summary>No elements could be found using given find logic</summary>
    public class FindLogicReturnedNoElements : Exception
    {
        static private string FormatMessage(string ParentName, string RawFindLogic, string FriendlyName, string TimeoutInSeconds, string PollIntervalInMinilliseconds)
        {
            return string.Format("No Elements Found Exception: Parent [{0}], Find Logic [{1}], Required Element = [{2}], Timeout = [{3}Secs], Polling every [{4}ms]", ParentName, RawFindLogic, FriendlyName, TimeoutInSeconds, PollIntervalInMinilliseconds);
        }


        /// <summary>No elements could be found using given find logic</summary>
        /// <param name="ParentName">>Parent element</param>
        /// <param name="RawFindLogic">Find logic applied to parent</param>
        /// <param name="FriendlyName">Name of element we wanted</param>
        public FindLogicReturnedNoElements(string ParentName, string RawFindLogic, string FriendlyName, string TimeoutInSeconds, string PollIntervalInMinilliseconds)
            : base(FindLogicReturnedNoElements.FormatMessage(ParentName, RawFindLogic, FriendlyName, TimeoutInSeconds, PollIntervalInMinilliseconds))
        {
        }
    }

    /// <summary>Attribute returned null</summary>
    public class AttributeReturnedNull : Exception
    {
        static private string FormatMessage(string ElementName, string Attribute)
        {
            return string.Format("Attribute [{0}] from element [{1}] returned null.", Attribute, ElementName);
        }


        /// <summary>Attribute returned null</summary>
        /// <param name="ElementName">Element whose attribute was being read</param>
        /// <param name="Attribute">Attribute being read</param>
        public AttributeReturnedNull(string ElementName, string Attribute)
            : base(AttributeReturnedNull.FormatMessage(ElementName, Attribute))
        {
        }
    }

    /// <summary>Trying to set text caused Selenium to throw an exception</summary>
    public class UnableToSetText : Exception
    {
        static private string FormatMessage(string ElementName, string Text)
        {
            return string.Format("Setting text on element [{0}] (Text [{1}]) caused an exception from Selenium! See inner exception", ElementName, Text);
        }


        /// <summaryTrying to set text caused Selenium to throw an exception</summary>
        /// <param name="ElementName">Element text was being entered into</param>
        /// <param name="Text">Text being entered</param>
        /// <param name="ex">Inner Exception</param>
        public UnableToSetText(string ElementName, string Text, Exception ex)
            : base(UnableToSetText.FormatMessage(ElementName, Text), ex)
        {
        }
    }

    /// <summary>Timed out waiting for an element</summary>
    public class TimeoutWaitingForElement : Exception
    {
        static private string FormatMessage(string ParentElementName, string WaitedForElementName, string FindLogic, string RequiredVisibility, string ElapsedTime)
        {
            return string.Format("Waiting for element [{0} (FindLogic:{1})] (Descendant of [{2}]) caused an exception. Wanted to be {3} (waited {4} Seconds)", WaitedForElementName, FindLogic, ParentElementName, RequiredVisibility, ElapsedTime);
        }

        /// <summary>
        /// Waiting for an element caused an exception!
        /// </summary>
        /// <param name="ParentElementName">Name of root element being looked under</param>
        /// <param name="ElementName">Name of element being waited on</param>
        /// <param name="FindLogic">Find logic used to identify element being waited on</param>
        /// <param name="RequiredVisibility">What state we were waiting for</param>
        /// <param name="ElapsedTime">How long (in Seconds) we waited</param>
        public TimeoutWaitingForElement(string ParentElementName, string ElementName, string FindLogic, string RequiredVisibility, string ElapsedTime)
            : base(TimeoutWaitingForElement.FormatMessage(ParentElementName, ElementName, FindLogic, RequiredVisibility, ElapsedTime))
        {
        }
    }

    /// <summary>Timeout waiting for a wait element</summary>
    public class WaitTimeout : Exception
    {
        static private string FormatMessage(string ParentElementName, string WaitedForElementName, string FindLogic, string RequiredVisibility, string ElapsedTime, string PollIntervalInMilliseconds)
        {
            return string.Format("Wait timed out after {0} Seconds.  Waiting for element {1} ({2}){3} to be {4} (Poll Interval = {5}mS", ElapsedTime, WaitedForElementName, FindLogic, string.IsNullOrEmpty(ParentElementName) ? string.Empty : " under " + ParentElementName, RequiredVisibility, PollIntervalInMilliseconds);
        }

        /// <summary>
        /// Waiting for an element caused an exception!
        /// </summary>
        /// <param name="ParentElementName">Name of root element being looked under</param>
        /// <param name="ElementName">Name of element being waited on</param>
        /// <param name="FindLogic">Find logic used to identify element being waited on</param>
        /// <param name="RequiredVisibility">What state we were waiting for</param>
        /// <param name="ElapsedTime">How long (in Seconds) we waited</param>
        public WaitTimeout(string ParentElementName, string ElementName, string FindLogic, string RequiredVisibility, string ElapsedTimeInSeconds, string PollIntervalInMilliseconds)
            : base(WaitTimeout.FormatMessage(ParentElementName, ElementName, FindLogic, RequiredVisibility, ElapsedTimeInSeconds, PollIntervalInMilliseconds))
        {
        }
    }

    /// <summary>Timed out waiting for an element</summary>
    public class BasestateError : Exception
    {
        static private string FormatMessage(string Message)
        {
            return string.Format("Unable to perform test.  Basestate operation threw error: {0}.", Message);
        }

        /// <summary>
        /// Waiting for an element caused an exception!
        /// </summary>
        /// <param name="ParentElementName">Name of root element being looked under</param>
        /// <param name="ElementName">Name of element being waited on</param>
        /// <param name="FindLogic">Find logic used to identify element being waited on</param>
        /// <param name="RequiredVisibility">What state we were waiting for</param>
        /// <param name="ElapsedTime">How long (in Seconds) we waited</param>
        public BasestateError(string Message, Exception ex)
            : base(BasestateError.FormatMessage(Message), ex)
        {
        }
    }


}

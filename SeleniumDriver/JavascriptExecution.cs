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
        /// <summary>Injects and executes Javascript in the currently active Selenium browser with no exception thrown. Javascript has return object which is passed back as a string</summary>
        /// <param name="script">Javascript that will be injected into the DOM and executed. Script must have a return instruction</param>
        /// <param name="result">Data passed back from the Javascript when executed. Empty string if an exception occured</param>
        /// <param name="args">Any arguments passed in to the Javascript</param>
        /// <seealso cref="TryException">Property referencing exception if thrown</seealso>
        /// <seealso cref="ExecuteJavaScriptReturningString(string, object[])"/>
        /// <returns>True if no Selenium exception thrown, or false if exception thrown</returns>
        /// <remarks>
        /// Selenium and Appium servers may implement how Javascript execution is perfomed and data passed back.  Some implementations may not
        /// support it directly, requiring a local implementation of the IJavaScriptExecutor interface. This method guarantees a unified method
        /// of executing Javascript in automated tests.
        /// <para/><para/>
        /// If, at any stage, an Exception is thrown - either in Selenium or bubbled through from the JavaScript execution - it is logged in
        /// TryException and a false is returned</remarks>
        /// <example>Clear local storage and verify it is empty:
        /// <code lang="C#">
        /// if (SeleniumDriver.TryExecuteJavaScript("window.localStorage.clear(); return window.localStorage.length;",out NumItemsLeft))
        /// {
        ///   if (int.Parse(NumItemsLeft)>0)
        ///   {
        ///     // Oh dear, we have not cleared the store!
        ///   }
        /// }
        /// else
        /// {
        ///   // We had a problem executing the script
        ///   SSDebug.WriteLine("MyTest","Test","JavaScript Error - {0}",SeleniumDriver.TryException.Message);
        /// }</code></example>
        public bool TryExecuteJavaScript(string script, out string result, params object[] args)
        {
            try
            {
                result = (string)((IJavaScriptExecutor)webDriver).ExecuteScript(script, args).ToString();
                return true;
            }
            catch (Exception ex)
            {
                string exceptionString = string.Empty;
                foreach (object arg in args)
                {
                    if (string.IsNullOrEmpty(exceptionString))
                        exceptionString = string.Format("TryExecuteJavaScript(\"{0}\")-(Args: \"{1}\"", script, arg.GetType().Name);
                    else
                        exceptionString = string.Format("{0}, \"{1}\"", exceptionString, arg.GetType().Name);
                }
                if (!string.IsNullOrEmpty(exceptionString))
                    exceptionString = string.Format("{0})", exceptionString);
                else
                    exceptionString = string.Format("TryExecuteJavaScript(\"{0}\")", script);
                TryException = ex;
                result = string.Empty;
                return false;
            }
        }

        /// <summary>Injects and executes Javascript in the currently active Selenium browser with no exception thrown. Javascript returns Element object</summary>
        /// <param name="script">Javascript that will be injected into the DOM and executed. Script must have a return instruction</param>
        /// <param name="result">Element returned by Javascript</param>
        /// <param name="args">Any arguments passed in to the Javascript</param>
        /// <seealso cref="TryException">Property referencing exception if thrown</seealso>
        /// <seealso cref="ExecuteJavaScriptReturningWebElement(string, object[])"/>
        /// <returns>True if no Selenium exception thrown, or false if exception thrown</returns>
        /// <remarks>
        /// Selenium and Appium servers may implement how Javascript execution is perfomed and data passed back.  Some implementations may not
        /// support it directly, requiring a local implementation of the IJavaScriptExecutor interface. This method guarantees a unified method
        /// of executing Javascript in automated tests.
        /// <para/><para/>
        /// If, at any stage, an Exception is thrown - either in Selenium or bubbled through from the JavaScript execution - it is logged in
        /// TryException and a false is returned</remarks>
        /// <example>Clear local storage and verify it is empty:
        /// <code lang="C#">
        /// if (SeleniumDriver.TryExecuteJavaScript("window.localStorage.clear(); return window.localStorage.length;",out NumItemsLeft))
        /// {
        ///   if (int.Parse(NumItemsLeft)>0)
        ///   {
        ///     // Oh dear, we have not cleared the store!
        ///   }
        /// }
        /// else
        /// {
        ///   // We had a problem executing the script
        ///   SSDebug.WriteLine("MyTest","Test","JavaScript Error - {0}",SeleniumDriver.TryException.Message);
        /// }</code></example>
        public bool TryExecuteJavaScript(string script, out IWebElement result, params object[] args)
        {
            try
            {
                result = (IWebElement)((IJavaScriptExecutor)webDriver).ExecuteScript(script, args);
                return true;
            }
            catch (Exception ex)
            {
                string exceptionString = string.Empty;
                foreach (object arg in args)
                {
                    if (string.IsNullOrEmpty(exceptionString))
                        exceptionString = string.Format("TryExecuteJavaScript(\"{0}\")-(Args: \"{1}\"", script, (string)arg);
                    else
                        exceptionString = string.Format("{0}, \"{1}\"", exceptionString, (string)arg);
                }
                if (!string.IsNullOrEmpty(exceptionString))
                    exceptionString = string.Format("{0})", exceptionString);
                else
                    exceptionString = string.Format("TryExecuteJavaScript(\"{0}\")", script);

                TryException = ex;
                result = null;
                return false;
            }
        }

        public bool TryExecuteJavaScript<T>(string script, out T result, params object[] args)
        {
            try
            {
                result = (T)((IJavaScriptExecutor)webDriver).ExecuteScript(script, args);
                return true;
            }
            catch (Exception ex)
            {
                string exceptionString = string.Empty;
                foreach (object arg in args)
                {
                    if (string.IsNullOrEmpty(exceptionString))
                        exceptionString = string.Format("TryExecuteJavaScript(\"{0}\")-(Args: \"{1}\"", script, (string)arg);
                    else
                        exceptionString = string.Format("{0}, \"{1}\"", exceptionString, (string)arg);
                }
                if (!string.IsNullOrEmpty(exceptionString))
                    exceptionString = string.Format("{0})", exceptionString);
                else
                    exceptionString = string.Format("TryExecuteJavaScript(\"{0}\")", script);

                TryException = ex;
                result = default(T);
                return false;
            }
        }



        /// <summary>Injects and executes Javascript in the currently active Selenium browser with no exception thrown. Javascript has no return object.</summary>
        /// <param name="script">Javascript that will be injected into the DOM and executed. Script must have a return instruction</param>
        /// <param name="args">Any arguments passed in to the Javascript</param>
        /// <seealso cref="TryException">Property referencing exception if thrown</seealso>
        /// <seealso cref="ExecuteJavaScriptNoReturnData(string, object[])"/>
        /// <returns>True if no Selenium exception thrown, or false if exception thrown</returns>
        /// <remarks>
        /// Selenium and Appium servers may implement how Javascript execution is perfomed and data passed back.  Some implementations may not
        /// support it directly, requiring a local implementation of the IJavaScriptExecutor interface. This method guarantees a unified method
        /// of executing Javascript in automated tests.
        /// <para/><para/>
        /// If, at any stage, an Exception is thrown - either in Selenium or bubbled through from the JavaScript execution - it is logged in
        /// TryException and a false is returned</remarks>
        /// <example>Clear specific item (TelephoneNum) from local storage:
        /// <code lang="C#">
        /// if (SeleniumDriver.TryExecuteJavaScript("window.localStorage.removeItem(\"arguments[0]\");","TelephoneNum"))
        /// {
        ///   // We had a problem executing the script
        ///   SSDebug.WriteLine("MyTest","Test","JavaScript Error - {0}",SeleniumDriver.TryException.Message);
        /// }</code></example>
        public bool TryExecuteJavaScriptNoReturnData(string script, params object[] args)
        {
            try
            {
                ((IJavaScriptExecutor)webDriver).ExecuteScript(script, args);
                return true;
            }
            catch (Exception ex)
            {
                string exceptionString = string.Empty;
                foreach (object arg in args)
                {
                    if (string.IsNullOrEmpty(exceptionString))
                        exceptionString = string.Format("TryExecuteJavaScript(\"{0}\")-(Args: \"{1}\"", script, (string)arg);
                    else
                        exceptionString = string.Format("{0}, \"{1}\"", exceptionString, (string)arg);
                }
                if (!string.IsNullOrEmpty(exceptionString))
                    exceptionString = string.Format("{0})", exceptionString);
                else
                    exceptionString = string.Format("TryExecuteJavaScript(\"{0}\")", script);

                TryException = ex;
                return false;
            }
        }

        /// <summary>Injects and executes Javascript in the currently active Selenium browser. If Selenium throws an error, test is aborted.</summary>
        /// <param name="script">Javascript that will be injected into the DOM and executed. Script must have a return instruction</param>
        /// <param name="args">Any arguments passed in to the Javascript</param>
        /// <seealso cref="TryExecuteJavaScript(string, out string,object[])"/>
        /// <returns>Data passed back from the Javascript when executed.</returns>
        /// <remarks>
        /// Selenium and Appium servers may implement how Javascript execution is perfomed and data passed back.  Some implementations may not
        /// support it directly, requiring a local implementation of the IJavaScriptExecutor interface. This method guarantees a unified method
        /// of executing Javascript in automated tests.
        /// <para/><para/>
        /// It is the responsibility of the tool (or wrapper) to gracefully handle the abort and throw an exception.  The test should have the steps wrapped to enable catching
        /// the abort exception to ensure graceful closure of the test.</remarks>
        /// <example>Clear local storage and verify it is empty:
        /// <code lang="C#">
        /// if (SeleniumDriver.ExecuteJavaScript("window.localStorage.clear(); return window.localStorage.length;")>0)
        /// {
        ///     // Oh dear, we have not cleared the store!
        /// }
        /// </code></example>

        public string ExecuteJavaScriptReturningString(string script, params object[] args)
        {
            string returnString = null;
            if (!TryExecuteJavaScript(script, out returnString, args))
            {
                throw new Exception(TryException.Message, TryException);
            }
            return returnString;
        }
        /// <summary>Injects and executes Javascript in the currently active Selenium browser. If Selenium throws an error, test is aborted.</summary>
        /// <param name="script">Javascript that will be injected into the DOM and executed. Script must have a return instruction</param>
        /// <param name="args">Any arguments passed in to the Javascript</param>
        /// <seealso cref="TryExecuteJavaScript(string, out string,object[])"/>
        /// <returns>Element returned by Javascript</returns>
        /// <remarks>
        /// Selenium and Appium servers may implement how Javascript execution is perfomed and data passed back.  Some implementations may not
        /// support it directly, requiring a local implementation of the IJavaScriptExecutor interface. This method guarantees a unified method
        /// of executing Javascript in automated tests.
        /// <para/><para/>
        /// It is the responsibility of the tool (or wrapper) to gracefully handle the abort and throw an exception.  The test should have the steps wrapped to enable catching
        /// the abort exception to ensure graceful closure of the test.</remarks>
        /// <example>Clear local storage and verify it is empty:
        /// <code lang="C#">
        /// if (SeleniumDriver.ExecuteJavaScript("window.localStorage.clear(); return window.localStorage.length;")>0)
        /// {
        ///     // Oh dear, we have not cleared the store!
        /// }
        /// </code></example>
        public IWebElement ExecuteJavaScriptReturningWebElement(string script, params object[] args)
        {
            IWebElement returnElement = null;
            if (!TryExecuteJavaScript(script, out returnElement, args))
            {
                string scriptAndArgs = string.Empty;
                foreach (object arg in args)
                {
                    if (string.IsNullOrEmpty(scriptAndArgs))
                        scriptAndArgs = string.Format("\"{0}\" -(Args: \"{1}\"", script, (string)arg);
                    else
                        scriptAndArgs = string.Format("{0}, \"{1}\"", scriptAndArgs, (string)arg);
                }
                if (!string.IsNullOrEmpty(scriptAndArgs))
                    scriptAndArgs = string.Format("{0})", scriptAndArgs);
                else
                    scriptAndArgs = string.Format("\"{0}\"", script);

                throw new Exception(TryException.Message, TryException);
            }
            return returnElement;
        }

        /// <summary>Injects and executes Javascript in the currently active Selenium browser. If Selenium throws an error, test is aborted.</summary>
        /// <param name="script">Javascript that will be injected into the DOM and executed.</param>
        /// <param name="args">Any arguments passed in to the Javascript</param>
        /// <seealso cref="TryExecuteJavaScriptNoReturnData(string,object[])"/>
        /// <remarks>
        /// Selenium and Appium servers may implement how Javascript execution is perfomed and data passed back.  Some implementations may not
        /// support it directly, requiring a local implementation of the IJavaScriptExecutor interface. This method guarantees a unified method
        /// of executing Javascript in automated tests.
        /// <para/><para/>
        /// It is the responsibility of the tool (or wrapper) to gracefully handle the abort and throw an exception.  The test should have the steps wrapped to enable catching
        /// the abort exception to ensure graceful closure of the test.</remarks>
        /// <example>Clear specific item (TelephoneNum) from local storage:
        /// <code lang="C#">
        /// if (SeleniumDriver.ExecuteJavaScript("window.localStorage.removeItem(\"arguments[0]\");","TelephoneNum"))
        /// {
        ///   // We had a problem executing the script
        ///   SSDebug.WriteLine("MyTest","Test","JavaScript Error - {0}",SeleniumDriver.TryException.Message);
        /// }</code></example>
        public void ExecuteJavaScriptNoReturnData(string script, params object[] args)
        {
            if (!TryExecuteJavaScriptNoReturnData(script, args))
            {
                string scriptAndArgs = string.Empty;
                foreach (object arg in args)
                {
                    if (string.IsNullOrEmpty(scriptAndArgs))
                        scriptAndArgs = string.Format("\"{0}\" -(Args: \"{1}\"", script, (string)arg);
                    else
                        scriptAndArgs = string.Format("{0}, \"{1}\"", scriptAndArgs, (string)arg);
                }
                if (!string.IsNullOrEmpty(scriptAndArgs))
                    scriptAndArgs = string.Format("{0})", scriptAndArgs);
                else
                    scriptAndArgs = string.Format("\"{0}\"", script);
                throw new Exception(TryException.Message, TryException);
            }
        }

    }
}

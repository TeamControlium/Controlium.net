using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TeamControlium.Utilities;

namespace TeamControlium.Controlium
{
    public partial class SeleniumDriver
    {
        /// <summary>Applies Find Logic to DOM and locates an Element.  If find logic does not locate an element or locates more than one returns false and sets
        /// <see cref="TryException"/> to exception that would be thrown
        /// </summary>
        /// <param name="ParentName">Name of parent this Find is from - null for DOM Root level</param>
        /// <param name="RootElement">Element to search from.  If null, search from DOM level</param>
        /// <param name="FindLogic">Find logic to be applied to DOM</param>
        /// <param name="FriendlyName">Human readable name of element being located</param>
        /// <param name="element">Populated with element reference if found.  Null if not found (or more than one element found).</param>
        /// <returns>True if element found or False if not found (or more than one element found)</returns>
        public bool TryFindElement(Element ParentElement, ObjectMappingDetails Mapping, bool AllowMultipleMatches, TimeSpan? TimeoutOverride, TimeSpan? PollOverride, bool WaitUntilStable, out Element element)
        {
            WebDriverWait webDriverWait = GetPollAndTimeout(elementFindTimings, TimeoutOverride, PollOverride);
            List<Element> clauseResults = new List<Element>();
            Stopwatch timer = Stopwatch.StartNew();
            Logger.WriteLine(Logger.LogLevels.FrameworkDebug, "Timeout - {0} Seconds", webDriverWait.Timeout.TotalSeconds);
            while (clauseResults.Count == 0)
            {
                clauseResults = (ParentElement == null) ? FindElements(Mapping) : FindElements(ParentElement, Mapping);
                if (clauseResults.Count == 0) Thread.Sleep(webDriverWait.PollingInterval);
                if ((timer.Elapsed >= webDriverWait.Timeout)) break;
            }
            timer.Stop();
            if (clauseResults.Count > 1 && !AllowMultipleMatches)
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Found {0} matching elements in {1} mS. Do not allow multiple matches.", clauseResults.Count.ToString(), timer.ElapsedMilliseconds.ToString());
                TryException = new FindLogicReturnedMultipleElements(((ParentElement == null) ? "DOM Top Level" : (string.IsNullOrEmpty(ParentElement.MappingDetails.FriendlyName) ? ("Unknown Parent (" + ParentElement.MappingDetails.FindLogic + ")") : ParentElement.MappingDetails.FriendlyName)), Mapping, clauseResults.Count);
                element = null;
                return false;
            }
            if (clauseResults.Count == 1 || (clauseResults.Count > 1 && AllowMultipleMatches))
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Found {0} matching elements in {1} mS.{2}", clauseResults.Count.ToString(), timer.ElapsedMilliseconds.ToString(), AllowMultipleMatches ? " Allow multiple matches so using first match." : string.Empty);

                if (WaitUntilStable && !clauseResults[0].IsPositionStable)
                {
                    if (clauseResults[0].WaitForElementPositionStable())
                    {
                        if (TryFindElement(ParentElement, Mapping, AllowMultipleMatches, TimeoutOverride, PollOverride, false, out element))
                        {
                            return true;
                        }
                        else
                        {
                            TryException = new Exception("Could not find element after waiting for position stable!", TryException);
                            element = null;
                            return false;
                        }
                    }
                }

                element = clauseResults[0];
                return true;
            }
            else
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "Found no matching elements in {0} mS", timer.ElapsedMilliseconds.ToString());
                TryException = new FindLogicReturnedNoElements(ParentElement?.MappingDetails?.FriendlyName??"No parent (searched from top level of DOM)", Mapping, webDriverWait.Timeout.TotalSeconds.ToString(), webDriverWait.PollingInterval.TotalMilliseconds.ToString());
                element = null;
                return false;
            }
        }
        
        internal List<Element> FindElements(Element ParentElement, ObjectMappingDetails Mapping)
        {
            int index = 0;
            List<IWebElement> FoundElements;
            List<Element> returnList = new List<Element>();

            if (ParentElement == null)
                return FindElements(Mapping);
            else
            {
                By SeleniumFindBy = Mapping.SeleniumBy;
                Logger.Write(Logger.LogLevels.FrameworkDebug, "Find Elements: Name - [{0}], Selenium logic - [{1}]", Mapping.FriendlyName, SeleniumFindBy.ToString());
                try
                {
                    FoundElements = ParentElement.WebElement.FindElements(SeleniumFindBy).ToList();
                }
                catch (Exception ex)
                {
                    throw new SeleniumFindElementError(Mapping, ParentElement, ex);
                }
                Logger.WriteLine(Logger.LogLevels.FrameworkDebug, " (Found [{0}] elements)", FoundElements.Count.ToString());

                //
                // Our find logic has returned a bunch of elements.  We _may_ want to use the find logic of a specific element again.  So, when saving the elements add
                // an indexer to the ActualFindLogic.....
                //
                FoundElements.ForEach(iWebElement =>
                {
                    ObjectMappingDetails thisMapping = Mapping.Copy();
                    thisMapping.FriendlyName = string.Format("{0}[{1}]", thisMapping.FriendlyName, index.ToString());
                    if (thisMapping.FindType == ObjectMappingDetails.ByType.XPath)
                        thisMapping.FindLogicUsed = string.Format("({0})[{1}]", thisMapping.FindLogic, index.ToString());
                    Element Element = new Element(ParentElement, iWebElement, thisMapping);
                    returnList.Add(Element);
                    index++;
                });
                return returnList;
            }
        }
        public List<Element> FindElements(ObjectMappingDetails Mapping)
        {
            int index = 0;
            List<IWebElement> FoundElements;
            List<Element> returnList = new List<Element>();

            //
            // Get the Selenium By and perform the Selenium FindElements from the top level to get a list of matching elements
            //
            By SeleniumFindBy = Mapping.SeleniumBy;
            Logger.Write(Logger.LogLevels.FrameworkInformation, "Find Elements: {0}", SeleniumFindBy.ToString());
            try
            {
                FoundElements = webDriver.FindElements(SeleniumFindBy).ToList();
            }
            catch (Exception ex)
            {
                throw new SeleniumFindElementError(Mapping, this, ex);
            }


            Logger.WriteLine(Logger.LogLevels.FrameworkInformation, " (Found [{0}] elements)", FoundElements.Count.ToString());

            //
            // Now go through the list and bind each iWebElement to the framework element.
            //
            FoundElements.ForEach(iWebElement =>
            {
                ObjectMappingDetails thisMapping = Mapping;
                if (thisMapping.FindType == ObjectMappingDetails.ByType.XPath)
                    thisMapping.FindLogicUsed = string.Format("({0})[{1}]", thisMapping.FindLogic, index.ToString());
                Element Element = new Element(this, iWebElement, thisMapping);
                returnList.Add(Element);
                index++;
            });

            return returnList;
        }
    }
}

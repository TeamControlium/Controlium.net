using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamControlium.Framework;

namespace TeamControlium.Framework
{
    /// <summary>
    /// Run settings repository for a Test Automation project
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Delegate for processing Tokens.
        /// </summary>
        /// <remarks>
        /// Throw exception if failure of processing.  Calls to the TokenProcessor are thread-safe and
        /// allows dynamic deassigning of delegates.  TokenProcessor, if set, is called by the framework prior
        /// to storage of any string option is being stored.
        /// </remarks>
        static public Func<string, string> TokenProcessor;

        /// <summary>
        /// Returns the last exception were a TryGetRunCategoryOptions returned false
        /// </summary>
        static public Exception TryException { get; private set; }

        //
        // Main options dictionary.  All options stored using SetCategoryOption or SetOption are stored in this dictionary.  Items are stored as dynamic objects in
        // in the dictionary to allow any object types to be stored.
        //
        static private Dictionary<string, Dictionary<string, dynamic>> options = new Dictionary<string, Dictionary<string, dynamic>>();

        /// <summary>
        /// If object is a string and <see cref="Settings.TokenProcessor"/> has been set, tokens with the string are processed
        /// </summary>
        /// <remarks><see cref="Settings.TokenProcessor"/> is called recursively until string returned matches string sent.  Ensures all tokens and nested tokens removed.</remarks>
        /// <typeparam name="T">Returns same object type as submitted</typeparam>
        /// <param name="ObjectToProcess">Object to process</param>
        /// <returns>Processed object</returns>
        static public T DetokeniseString<T>(T ObjectToProcess)
        {
            //
            // Make sure we are thread-safe. There is a possibility that the framework is in a multi-threaded apartment (MTA) and it is possible for TokenProcessor to become null
            // between checking and use ( if (TokenProcessor!=null) option = (T)(object)TokenProcessor((string)(object)option); ).  Using the processor temporary variable
            // forces .NET to make a copy of the handler. See https://blogs.msdn.microsoft.com/ericlippert/2009/04/29/events-and-races/ for details.
            //
            Func<string, string> processor = null;

            // Only do any processing if the object is a string.
            if (typeof(T) == typeof(String))
            {
                //
                // Call the TokenProcessor in a loop until the string returned is the same as the string passed in.  This indicates any processing has been
                // completed.  Doing this allows token values to themselves contain ) 
                //
                string StringToProcess = string.Empty;
                string ProcessedString = (string)(object)ObjectToProcess;
                Logger.Write(Logger.LogLevels.FrameworkDebug, "Object is a string [{0}].  Processing....", ProcessedString ?? string.Empty);
                while (!StringToProcess.Equals(ProcessedString))
                {
                    StringToProcess = ProcessedString;
                    processor = TokenProcessor;
                    if (processor != null)
                    {
                        ProcessedString = processor(StringToProcess);
                        Logger.WriteLine(Logger.LogLevels.FrameworkDebug, "Processed [{0}] to [{1}]", StringToProcess, ProcessedString ?? string.Empty);
                    }
                    else
                    {
                        Logger.WriteLine(Logger.LogLevels.FrameworkDebug, "No Token Processor active. String not processed");
                        ProcessedString = StringToProcess;
                    }
                    processor = null;
                }
                return (T)(object)ProcessedString;
            }
            else
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkDebug, "Object [{0}] not a string. Not processed", typeof(T).Name);
                return ObjectToProcess;
            }
        }

        /// <summary>Returns named option from named category</summary>
        /// <param name="Category">Category to obtain option from</param>
        /// <param name="Name">Name of option to get</param>
        /// <typeparam name="T">Expected return Type of object being obtained</typeparam>
        /// <returns>Object of Category and Name</returns>
        static public T GetRunCategoryOption<T>(string Category, string Name)
        {
            try
            {
                dynamic obtainedObject = options[Category][Name];

                //                Type obtainedObjectType = ((ObjectHandle)obtainedObject).Unwrap().GetType();    This was how I was doing it.  Seems using GetType() works ok but left this here incase
                //                if (obtainedObjectType.Equals(typeof(T)))                                       there are any issues....
                if (obtainedObject is T)
                    return (T)obtainedObject;
                else
                {
                    throw new Exception(string.Format("Expected type [{0}] but got type [{1}].", typeof(T).Name, obtainedObject.GetType()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Exception getting Category.Name ([{0}].[{1}])", Category, Name), ex);
            }
        }

        /// <summary>Returns named option from named category</summary>
        /// <param name="Category">Category to obtain option from</param>
        /// <param name="Name">Name of option to get</param>
        /// <param name="Value">Object of Category and Name if successful</param>
        /// <typeparam name="T">Expected return Type of object being obtained</typeparam>
        /// <returns>true if object obtained successfullt otherwise false (use TryExeception to exception thrown)</returns>
        static public bool TryGetRunCategoryOption<T>(string Category, string Name, out T Value)
        {
            try
            {
                Value = GetRunCategoryOption<T>(Category, Name);
                return true;
            }
            catch (Exception ex)
            {
                TryException = new Exception(string.Format("Exception getting Category.Name ([{0}].[{1}])", Category, Name), ex);
                Value = default(T);
                return false;
            }
        }

        /// <summary>Returns named option from named category.  Returns null if object not set</summary>
        /// <param name="Category">Category to obtain option from</param>
        /// <param name="Name">Name of option to get</param>
        /// <typeparam name="T">Expected return Type of object being obtained</typeparam>
        /// <returns>Object of Category and Name.  If object does not exist returns default value (Null if a reference type)</returns>
        static public T GetRunCategoryOptionOrDefault<T>(string Category, string Name)
        {
            try
            {
                dynamic obtainedObject = options[Category][Name];
                //        Type obtainedObjectType = ((ObjectHandle)obtainedObject).Unwrap().GetType();
                //        if (obtainedObjectType.Equals(typeof(T)))
                if (obtainedObject.GetType().Equals(typeof(T)))
                    return (T)obtainedObject;
                else
                    throw new Exception(string.Format("Expected type [{0}] but got type [{1}].", typeof(T).Name, obtainedObject.GetType().Name));
            }
            catch
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkInformation, "No data found for [{0}.{1}] - returning [Type ({2})] default value.", Category, Name, typeof(T).Name);
                return default(T);
            }
        }

        /// <summary>
        /// Returns the named option from the NoCategory category
        /// </summary>
        /// <param name="Name">Name of option to get</param>
        /// <typeparam name="T">Expected return Type of object being obtained</typeparam>
        /// <returns>Object of Name option in NoCategory.</returns>
        static public T GetRunOption<T>(string Name)
        {
            //
            // No need to call DetokeniseString as it is done in GetRunCategoryOption...
            //
            return GetRunCategoryOption<T>("NoCategory", Name);
        }


        /// <summary>
        /// Returns the named option from the NoCategory category
        /// </summary>
        /// <param name="Name">Name of option to get</param>
        /// <param name="Value">Object of Name option in NoCategory.</param>
        /// <typeparam name="T">Expected return Type of object being obtained</typeparam>
        /// <returns>True if option found, or false if not</returns>
        static public bool TryGetRunOption<T>(string Name, out T Value)
        {
            //
            // No need to call DetokeniseString as it is done in GetRunCategoryOption...
            //
            return TryGetRunCategoryOption<T>("NoCategory", Name, out Value);
        }



        /// <summary>
        /// Returns the named option from the NoCategory category.
        /// </summary>
        /// <param name="Name">Name of option to get</param>
        /// <typeparam name="T">Expected return Type of object being obtained</typeparam>
        /// <returns>Object of Name option in NoCategory.  If object does not exist returns default value (Null if a reference type)</returns>
        static public T GetRunOptionOrDefault<T>(string Name)
        {
            //
            // No need to call DetokeniseString as it is done in GetRunCategoryOption...
            //
            return GetRunCategoryOptionOrDefault<T>("NoCategory", Name);
        }

        /// <summary>Returns all options defined in passed Category</summary>
        /// <param name="Category">Category to get options for</param>
        /// <returns>Dictionary of KeyValue pairs where Keys are the option names and Values are the option data</returns>
        static public Dictionary<string, dynamic> GetRunCategoryOptions(string Category)
        {
            Dictionary<string, dynamic> returnOptions = new Dictionary<string, dynamic>();
            try
            {
                return options[Category];
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Exception getting options for Category ([{0}])", Category), ex);
            }
        }

        /// <summary>
        /// Sets a runtime option in the defined category and of the defined name
        /// </summary>
        /// <param name="Category">Category of option</param>
        /// <param name="Name">Name of option</param>
        /// <param name="Option">Option to set</param>
        /// <typeparam name="T">Type of object being set (and returned)</typeparam>
        /// <remarks>Underlying tools usually have their own system for storing and obtaining runtime options (Usually a runtime configuartion file). 
        /// This method is ued to explicitly set the options as required.
        /// If the underlying object is a string and the TokenProcessor delegate has been assigned, it is called before the object is returned to the caller.  This
        /// allows test framework/suites to perform any processing of parameter strings (such as token substitution) before obtaining parameters.
        /// </remarks>
        /// <returns>Object (detokenised if needed) being set</returns>
        static public T SetRunCategoryOption<T>(string Category, string Name, T Option)
        {
            // Process option before adding t
            T option = DetokeniseString<T>(Option);

            // Add Category if we dont already have it
            if (!options.ContainsKey(Category)) options.Add(Category, new Dictionary<string, dynamic>());

            // Select Category
            Dictionary<string, dynamic> names = options[Category];

            // Add Name if we dont already have it, otherwise change contents of name
            if (names.ContainsKey(Name))
                names[Name] = option;
            else
                names.Add(Name, option);

            return GetRunCategoryOption<T>(Category, Name);
        }

        /// <summary>
        /// Sets a categoryless option
        /// </summary>
        /// <param name="Name">Name of option (actually put in category NoCategory)</param>
        /// <param name="Option">Option data to set</param>
        /// <typeparam name="T">Type of object being set (and returned)</typeparam>        
        /// <remarks>Underlying tools usually have their own system for storing and obtaining runtime options (Usually a runtime configuartion file).  This method is ued to explicitly set the options as required</remarks>
        /// <returns>Object (detokenised if needed) being set</returns>
        static public T SetRunOption<T>(string Name, T Option)
        {
            return SetRunCategoryOption("NoCategory", Name, Option);
        }

        /// <summary>
        /// Removes options in given category
        /// </summary>
        /// <param name="Category">Name of category to remove options for</param>
        static public void ClearCategoryOptions(string Category)
        {
            try
            {
                options.Remove(Category);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(Logger.LogLevels.FrameworkDebug, "Error clearing category [{0}]: {1}", Category, ex);
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
    }
}
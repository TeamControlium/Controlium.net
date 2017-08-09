using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamControlium.TestFramework
{
    public partial class Utilities
    {
        /// <summary>
        /// Delegate for processing Tokens.
        /// </summary>
        public static Func<string, string> TokenProcessor;


        /// <summary>
        /// If object is a string and <see cref="Settings.TokenProcessor"/> has been set, tokens with the string are processed
        /// </summary>
        /// <remarks><see cref="Settings.TokenProcessor"/> is called recursively until string returned matches string sent.  Ensures all tokens and nested tokens removed.</remarks>
        /// <typeparam name="T">Returns same object type as submitted</typeparam>
        /// <param name="ObjectToProcess">Object to process</param>
        /// <returns>Processed object</returns>
        public static T DetokeniseString<T>(T ObjectToProcess)
        {
            //
            // Make sure we are thread-safe. There is a possibility that the framework is in a multi-threaded apartment (MTA) and it is possible
            // for TokenProcessor to become null between checking and use ( if (TokenProcessor!=null) option = (T)(object)TokenProcessor((string)(object)option); ). 
            // Using the processor temporary variable forces .NET to make a copy of the handler.
            // See https://blogs.msdn.microsoft.com/ericlippert/2009/04/29/events-and-races/ for details.
            //
            Func<string, string> processor = null;

            // Only do any processing if the object is a string.
            if (typeof(T) == typeof(String))
            {
                //
                // Call the TokenProcessor in a loop until the string returned is the same as the string passed in.  This indicates any processing has been
                // completed.  Doing this allows token values to themselves contain tokens) 
                //
                string StringToProcess = string.Empty;
                string ProcessedString = (string)(object)ObjectToProcess;
                Logger.Write(Logger.LogLevels.FrameworkDebug, "Object is a string [{0}]. ", ProcessedString ?? string.Empty);
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

        /// <summary>
        /// Returns true if string does not start with 0 or starts with t(rue), y(es) or o(n)
        /// </summary>
        /// <param name="Value">value to check</param>
        /// <returns>true if string first digit is not 0 or is true, yes or on</returns>
        public static bool IsValueTrue(string Value)
        {
            if (string.IsNullOrEmpty(Value)) return false;
            int i;
            if (int.TryParse(Value, out i))
                if (i > 0) return true; else return false;
            return Value.ToLower().StartsWith("t") || Value.ToLower().StartsWith("y") || Value.ToLower().StartsWith("on");
        }
    }
}

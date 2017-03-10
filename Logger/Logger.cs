using System;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace TeamControlium.Framework
{
    /// <summary>
    /// Enables test scripts to log data that assists in debugging of test scripts and/or framework.  Library and
    /// helper classes write to the debug log to ensure detailed analysis is possible.
    /// Debug text redirection is possible if underlying tool supplies its own logging and/or debug output.
    /// </summary>
    static public class Logger
    {
        static private Stopwatch testTimer;      // Used to keep track of time since first call to Logger class made.
        static private string TestToolString;    // Used to build string as logger Write calls made.
        static private bool WriteStart;          // Indicates if last write was an end-of line or start.  Used to indicate if a Line pre-amble required
        static private object padLock;           // Used for locking during a DoWriteLine to ensure thread safety

        /// <summary>
        /// Instantiates an instant of the Logger static class.  Starts the Stopwatch running for timing information in debug data.
        /// </summary>
        static Logger()
        {
            LoggingLevel = LogLevels.TestInformation; // Default logging level
            WriteStart = true;
            ResetTimer();
        }

        /// <summary>
        /// Levels of logging - Verbose (Maximum) to Exception (Minimum).  If level of text being written to
        /// logging is equal to, or higher than the current LoggingLevel the text is written.
        /// </summary>
        public enum LogLevels
        {
            /// <summary>
            /// Only written to results if Level set to Maximum (Verbose) Output
            /// </summary>
            Verbose = 0,
            /// <summary>
            /// Data written to results if level is Framework Debug or Maximum Output
            /// </summary>
            FrameworkDebug = 1,
            /// <summary>
            /// Data written to results if level is Framework Information/Debug or Maximum Output
            /// </summary>
            FrameworkInformation = 2,
            /// <summary>
            /// Data written to results if level is Test Debug, Framework Information/Debug or Maximum Output
            /// </summary>
            TestDebug = 3,
            /// <summary>
            /// Data written to results if level is Test Information/Debug, Framework Information/Debug or Maximum Output
            /// </summary>
            TestInformation = 4,
            /// <summary>
            /// Data always written to results
            /// </summary>
            Information = 5
        };

        /// <summary>
        /// Level of logging.  Lowest is Information (least amount of log data written), Highest is Verbose (lots of log data written)
        /// </summary>
        static public LogLevels LoggingLevel { get; private set; }

        /// <summary>
        /// Where and how debug data is recorded.
        /// If true, debug data is written to the Console (stdout)
        /// If false, debug data logging is written to the the TestToolLog delegate
        /// </summary>
        static public bool WriteToConsole { get; set; }

        /// <summary>
        /// System delegate to write debug data to if WriteToConsole is false.
        /// </summary>
        /// <seealso cref="WriteToConsole"/>
        static public Action<string> TestToolLog { get; set; }

        /// <summary>
        /// Set logging level. Lowest is Information (least amount of log data written), Highest is MaximumOutput (lots of log data written)
        /// </summary>
        /// <param name="Level">Minimum Output (Least output), Test Information, Test Debug, Framework Information Framework Debug or Maximum Output (Most output)</param>
        static public void SetLoggingLevel(string Level)
        {
            switch (Level.Replace(" ", "").ToLower())
            {
                case "exceptions":
                case "exception":
                case "minimumoutput":
                case "information": LoggingLevel = LogLevels.Information; break;
                case "testinformation": LoggingLevel = LogLevels.TestInformation; break;
                case "testdebug": LoggingLevel = LogLevels.TestDebug; break;
                case "frameworkinformarion": LoggingLevel = LogLevels.FrameworkInformation; break;
                case "frameworkdebug": LoggingLevel = LogLevels.FrameworkDebug; break;
                case "maximumoutput": LoggingLevel = LogLevels.Verbose; break;
                default: throw new ArgumentException("Invalid logging level; must be Minimum Output, Test Information, Test Debug, Framework Information Framework Debug or Maximum Output", "Level");
            }
        }

        /// <summary>
        /// Resets the logger elapsed timer
        /// </summary>
        static public void ResetTimer()
        {
            testTimer = new Stopwatch();
            testTimer.Start();
        }

        /// <summary>
        /// Writes details of a caught exception to the active debug log.  All calls must include Class and Method (or property if a Get/Set property)
        /// information. Exception message is written to debug log (with no stack or inner exception information).  If an exception is thrown during write, Logger
        /// attempts to write the error details if able.
        /// </summary>
        /// <param name="ex">Exception caught and being reported</param>
        /// <example>
        /// <code lang="C#">
        /// catch (InvalidHostURI ex)
        /// {
        ///   // Log exception and abort the test - we cant talk to the remote Selenium server
        ///   Logger.LogException(ex);
        ///   toolWrapper.AbortTest("Cannot connect to remote Selenium host");
        /// }
        /// </code></example>
        static public void LogException(Exception ex)
        {
            StackFrame stackFrame = new StackFrame(1, true);
            DoWriteLine((stackFrame == null) ? null : stackFrame.GetMethod(), LogLevels.Information,
                string.Format("Exception being thrown: {0}", ex.ToString()));
        }
        /// <summary>
        /// Writes details of a caught exception to the active debug log.  All calls must include Class and Method (or property if a Get/Set property)
        /// information. Exception message is written to debug log (with no stack or inner exception information).  If an exception is thrown during write, Logger
        /// attempts to write the error details if able.
        /// </summary>
        /// <param name="ex">Exception caught and being reported</param>
        /// <param name="text">Additional string format text to show when logging exception</param>
        /// <param name="args">Arguments shown in string format text</param>
        /// <example>
        /// <code lang="C#">
        /// catch (InvalidHostURI ex)
        /// {
        ///   // Log exception and abort the test - we cant talk to the remote Selenium server
        ///   Logger.LogException(ex,"Given up trying to connect to [{0}]",Wherever);
        ///   toolWrapper.AbortTest("Cannot connect to remote Selenium host");
        /// }
        /// </code></example>
        static public void LogException(Exception ex, string text, params Object[] args)
        {
            StackFrame stackFrame = new StackFrame(1, true);
            DoWrite((stackFrame == null) ? null : stackFrame.GetMethod(), LogLevels.Information, string.Format(text, args));
            DoWriteLine((stackFrame == null) ? null : stackFrame.GetMethod(), LogLevels.Information,
                string.Format("Exception being thrown: {0}", ex.ToString()));
        }

        /// <summary>
        /// Writes a line of data to the active debug log.
        /// Data can be formatted in the standard string.format syntax.  If an exception is thrown during write, Logger
        /// attempts to write the error deatils if able.
        /// </summary>
        /// <param name="textString">Text to be written</param>
        /// <param name="args">String formatting arguments (if any)</param>
        /// <param name="logLevel">Level of text being written (See <see cref="Logger.LogLevels"/> for usage of the Log Level)</param>
        /// <example>Write a line of data from our test:
        /// <code lang="C#">
        /// Logger.WriteLn(LogLevels.TestDebug, "Select member using key (Member: {0})","90986754332");
        /// </code>code></example>
        static public void Write(LogLevels logLevel, string textString, params Object[] args)
        {
            StackFrame stackFrame = new StackFrame(1, true);
            DoWrite((stackFrame == null) ? null : stackFrame.GetMethod(), logLevel, string.Format(textString, args));
        }

        /// <summary>
        /// Writes a line of data to the active debug log. 
        /// Data can be formatted in the standard string.format syntax.  If an exception is thrown during write, Logger
        /// attempts to write the error deatils if able.
        /// </summary>
        /// <param name="logLevel">Level of text being written (See <see cref="Logger.LogLevels"/> for usage of the Log Level)</param>
        /// <param name="textString">Text to be written</param>
        /// <param name="args">String formatting arguments (if any)</param>
        /// <example>Write a line of data from our test:
        /// <code lang="C#">
        /// Logger.WriteLine(LogLevels.TestDebug, "Select member using key (Member: {0})","90986754332");
        /// </code></example>
        static public void WriteLine(LogLevels logLevel, string textString, params Object[] args)
        {
            StackFrame stackFrame = new StackFrame(1, true);
            DoWriteLine((stackFrame == null) ? null : stackFrame.GetMethod(), logLevel,
                    string.Format(textString, args));
        }

        /// <summary>
        /// Writes given Text to a text file, optionally auto versioning (adding (n) to filename) OR
        /// overwriting.
        /// </summary>
        /// <remarks>
        /// No exception is raised if there is any problem, but details of error is written to Logger log
        /// </remarks>
        /// <param name="Filename">Full path and filename to use</param>
        /// <param name="AutoVersion">If true and file exists. (n) is added to auto-version.  If false and file exists, it is overwritten if able</param>
        /// <param name="Text">Text to write</param>
        static public void WriteTextToFile(string Filename, bool AutoVersion, string Text)
        {
            try
            {
                string FilenameToUse = Filename;
                if (AutoVersion)
                {
                    int count = 1;
                    string fileNameOnly = Path.GetFileNameWithoutExtension(Filename);
                    string extension = Path.GetExtension(Filename);
                    string path = Path.GetDirectoryName(Filename);
                    FilenameToUse = Filename;

                    while (File.Exists(FilenameToUse))
                    {
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                        FilenameToUse = Path.Combine(path, tempFileName + extension);
                    }
                }

                File.WriteAllText(FilenameToUse, Text);
            }
            catch (Exception ex)
            {
                WriteLine(LogLevels.Information, "Error writing data to file [{0}] (AutoVersion={1}):{2}", Filename,
                    AutoVersion ? "Yes" : "No", ex.Message);
            }
        }

        /// <summary>
        /// Gets class-type and Method name of passed MethodBase class.
        /// </summary>
        /// <param name="methodBase">MethodBase of class</param>
        /// <returns>Formatted string containing Type.Method</returns>
        static private string CallingMethodDetails(MethodBase methodBase)
        {
            string methodName = "<Unknown>";
            string typeName = "<Unknown>";
            if (methodBase != null)
            {
                methodName = methodBase.Name ?? methodName;
                if (methodBase.DeclaringType != null)
                {
                    typeName = methodBase.DeclaringType.Name ?? typeName;
                }
            }
            return string.Format("{0}.{1}", typeName, methodName);
        }


        /// <summary>
        /// Appends text to currently active line.  If the start of line, text is pre-pended with Line header information
        /// </summary>
        /// <param name="methodBase">MethodBase of class calling Logger class</param>
        /// <param name="TypeOfWrite">Level of debug text to be written</param>
        /// <param name="textString">Text string to be written</param>
        /// <remarks>Text is written if TypeOfWrite is equal to, or higher the current Logging Level</remarks>
        static private void DoWrite(MethodBase methodBase, LogLevels TypeOfWrite, string textString)
        {
            if (TypeOfWrite >= LoggingLevel)
            {
                TestToolString += (WriteStart ? GetPreAmble(methodBase, TypeOfWrite) : string.Empty) + textString ?? string.Empty;
                WriteStart = false;
            }
        }

        /// <summary>
        /// Appends text to currently active line and writes line to active log.  If new line, text is pre-pended with Line header information
        /// </summary>
        /// <param name="methodBase">MethodBase of class calling Logger class</param>
        /// <param name="TypeOfWrite">Level of debug text to be written</param>
        /// <param name="textString">Text string to be written</param>
        /// <remarks>Text is written if TypeOfWrite is equal to, or higher the current Logging Level</remarks> 
        static private void DoWriteLine(MethodBase methodBase, LogLevels TypeOfWrite, string textString)
        {
            if (TypeOfWrite >= LoggingLevel)
            {
                lock (padLock)
                {
                    if (WriteStart)
                    {
                        TestToolString += GetPreAmble(methodBase, TypeOfWrite) + textString ?? string.Empty;
                    }
                    else
                    {
                        WriteStart = true;
                        TestToolString += textString;
                    }
                    try
                    {
                        if (WriteToConsole)
                            Console.WriteLine(TestToolString);
                        else
                            TestToolLog(TestToolString);
                    }
                    catch (Exception ex)
                    {
                        string details;
                        using (EventLog appLog = new EventLog("Application"))
                        {
                            if (WriteToConsole)
                            {
                                details = "console (STDOUT)";
                            }
                            else
                            {
                                details = string.Format("delegate provide by tool ({0}).", (TestToolLog == null) ? "Is null - Has not been implemented!" : "Not null - Has been implemented");
                            }
                            appLog.Source = "Application";
                            appLog.WriteEntry(string.Format("AppServiceInterfaceMock - Logger error writing to {0}.\r\n\r\nAttempt to write line;\r\n{1}", details, TestToolString, ex), EventLogEntryType.Warning, 12791, 1);
                        }
                    }
                    finally
                    {
                        TestToolString = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Constructs and returns a log-file pre-amble.  Preamble is {Log Type} {Time} [Calling Type.Method]:
        /// </summary>
        /// <param name="methodBase">Reference to calling method</param>
        /// <param name="TypeOfWrite">Type of write</param>
        /// <returns>Line pre-amble text</returns>
        static private string GetPreAmble(MethodBase methodBase, LogLevels TypeOfWrite)
        {
            string time = String.Format("[{0:HH:mm:ss.ff}][{1:00000.00}]", DateTime.Now, testTimer.Elapsed.TotalSeconds);
            int totalTimeLength = time.Length + (8 - (int)TypeOfWrite);
            if ((time.Length + 1) <= totalTimeLength) time = time.PadRight(totalTimeLength);
            string preAmble = String.Format("{0} {1} [{2}]: ", WriteTypeString(TypeOfWrite),
                time,
                CallingMethodDetails(methodBase));
            return preAmble;
        }

        /// <summary>
        /// Returns debug line inital token based on LogLevel of text being written
        /// </summary>
        /// <param name="TypeOfWrite">Log Level to obtain text for</param>
        /// <returns>Textual representation for Debug log line pre-amble</returns>
        static private string WriteTypeString(LogLevels TypeOfWrite)
        {
            switch (TypeOfWrite)
            {
                case LogLevels.Verbose:
                    return "LOG-LV";
                case LogLevels.FrameworkDebug:
                    return "LOG-FD";
                case LogLevels.FrameworkInformation:
                    return "LOG-FI";
                case LogLevels.TestDebug:
                    return "LOG-TD";
                case LogLevels.TestInformation:
                    return "LOG-TI";
                default:
                    return "LOG-IN";
            }
        }
    }
}
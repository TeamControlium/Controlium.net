using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamControlium.Framework;

namespace TeamControlium.Framework
{
    /// <summary>
    /// Run-settings repository for a Test Automation project
    /// </summary>
    public class Settings
    {


 

        /// <summary>
        /// Removes options in given category
        /// </summary>
        /// <param name="Category">Name of category to remove options for</param>
        static public void ClearCategoryOptions(string Category)
        {
            try
            {
                testData.Remove(Category);
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
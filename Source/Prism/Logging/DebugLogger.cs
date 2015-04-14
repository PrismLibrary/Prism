using System;
using System.Diagnostics;
using System.Globalization;
using Prism.Properties;

namespace Prism.Logging
{
    /// <summary>
    /// Implementation of <see cref="ILoggerFacade"/> that logs into a message into the Debug.Listeners collection.
    /// </summary>
    public class DebugLogger : ILoggerFacade
    {
        /// <summary>
        /// Write a new log entry with the specified category and priority.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category of the entry.</param>
        /// <param name="priority">The priority of the entry.</param>
        public void Log(string message, Category category, Priority priority)
        {
            string messageToLog = String.Format(CultureInfo.InvariantCulture, Resources.DefaultDebugLoggerPattern, DateTime.Now,
                                                category.ToString().ToUpper(), message, priority);

            Debug.WriteLine(messageToLog);
        }
    }
}

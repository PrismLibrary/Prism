// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Prism.Logging
{
    /// <summary>
    /// Implementation of <see cref="ILoggerFacade"/> that logs to .NET <see cref="Trace"/> class.
    /// </summary>
    public class TraceLogger : ILoggerFacade
    {
        /// <summary>
        /// Write a new log entry with the specified category and priority.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category of the entry.</param>
        /// <param name="priority">The priority of the entry.</param>
        public void Log(string message, Category category, Priority priority)
        {
            if (category == Category.Exception)
            {
                Trace.TraceError(message);
            }
            else
            {
                Trace.TraceInformation(message);
            }
        }
    }
}
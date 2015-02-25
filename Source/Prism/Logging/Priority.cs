// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace Prism.Logging
{
    /// <summary>
    /// Defines values for the priorities used by <see cref="ILoggerFacade"/>.
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// No priority specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// High priority entry.
        /// </summary>
        High = 1,

        /// <summary>
        /// Medium priority entry.
        /// </summary>
        Medium,

        /// <summary>
        /// Low priority entry.
        /// </summary>
        Low
    }
}
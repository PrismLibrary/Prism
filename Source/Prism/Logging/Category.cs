// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace Prism.Logging
{
    /// <summary>
    /// Defines values for the categories used by <see cref="ILoggerFacade"/>.
    /// </summary>
    public enum Category
    {
        /// <summary>
        /// Debug category.
        /// </summary>
        Debug,

        /// <summary>
        /// Exception category.
        /// </summary>
        Exception,

        /// <summary>
        /// Informational category.
        /// </summary>
        Info,

        /// <summary>
        /// Warning category.
        /// </summary>
        Warn
    }
}
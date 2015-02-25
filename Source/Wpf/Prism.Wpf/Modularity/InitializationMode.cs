// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Prism.Modularity
{
    /// <summary>
    /// Specifies on which stage the Module group will be initialized.
    /// </summary>
    public enum InitializationMode
    {
        /// <summary>
        /// The module will be initialized when it is available on application start-up.
        /// </summary>
        WhenAvailable,

        /// <summary>
        /// The module will be initialized when requested, and not automatically on application start-up.
        /// </summary>
        OnDemand
    }
}

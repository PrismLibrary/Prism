// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Prism.Modularity
{
    /// <summary>
    /// Declares a service which initializes the modules into the application.
    /// </summary>
    public interface IModuleInitializer
    {
        /// <summary>
        /// Initializes the specified module.
        /// </summary>
        /// <param name="moduleInfo">The module to initialize</param>
        void Initialize(ModuleInfo moduleInfo);
    }
}

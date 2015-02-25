// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace Prism.Modularity
{
    /// <summary>
    /// Defines the contract for the modules deployed in the application.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        void Initialize();
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Prism.Modularity
{
    /// <summary>
    /// Interface for classes that are responsible for resolving and loading assembly files. 
    /// </summary>
    public interface IAssemblyResolver
    {
        /// <summary>
        /// Load an assembly when it's required by the application. 
        /// </summary>
        /// <param name="assemblyFilePath"></param>
        void LoadAssemblyFrom(string assemblyFilePath);
    }
}

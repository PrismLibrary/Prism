// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Prism.Interfaces
{
    /// <summary>
    /// The IResourceLoader interface abstracts the Windows.ApplicationModel.Resources.ResourceLoader object for use by apps that derive from the MvvmAppBase class.
    /// A ResourceLoader represents a class that reads the assembly resource file and looks for a named resource. The default implementation of IResourceLoader
    /// is the ResourceLoaderAdapter class, which simply passes method invocations to an underlying Windows.ApplicationModel.Resources.ResourceLoader object.
    /// </summary>
    public interface IResourceLoader
    {
        /// <summary>
        /// Gets the value of the named resource.
        /// </summary>
        /// <param name="resource">The resource name.</param>
        /// <returns>The named resource value.</returns>
        string GetString(string resource);
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Prism.Regions
{
    /// <summary>
    /// Interface for allowing extensible behavior on regions.
    /// </summary>
    public interface IRegionBehavior
    {
        /// <summary>
        /// The region that this behavior is extending.
        /// </summary>
        IRegion Region { get; set; }

        /// <summary>
        /// Attaches the behavior to the specified region.
        /// </summary>
        void Attach();

    }
}
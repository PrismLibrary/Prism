// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


using System;

namespace Prism.Events
{
    /// <summary>
    /// Represents a reference to a <see cref="Delegate"/>.
    /// </summary>
    public interface IDelegateReference
    {
        /// <summary>
        /// Gets the referenced <see cref="Delegate" /> object.
        /// </summary>
        /// <value>A <see cref="Delegate"/> instance if the target is valid; otherwise <see langword="null"/>.</value>
        Delegate Target { get; }
    }
}
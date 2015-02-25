// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Represents an interaction request used for confirmations.
    /// </summary>
    public interface IConfirmation : INotification
    {
        /// <summary>
        /// Gets or sets a value indicating that the confirmation is confirmed.
        /// </summary>
        bool Confirmed { get; set; }
    }
}

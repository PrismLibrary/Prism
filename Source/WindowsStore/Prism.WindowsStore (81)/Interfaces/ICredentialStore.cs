// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


using Windows.Security.Credentials;

namespace Prism.Interfaces
{
    /// <summary>
    /// The ICredentialStore interface abstracts the Windows.Security.Credentials.PasswordVault object for managing the users credentials of your application.
    /// A PasswordVault represents a Credential Locker. The default implementation of ICredentialStore
    /// is the RoamingCredentialStore class, which uses the Windows.Security.Credentials.PasswordVault object to get, save, and remove the application credentials.
    /// </summary>
    public interface ICredentialStore
    {
        /// <summary>
        /// Saves the credentials in the password vault, for the specified resource.
        /// </summary>
        /// <param name="resource">The resource for which the credentials will be stored. For example, the application Name.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        void SaveCredentials(string resource, string userName, string password);

        /// <summary>
        /// Gets the saved credentials for the specified resource.
        /// </summary>
        /// <param name="resource">The resource name of the credentials that will be stored.</param>
        /// <returns>The <see cref="PasswordCredential"/> instance containing all the saved credentials for the specified resource.</returns>
        PasswordCredential GetSavedCredentials(string resource);

        /// <summary>
        /// Removes the specified saved credentials.
        /// </summary>
        /// <param name="resource">The name of the resource that will have its credential removed.</param>
        void RemoveSavedCredentials(string resource);
    }
}

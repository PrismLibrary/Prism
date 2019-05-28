﻿using System;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for ViewModels involved in navigation to be notified of navigation activities prior to the target Page being added to the navigation stack.
    /// </summary>
    [Obsolete("Use IInitialize, IInitializeAsync, or IAutoInitialize instead", true)]
    public interface INavigatingAware
    {
        /// <summary>
        /// Called before the implementor has been navigated to.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        /// <remarks>Not called when using device hardware or software back buttons</remarks>
        [Obsolete("Use IInitialize, IInitializeAsync, or IAutoInitialize instead", true)]
        void OnNavigatingTo(INavigationParameters parameters);
    }
}

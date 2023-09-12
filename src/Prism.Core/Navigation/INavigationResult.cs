using System;
using Prism.Navigation.Regions;

namespace Prism.Navigation;

#nullable enable
/// <summary>
/// Provides a wrapper for a Navigation Result
/// </summary>
public interface INavigationResult
{
    /// <summary>
    /// Indicates that the navigation was successful and no Navigation Errors occurred
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// If <c>true</c> this indicates that the Navigation Event was cancelled.
    /// </summary>
    bool Cancelled { get; }

    /// <summary>
    /// The Exception if one occurred.
    /// </summary>
    Exception? Exception { get; }

    /// <summary>
    /// If the <see cref="INavigationResult"/> is the result of Region Navigation
    /// This will provide the associate <see cref="NavigationContext"/>
    /// </summary>
    NavigationContext? Context { get; }
}

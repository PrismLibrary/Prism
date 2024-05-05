using System;
using Prism.Navigation.Regions;

namespace Prism.Navigation;

#nullable enable
/// <summary>
/// Default implementation for the <see cref="INavigationResult"/>
/// </summary>
public class NavigationResult : INavigationResult
{
    private readonly bool? _success;

    /// <summary>
    /// Initializes a new Navigation Result
    /// </summary>
    public NavigationResult()
    {
    }

    /// <summary>
    /// Initializes a new NavigationResult
    /// </summary>
    /// <param name="success"></param>
    public NavigationResult(bool success)
    {
        _success = success;
    }

    /// <summary>
    /// Initializes a new NavigationResult with an <see cref="Exception"/>
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> encountered as part of the navigation.</param>
    public NavigationResult(Exception? ex)
    {
        Exception = ex;
    }

    /// <summary>
    /// Initializes a new NavigationResult
    /// </summary>
    /// <param name="context"></param>
    /// <param name="success"></param>
    public NavigationResult(NavigationContext context, bool success)
    {
        Context = context;
        _success = success;
    }

    /// <summary>
    /// Initializes a new NavigationResult
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    public NavigationResult(NavigationContext context, Exception exception)
    {
        Context = context;
        Exception = exception;
    }

    /// <inheritdoc />
    public bool Success => _success ?? Exception is null;

    /// <inheritdoc />
    public bool Cancelled =>
        Exception is NavigationException navigationException
        && navigationException.Message == NavigationException.IConfirmNavigationReturnedFalse;

    /// <inheritdoc />
    public Exception? Exception { get; }

    /// <inheritdoc />
    public NavigationContext? Context { get; }
}

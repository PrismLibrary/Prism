using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prism.Common;

#nullable enable
/// <summary>
/// Provides a wrapper for managing multicast delegates for handling specific errors
/// </summary>
public struct MulticastExceptionHandler
{
    private readonly Dictionary<Type, MulticastDelegate> _handlers;

    /// <summary>
    /// Initializes a new MulticastExceptionHandler
    /// </summary>
    public MulticastExceptionHandler()
    {
        _handlers = new Dictionary<Type, MulticastDelegate>();
    }

    public void Register<TException>(MulticastDelegate callback)
        where TException : Exception
    {
        _handlers.Add(typeof(TException), callback);
    }

    /// <summary>
    /// Determines if there is a callback registered to handle the specified exception
    /// </summary>
    /// <param name="exception">An <see cref="Exception"/> to handle or rethrow</param>
    /// <returns><c>True</c> if a Callback has been registered for the given type of <see cref="Exception"/>.</returns>
    public bool CanHandle(Exception exception) =>
        GetDelegate(exception.GetType()) is not null;

    /// <summary>
    /// Handles a specified 
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="parameter"></param>
    public async void Handle(Exception exception, object? parameter = null) =>
        await HandleAsync(exception, parameter);

    public async Task HandleAsync(Exception exception, object? parameter = null)
    {
        var multicastDelegate = GetDelegate(exception.GetType());

        if (multicastDelegate is null)
            return;

        // Get Invoke() method of the delegate
        var invokeMethod = multicastDelegate.GetType().GetMethod("Invoke");

        if (invokeMethod == null)
            throw new InvalidOperationException($"Could not find Invoke() method for delegate of type {multicastDelegate.GetType().Name}");

        var parameters = invokeMethod.GetParameters();
        var arguments = parameters.Length switch
        {
            0 => Array.Empty<object?>(),
            1 => typeof(Exception).IsAssignableFrom(parameters[0].ParameterType) ? new object?[] { exception } : new object?[] { parameter },
            2 => typeof(Exception).IsAssignableFrom(parameters[0].ParameterType) ? new object?[] { exception, parameter } : new object?[] { parameter, exception },
            _ => throw new InvalidOperationException($"Handler of type {multicastDelegate.GetType().Name} is not supported", exception)
        };

        // Invoke the delegate
        var result = invokeMethod.Invoke(multicastDelegate, arguments);

        // If the handler is async (returns a Task), then we await the task
        if (result is Task task)
        {
            await task;
        }
#if NET6_0_OR_GREATER
        else if (result is ValueTask valueTask)
        {
            await valueTask;
        }
#endif
    }

    private MulticastDelegate? GetDelegate(Type type)
    {
        if (_handlers.ContainsKey(type))
            return _handlers[type];
        else if (type.BaseType is not null)
            return GetDelegate(type.BaseType);

        return null;
    }
}

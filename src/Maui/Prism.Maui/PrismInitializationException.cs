namespace Prism;

/// <summary>
/// Represents errors that occur during application initialization.
/// </summary>
public sealed class PrismInitializationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrismInitializationException"/> class with a specified error
    /// message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PrismInitializationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

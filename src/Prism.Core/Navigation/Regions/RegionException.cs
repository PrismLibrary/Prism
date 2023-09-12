using System;
using System.Runtime.Serialization;

namespace Prism.Navigation.Regions;

/// <summary>
/// Provides a common base class for Region Exceptions
/// </summary>
public abstract class RegionException : Exception
{
    /// <inheritdoc />
    protected RegionException()
    {
    }

    /// <inheritdoc />
    protected RegionException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    protected RegionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    /// <inheritdoc />
    protected RegionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

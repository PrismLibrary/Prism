using System;
#if NETFRAMEWORK
using System.Runtime.Serialization;
#endif

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

#if NETFRAMEWORK
    /// <inheritdoc cref="Exception(SerializationInfo, StreamingContext)" />
    protected RegionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
#endif

    /// <inheritdoc />
    protected RegionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

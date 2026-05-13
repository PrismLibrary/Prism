using System;
#if NETFRAMEWORK
using System.Runtime.Serialization;
#endif

namespace Prism.Navigation.Regions;

/// <summary>
/// An exception used when encountering an error in the creation of a Region
/// </summary>
#if NETFRAMEWORK
[Serializable]
#endif
public sealed class RegionCreationException : RegionException
{
    /// <inheritdoc />
    public RegionCreationException()
    {
    }

    /// <inheritdoc />
    public RegionCreationException(string message) : base(message)
    {
    }

#if NETFRAMEWORK
    /// <inheritdoc cref="RegionException(SerializationInfo, StreamingContext)" />
    public RegionCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
#endif

    /// <inheritdoc />
    public RegionCreationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

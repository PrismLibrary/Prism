using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Prism.Navigation.Regions;

/// <summary>
/// An exception used when encountering an error in the creation of a Region
/// </summary>
[Serializable]
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

    /// <inheritdoc />
    public RegionCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    /// <inheritdoc />
    public RegionCreationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

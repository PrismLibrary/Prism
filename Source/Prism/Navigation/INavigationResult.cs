using System;

namespace Prism.Navigation
{
    public interface INavigationResult
    {
        bool Success { get; }

        Exception Exception { get; }
    }
}

using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace Prism.Tests.Lifetime;

internal static class CollectionProbe
{
    // Read weak targets only after all collections, outside the lifetime helper.
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void Collect()
    {
        for (var i = 0; i < 3; i++)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
            GC.WaitForPendingFinalizers();
        }
    }

    internal static void Collected(WeakReference reference)
    {
        Collect();
        Assert.False(reference.IsAlive);
    }
}

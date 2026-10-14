#nullable enable

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Prism.Mvvm;
using Xunit;

namespace Prism.Tests.Lifetime;

[CollectionDefinition("Static factory lifetime", DisableParallelization = true)]
public class StaticFactoryLifetimeCollection { }

[Collection("Static factory lifetime")]
public class StaticFactoryLifetimeFixture
{
    [Fact]
    public void RegisteredCapturingFactoryOwnsTargetUntilRegistrationReplaced()
    {
        var field = typeof(ViewModelLocationProvider).GetField("_factories", BindingFlags.Static | BindingFlags.NonPublic)!;
        var priorFactories = field.GetValue(null);
        try
        {
            // Preserve other tests' global state, while isolating this fixture's registration.
            field.SetValue(null, Activator.CreateInstance(field.FieldType));
            var reference = RegisterCapturingFactory();
            CollectionProbe.Collect();
            Assert.True(reference.IsAlive);
            ReplaceFactory();
            CollectionProbe.Collected(reference);
        }
        finally { field.SetValue(null, priorFactories); }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ReplaceFactory() => ViewModelLocationProvider.Register("Lifetime.Test.View", static () => new object());

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference RegisterCapturingFactory()
    {
        var target = new object();
        ViewModelLocationProvider.Register("Lifetime.Test.View", () => target);
        return new WeakReference(target);
    }
}

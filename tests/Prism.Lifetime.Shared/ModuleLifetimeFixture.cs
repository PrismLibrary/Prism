using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Moq;
using Prism.Modularity;
using Xunit;

namespace Prism.Tests.Lifetime;

public class ModuleLifetimeFixture
{
    [Fact]
    [Trait("Lifetime", "KnownRetention")]
    public void DisposedManagerShouldCollectWithCustomLoaderPublisherAlive()
    {
        var loader = new RetainedLoader();
        var reference = LoadAndDispose(loader);
        Assert.Equal(1, loader.Loads);
        CollectionProbe.Collected(reference);
        GC.KeepAlive(loader);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference LoadAndDispose(RetainedLoader loader)
    {
        var catalog = new ModuleCatalog();
        catalog.AddModule(new ModuleInfo("Pending", "Missing.Type, Missing.Assembly")
            { InitializationMode = InitializationMode.OnDemand });
        var manager = new ModuleManager(Mock.Of<IModuleInitializer>(), catalog)
            { ModuleTypeLoaders = new List<IModuleTypeLoader> { loader } };
        manager.LoadModule("Pending");
        Assert.Equal(ModuleState.Initialized, catalog.Modules.Single().State);
        manager.Dispose();
        return new WeakReference(manager);
    }

    private sealed class RetainedLoader : IModuleTypeLoader
    {
        public int Loads;
        public bool CanLoadModuleType(IModuleInfo info) => true;
        public void LoadModuleType(IModuleInfo info)
        {
            Loads++;
            LoadModuleCompleted?.Invoke(this, new LoadModuleCompletedEventArgs(info, null));
        }
#pragma warning disable CS0067
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;
#pragma warning restore CS0067
    }
}

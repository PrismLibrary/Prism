#nullable enable

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
    public void DisposedManagerShouldCollectWithCustomLoaderPublisherAlive()
    {
        var loader = new RetainedLoader();
        var reference = LoadAndDispose(loader);
        Assert.Equal(1, loader.Loads);
        CollectionProbe.Collected(reference);
        GC.KeepAlive(loader);
    }

    [Fact]
    public void DisposeDetachesSubscriptionsBeforeLoaderDisposalAndOnlyDisposesOnce()
    {
        var loader = new DisposableLoader();
        var manager = CreateManager(loader, Mock.Of<IModuleInitializer>());
        manager.LoadModule("Module");
        Assert.Equal(1, loader.ProgressListeners);
        Assert.Equal(1, loader.CompletionListeners);
        var lateProgress = 0;
        manager.ModuleDownloadProgressChanged += (_, _) => lateProgress++;
        manager.Dispose();
        manager.Dispose();
        Assert.Equal(1, loader.DisposeCalls);
        Assert.Equal(0, loader.ProgressListeners);
        Assert.Equal(0, loader.CompletionListeners);
        Assert.Equal(0, lateProgress);
        loader.RaiseProgress();
        Assert.Equal(0, lateProgress);
        Assert.Throws<ObjectDisposedException>(() => manager.Run());
        Assert.Throws<ObjectDisposedException>(() => manager.LoadModule("Module"));
        Assert.Equal(1, loader.Loads);
    }

    [Fact]
    public void CallbacksCapturedBeforeDisposeDoNotInitializeOrReportProgressAfterDispose()
    {
        var initializer = new Mock<IModuleInitializer>();
        var loader = new RetainedLoader { CompleteOnLoad = false };
        var manager = CreateManager(loader, initializer.Object);
        manager.LoadModule("Module");
        Assert.Equal(ModuleState.LoadingTypes, manager.Modules.Single().State);
        var complete = loader.CaptureCompletion();
        var progress = loader.CaptureProgress();
        var notifications = 0;
        manager.ModuleDownloadProgressChanged += (_, _) => notifications++;
        manager.Dispose();
        complete(loader, new LoadModuleCompletedEventArgs(loader.Module, null));
        progress(loader, new ModuleDownloadProgressChangedEventArgs(loader.Module, 1, 1));
        initializer.Verify(i => i.Initialize(It.IsAny<IModuleInfo>()), Times.Never());
        Assert.Equal(ModuleState.LoadingTypes, loader.Module.State);
        Assert.Equal(0, notifications);
    }

    [Fact]
    public void DisposeUnsubscribesPreviouslyUsedLoaderEvenWhenLoaderListWasReplaced()
    {
        var used = new RetainedLoader();
        var replacement = new DisposableLoader();
        var manager = CreateManager(used, Mock.Of<IModuleInitializer>());
        manager.LoadModule("Module");
        manager.ModuleTypeLoaders = new[] { replacement };
        manager.Dispose();
        Assert.Equal(0, used.ProgressListeners);
        Assert.Equal(0, used.CompletionListeners);
        Assert.Equal(1, replacement.DisposeCalls);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference LoadAndDispose(RetainedLoader loader)
    {
        var manager = CreateManager(loader, Mock.Of<IModuleInitializer>());
        manager.LoadModule("Module");
        Assert.Equal(ModuleState.Initialized, manager.Modules.Single().State);
        manager.Dispose();
        return new WeakReference(manager);
    }

    private static ModuleManager CreateManager(RetainedLoader loader, IModuleInitializer initializer)
    {
        var catalog = new ModuleCatalog();
        catalog.AddModule(new ModuleInfo("Module", "Missing.Type, Missing.Assembly")
            { InitializationMode = InitializationMode.OnDemand });
        return new ModuleManager(initializer, catalog)
            { ModuleTypeLoaders = new List<IModuleTypeLoader> { loader } };
    }

    private class RetainedLoader : IModuleTypeLoader
    {
        public int Loads;
        public bool CompleteOnLoad = true;
        private IModuleInfo? _module;
        public IModuleInfo Module => _module ?? throw new InvalidOperationException("No module has been loaded.");
        public int ProgressListeners => ModuleDownloadProgressChanged?.GetInvocationList().Length ?? 0;
        public int CompletionListeners => LoadModuleCompleted?.GetInvocationList().Length ?? 0;
        public bool CanLoadModuleType(IModuleInfo info) => true;
        public void LoadModuleType(IModuleInfo info)
        {
            Loads++;
            _module = info;
            if (CompleteOnLoad) LoadModuleCompleted?.Invoke(this, new LoadModuleCompletedEventArgs(info, null));
        }
        public void RaiseProgress()
        {
            if (_module != null)
                ModuleDownloadProgressChanged?.Invoke(this, new ModuleDownloadProgressChangedEventArgs(_module, 1, 1));
        }
        public EventHandler<LoadModuleCompletedEventArgs> CaptureCompletion() =>
            LoadModuleCompleted ?? throw new InvalidOperationException("No completion listener is subscribed.");
        public EventHandler<ModuleDownloadProgressChangedEventArgs> CaptureProgress() =>
            ModuleDownloadProgressChanged ?? throw new InvalidOperationException("No progress listener is subscribed.");
        public event EventHandler<ModuleDownloadProgressChangedEventArgs>? ModuleDownloadProgressChanged;
        public event EventHandler<LoadModuleCompletedEventArgs>? LoadModuleCompleted;
    }

    private sealed class DisposableLoader : RetainedLoader, IDisposable
    {
        public int DisposeCalls;
        public void Dispose()
        {
            DisposeCalls++;
            RaiseProgress();
        }
    }
}

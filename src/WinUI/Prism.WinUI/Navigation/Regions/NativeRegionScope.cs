using Prism.Properties;
using System.Runtime.CompilerServices;
using Prism.Mvvm;
using Prism.Windows;

namespace Prism.Navigation.Regions;

// The provider belongs to the originating window, not to the currently active window.
// Keep this association outside IRegion so custom region implementations need no new API.
internal sealed class NativeRegionScope
{
    private static readonly ConditionalWeakTable<IRegion, NativeRegionScope> Providers = new();
    private static readonly ConditionalWeakTable<DependencyObject, IContainerProvider> ViewProviders = new();
    private readonly IContainerProvider _provider;
    private readonly List<Action> _cleanup = new();
    private bool _closed;

    private NativeRegionScope(IContainerProvider provider)
    {
        _provider = provider;
    }

    internal static IContainerProvider FindProvider(DependencyObject element)
    {
        while (element is not null)
        {
            if (element.GetValue(ViewModelLocator.ContainerProviderProperty) is IContainerProvider provider)
                return provider;

            element = VisualTreeHelper.GetParent(element) ?? (element as FrameworkElement)?.Parent;
        }

        return null;
    }

    internal static IContainerProvider RequireProvider(DependencyObject element)
    {
        var provider = FindProvider(element) ?? throw new InvalidOperationException(
            NativeResources.RegionRequiresPrismWindow);
        VerifyProvider(provider);
        return provider;
    }

    internal static void VerifyProvider(IContainerProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        if (!ReferenceEquals(provider.Resolve<IWindowContext>().Container, provider))
            throw new InvalidOperationException(NativeResources.ContainerProviderDoesNotBelongToWindowContext);
    }

    internal static IContainerProvider RequireProvider(IRegion region)
    {
        if (Providers.TryGetValue(region, out var scope))
        {
            if (scope._closed)
                throw new ObjectDisposedException(nameof(IRegion), NativeResources.RegionOwningWindowClosed);

            VerifyProvider(scope._provider);
            return scope._provider;
        }

        if (region.RegionManager is RegionManager manager)
            return manager.ContainerProvider;

        throw new InvalidOperationException(NativeResources.RegionHasNoWindowContainerProvider);
    }

    internal static void SetProvider(IRegion region, IContainerProvider provider)
    {
        VerifyProvider(provider);
        if (Providers.TryGetValue(region, out var existing))
        {
            if (!ReferenceEquals(existing._provider, provider))
                throw new InvalidOperationException(NativeResources.RegionCannotChangeWindowScope);

            return;
        }

        Providers.Add(region, new NativeRegionScope(provider));
    }

    internal static void RegisterCleanup(IRegion region, Action cleanup)
    {
        var provider = RequireProvider(region);
        if (!Providers.TryGetValue(region, out var scope))
        {
            scope = new NativeRegionScope(provider);
            Providers.Add(region, scope);
        }
        scope._cleanup.Add(cleanup);
    }

    internal static bool BelongsTo(IRegion region, IContainerProvider provider) =>
        Providers.TryGetValue(region, out var scope) && ReferenceEquals(scope._provider, provider);

    internal static bool CanCleanUp(DependencyObject element, IContainerProvider provider)
    {
        if (ViewProviders.TryGetValue(element, out var owner))
            return ReferenceEquals(owner, provider);

        var attachedProvider = element.GetValue(ViewModelLocator.ContainerProviderProperty) as IContainerProvider;
        return attachedProvider is null || ReferenceEquals(attachedProvider, provider);
    }

    internal static void Cleanup(IRegion region, List<Exception> errors)
    {
        if (!Providers.TryGetValue(region, out var scope) || scope._closed)
            return;

        scope._closed = true;
        foreach (var cleanup in scope._cleanup)
        {
            try { cleanup(); }
            catch (Exception ex) { errors.Add(ex); }
        }
        scope._cleanup.Clear();
    }

    internal static void ConfigureView(object view, IContainerProvider provider)
    {
        VerifyProvider(provider);
        if (view is DependencyObject element)
        {
            if (ViewProviders.TryGetValue(element, out var owner) && !ReferenceEquals(owner, provider) ||
                element.GetValue(ViewModelLocator.ContainerProviderProperty) is IContainerProvider existing &&
                !ReferenceEquals(existing, provider))
                throw new InvalidOperationException(NativeResources.ViewCannotShareWindowScopes);

            // Retain the ownership identity even after attached properties are cleared
            // on close: reusing a singleton view would retain disposed constructor services.
            if (owner is null)
                ViewProviders.Add(element, provider);
            element.SetValue(ViewModelLocator.ContainerProviderProperty, provider);
        }
    }
}

using Prism.Navigation.Regions;

namespace Prism.Windows;

/// <summary>Identifies a native window and the services owned by its lifetime.</summary>
/// <remarks>
/// A context is assigned before the window's shell is resolved. Use this context, not
/// keyboard focus, to target dialogs and navigation. Access its services only on the
/// application's WinUI thread. Closing the window permanently ends its scope.
/// </remarks>
public interface IWindowContext
{
    /// <summary>Gets the window that owns this context.</summary>
    Window Window { get; }

    /// <summary>Gets this window's container, throwing after the window closes.</summary>
    IContainerProvider Container { get; }

    /// <summary>Gets this window's region manager, throwing after the window closes.</summary>
    IRegionManager RegionManager { get; }

    /// <summary>Gets whether this window's lifetime has ended.</summary>
    bool IsClosed { get; }
}

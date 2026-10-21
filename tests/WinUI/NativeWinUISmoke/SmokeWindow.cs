using Prism.Navigation.Regions;
using Prism.Windows;

namespace NativeWinUISmoke;

public sealed class SmokeWindow
{
    public SmokeWindow(IWindowContext context)
    {
        Context = context;
        Shell = (SmokeShell)context.Window.Content;
        Regions = context.RegionManager;
    }

    public IWindowContext Context { get; }
    public SmokeShell Shell { get; }
    public IRegionManager Regions { get; }
}

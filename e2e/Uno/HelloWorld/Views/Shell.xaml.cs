using Uno.Toolkit;

namespace HelloWorld.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Shell : Page, ILoadableShell
{
    public Shell()
    {
        this.InitializeComponent();
    }

    public ILoadable Source
    {
        get => Splash.Source;
        set => Splash.Source = value;
    }
}

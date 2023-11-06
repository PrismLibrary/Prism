using Uno.Toolkit;

namespace HelloWorld.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Shell : Page, ILoadableShell
{
    private readonly CompositeLoadableSource _loadable;
    public Shell()
    {
        this.InitializeComponent();
        _loadable = new ()
        {
            IsExecuting = true
        };
        Splash.Source = _loadable;
    }

    public void FinishLoading()
    {
        _loadable.IsExecuting = false;
    }
}

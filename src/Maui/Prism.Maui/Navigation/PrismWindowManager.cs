namespace Prism.Navigation;

internal sealed class PrismWindowManager : IWindowCreator, IWindowManager
{
    private IApplication _application { get; }

    public PrismWindowManager(IApplication application)
    {
        _application = application;
    }

    private Window _initialWindow;
    private readonly HashSet<Window> _trackedWindows = new();

    private Window _current;
    public Window Current => _current ?? _initialWindow;

    public IReadOnlyList<Window> Windows => _application.Windows.OfType<Window>().ToList();

    public Window CreateWindow(Application app, IActivationState activationState)
    {
        if (_initialWindow is not null)
            return _initialWindow;
        else if (app.Windows.OfType<PrismWindow>().Any())
        {
            _initialWindow = app.Windows.OfType<PrismWindow>().First();
            TrackWindow(_initialWindow);
            return _initialWindow;
        }

        activationState.Context.Services.GetRequiredService<PrismAppBuilder>().OnCreateWindow();

        return _initialWindow ?? throw new InvalidNavigationException("Expected Navigation Failed. No Root Window has been created.");
    }

    public void OpenWindow(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        TrackWindow(window);

        if (_initialWindow is null)
            _initialWindow = window;
        else
            _application.OpenWindow(window);
    }

    public void CloseWindow(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        _application.CloseWindow(window);
        ForgetWindow(window);
    }

    private void TrackWindow(Window window)
    {
        if (!_trackedWindows.Add(window))
            return;

        window.Activated += OnWindowActivated;
        window.Deactivated += OnWindowDeactivated;
        window.Destroying += OnWindowDestroying;
    }

    private void OnWindowActivated(object sender, EventArgs args)
    {
        _current = (Window)sender;
    }

    private void OnWindowDeactivated(object sender, EventArgs args)
    {
        if (ReferenceEquals(_current, sender))
            _current = null;
    }

    private void OnWindowDestroying(object sender, EventArgs args)
    {
        ForgetWindow((Window)sender);
    }

    private void ForgetWindow(Window window)
    {
        if (ReferenceEquals(_initialWindow, window))
            _initialWindow = null;

        if (ReferenceEquals(_current, window))
            _current = null;

        _trackedWindows.Remove(window);
        window.Activated -= OnWindowActivated;
        window.Deactivated -= OnWindowDeactivated;
        window.Destroying -= OnWindowDestroying;
    }
}

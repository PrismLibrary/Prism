using Prism.Behaviors;
using Prism.Extensions;

namespace Prism.Navigation;

internal sealed class PrismWindowManager : IWindowCreator, IWindowManager
{
    private IApplication _application { get; }

    public PrismWindowManager(IApplication application)
    {
        _application = application;
    }

    private Window _initialWindow;

    private Window _current;
    public Window Current => _current ?? _initialWindow;

    public IReadOnlyList<Window> Windows => _application.Windows.OfType<Window>().ToList();

    public Window CreateWindow(Application app, IActivationState activationState)
    {
        if (_initialWindow is not null)
            return _initialWindow;
        else if (app.Windows.OfType<PrismWindow>().Any())
            return _initialWindow = app.Windows.OfType<PrismWindow>().First();

        activationState.Context.Services.GetRequiredService<PrismAppBuilder>().OnCreateWindow();

        return _initialWindow ?? throw new InvalidNavigationException("Expected Navigation Failed. No Root Window has been created.");
    }

    public void OpenWindow(Window window)
    {
        if (_initialWindow is null)
            _initialWindow = window;
        else
            _application.OpenWindow(window);

        foreach(var pWindow in Windows.OfType<PrismWindow>().Where(x => x.IsActive))
        {
            pWindow.IsActive = window.Equals(pWindow);
        }
    }

    public void CloseWindow(Window window)
    {
        if (_initialWindow == window)
            _initialWindow = null;

        _application.CloseWindow(window);
    }
}

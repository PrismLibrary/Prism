using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Mocks;

public class ApplicationMock : IApplication
{
    private List<IWindow> _windows = new List<IWindow> { new Window() };

    private Window _currentWindow => _windows.Cast<Window>().FirstOrDefault();

    public ApplicationMock(Page page = null)
    {
        if (page != null)
            _currentWindow.Page = page;
    }

    public Page MainPage => _currentWindow?.Page;

    public IReadOnlyList<IWindow> Windows => _windows;
    public IElementHandler Handler { get; set; }
    public IElement Parent { get; }

    public void CloseWindow(IWindow window)
    {
        if (!_windows.Contains(window))
            throw new Exception("Application doesn't contain this window");

        _windows.Remove(window);
    }

    public IWindow CreateWindow(IActivationState activationState)
    {
        throw new NotImplementedException();
    }

    public void OpenWindow(IWindow window)
    {
        if (_windows.Contains(window))
            throw new Exception("Application already has this window");

        _windows.Add(window);
    }

    public void ThemeChanged()
    {
        throw new NotImplementedException();
    }
}

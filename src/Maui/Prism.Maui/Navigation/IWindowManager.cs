using System;
namespace Prism.Navigation;

public interface IWindowManager
{
    IReadOnlyList<Window> Windows { get; }

    void OpenWindow(Window window);
    void CloseWindow(Window window);
}


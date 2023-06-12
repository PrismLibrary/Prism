using System;
using System.ComponentModel;

namespace Prism.Mvvm;

public class ViewModelCreationException : Exception
{
    private static Func<object, string> _viewNameDelegate = null;
    private static string GetViewName(object view) => _viewNameDelegate is null ? "Platform not initialized" : _viewNameDelegate(view);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetViewNameDelegate(Func<object, string> viewNameDelegate) => _viewNameDelegate = viewNameDelegate;

    public ViewModelCreationException(object view, Exception innerException)
        : base($"Unable to Create ViewModel for '{view.GetType().FullName}'.", innerException)
    {
        View = view;
        ViewName = GetViewName(view);
    }

    public string ViewName { get; }

    public object View { get; }
}

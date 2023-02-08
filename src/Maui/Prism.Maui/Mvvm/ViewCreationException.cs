using Prism.Common;

namespace Prism.Mvvm;

public class ViewCreationException : Exception
{
    public ViewCreationException(string viewName, ViewType viewType)
        : this(viewName, viewType, null)
    {
    }

    public ViewCreationException(string viewName, ViewType viewType, Exception innerException)
        : base($"Unable to create {viewType} '{viewName}'.", innerException)
    {
        ViewName = viewName;
        ViewType = viewType;
    }

    public ViewType ViewType { get; }

    public string ViewName { get; }
}

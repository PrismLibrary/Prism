namespace Prism.Mvvm;

public class ViewModelCreationException : Exception
{
    public ViewModelCreationException(object view, Exception innerException)
        : base($"Unable to Create ViewModel for '{view.GetType().FullName}'.", innerException)
    {
        if (view is VisualElement visualElement)
        {
            View = visualElement;
            ViewName = (string)visualElement.GetValue(ViewModelLocator.NavigationNameProperty);
        }
    }

    public string ViewName { get; }

    public VisualElement View { get; }
}

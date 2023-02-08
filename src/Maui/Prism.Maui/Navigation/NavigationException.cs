namespace Prism.Navigation;

public class NavigationException : Exception
{
    public const string CannotPopApplicationMainPage = "Cannot Pop Application MainPage";
    public const string CannotGoBackFromRoot = "Cannot GoBack from NavigationPage Root.";
    public const string GoBackToRootRequiresNavigationPage = "GoBackToRootAsync can only be called when the calling Page is within a NavigationPage.";
    public const string RelativeNavigationRequiresNavigationPage = "Removing views using the relative '../' syntax while navigating is only supported within a NavigationPage";
    public const string IConfirmNavigationReturnedFalse = "IConfirmNavigation returned false";
    public const string NoPageIsRegistered = "No Page has been registered with the provided key";
    public const string ErrorCreatingPage = "An error occurred while resolving the page. This is most likely the result of invalid XAML or other type initialization exception";
    public const string UnsupportedMauiCreation = "An unsupported Maui Exception occurred. This may be due to a bug with MAUI or something that is otherwise not supported by MAUI.";
    public const string UnsupportedMauiNavigation = "An unsupported event occurred while Navigating. The attempted Navigation Stack is not supported by .NET MAUI";
    public const string ErrorCreatingViewModel = "A dependency issue occurred while resolving the ViewModel. Check the InnerException for the ContainerResolutionException";
    public const string MvvmPatternBreak = "You have referenced a View type and are likely breaking the MVVM pattern. You should never reference a View type from a ViewModel.";
    public const string UnknownException = "An unknown error occurred. You may need to specify whether to Use Modal Navigation or not.";

    public NavigationException()
        : this(UnknownException)
    {
    }

    public NavigationException(string message)
        : this(message, null, null, null)
    {
    }

    public NavigationException(string message, VisualElement view)
        : this(message, view, null)
    {
    }

    public NavigationException(string message, string navigationKey)
        : this(message, navigationKey, null, null)
    {
    }

    public NavigationException(string message, string navigationKey, Exception innerException)
        : this(message, navigationKey, null, innerException)
    {
    }

    public NavigationException(string message, VisualElement view, Exception innerException) 
        : this(message, null, view, innerException)
    {
    }

    public NavigationException(string message, string navigationKey, VisualElement view, Exception innerException) : base(message, innerException)
    {
        View = view;
        NavigationKey = navigationKey;
    }

    public VisualElement View { get; }

    public string NavigationKey { get; }
}
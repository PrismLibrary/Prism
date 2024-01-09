using System;

namespace Prism.Navigation;

/// <summary>
/// Represents errors that occurred during the navigation.
/// </summary>
public class NavigationException : Exception
{
    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when cannot pop application main page.
    /// </summary>
    public const string CannotPopApplicationMainPage = "Cannot Pop Application MainPage";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when cannot go back from root.
    /// </summary>
    public const string CannotGoBackFromRoot = "Cannot GoBack from NavigationPage Root.";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when GoBackAsync can only be called when the calling Page has been navigated.
    /// </summary>
    public const string GoBackRequiresNavigationPage = "GoBackAsync can only be called when the calling Page has been navigated.";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when GoBackToRootAsync can only be called when the calling Page is within a NavigationPage.
    /// </summary>
    public const string GoBackToRootRequiresNavigationPage = "GoBackToRootAsync can only be called when the calling Page is within a NavigationPage.";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when removing views using the relative '../' syntax while navigating is only supported within a NavigationPage.
    /// </summary>
    public const string RelativeNavigationRequiresNavigationPage = "Removing views using the relative '../' syntax while navigating is only supported within a NavigationPage";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when IConfirmNavigation returned false.
    /// </summary>
    public const string IConfirmNavigationReturnedFalse = "IConfirmNavigation returned false";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when no Page has been registered with the provided key.
    /// </summary>
    public const string NoPageIsRegistered = "No Page has been registered with the provided key";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when an error occurred while resolving the page.
    /// </summary>
    public const string ErrorCreatingPage = "An error occurred while resolving the page. This is most likely the result of invalid XAML or other type initialization exception";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when an unsupported Maui Exception occurred.
    /// </summary>
    public const string UnsupportedMauiCreation = "An unsupported Maui Exception occurred. This may be due to a bug with MAUI or something that is otherwise not supported by MAUI.";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when an unsupported event occurred while Navigating.
    /// </summary>
    public const string UnsupportedMauiNavigation = "An unsupported event occurred while Navigating. The attempted Navigation Stack is not supported by .NET MAUI";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when a dependency issue occurred while resolving the ViewModel..
    /// </summary>
    public const string ErrorCreatingViewModel = "A dependency issue occurred while resolving the ViewModel. Check the InnerException for the ContainerResolutionException";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when you have referenced a View type and are likely breaking the MVVM pattern.
    /// </summary>
    public const string MvvmPatternBreak = "You have referenced a View type and are likely breaking the MVVM pattern. You should never reference a View type from a ViewModel.";

    /// <summary>
    /// The <see cref="NavigationException"/> Message returned when an unknown error occurred.
    /// </summary>
    public const string UnknownException = "An unknown error occurred. You may need to specify whether to Use Modal Navigation or not.";

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationException"/>
    /// </summary>
    public NavigationException()
        : this(UnknownException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NavigationException(string message)
        : this(message, null, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationException"/> class with a specified error message and a view instance.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="view">The view instance.</param>
    public NavigationException(string message, object view)
        : this(message, view, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationException"/> class with a specified error message and a key used for the failed navigation.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="navigationKey">The key used for the failed navigation.</param>
    public NavigationException(string message, string navigationKey)
        : this(message, navigationKey, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationException"/> class with a specified error message, a key used for the failed navigation, and a reference
    /// to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="navigationKey">The key used for the failed navigation.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
    /// (Nothing in Visual Basic) if no inner exception is specified.</param>
    public NavigationException(string message, string navigationKey, Exception innerException)
        : this(message, navigationKey, null, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationException"/> class with a specified error message, a view instance and a reference
    /// to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="view">The view instance.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
    /// (Nothing in Visual Basic) if no inner exception is specified.</param>
    public NavigationException(string message, object view, Exception innerException)
        : this(message, null, view, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationException"/> class with a specified error message, a key used for the failed navigation, a view instance,
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="navigationKey">The key used for the failed navigation.</param>
    /// <param name="view">The view instance.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
    /// (Nothing in Visual Basic) if no inner exception is specified.</param>
    public NavigationException(string message, string navigationKey, object view, Exception innerException) : base(message, innerException)
    {
        View = view;
        NavigationKey = navigationKey;
    }

    /// <summary>
    /// The View Instance
    /// </summary>
    public object View { get; }

    /// <summary>
    /// The key used for the failed navigation
    /// </summary>
    public string NavigationKey { get; }
}

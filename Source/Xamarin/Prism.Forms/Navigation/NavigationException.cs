using System;
using Xamarin.Forms;

namespace Prism.Navigation
{
    public class NavigationException : Exception
    {
        public const string CannotPopApplicationMainPage = "Cannot Pop Application MainPage";
        public const string CannotGoBackFromRoot = "Cannot GoBack from NavigationPage Root.";
        public const string GoBackToRootRequiresNavigationPage = "GoBackToRootAsync can only be called when the calling Page is within a NavigationPage.";
        public const string RelativeNavigationRequiresNavigationPage = "Removing views using the relative '../' syntax while navigating is only supported within a NavigationPage";
        public const string IConfirmNavigationReturnedFalse = "IConfirmNavigation returned false";
        public const string NoPageIsRegistered = "No Page has been registered with the provided key";
        public const string ErrorCreatingPage = "An error occurred while resolving the page. This is most likely the result of invalid XAML or other type initialization exception";
        public const string UnknownException = "An unknown error occurred. You may need to specify whether to Use Modal Navigation or not.";

        public NavigationException()
        {
        }

        public NavigationException(string message, Page page) : this(message, page, null)
        {
        }

        public NavigationException(string message, Page page, Exception innerException) : base(message, innerException)
        {
            Page = page;
        }

        public Page Page { get; }
    }
}

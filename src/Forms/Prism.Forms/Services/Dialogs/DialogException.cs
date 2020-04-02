using System;

namespace Prism.Services.Dialogs
{
    public class DialogException : Exception
    {
        public const string ShowDialog = "Error while displaying dialog";

        public const string RequiresContentPage = "The current page must be a ContentPage";

        public const string HostPageIsNotDialogHost = "The current page is not currently hosting a Dialog";

        public const string CanCloseIsFalse = "CanClose returned false";

        public const string NoViewModel = "No ViewModel could be found";

        public const string ImplementIDialogAware = "The ViewModel does not implement IDialogAware";

        public DialogException(string message) : base(message)
        {
        }
    }
}

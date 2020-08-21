using Xamarin.Forms;

namespace Prism.Services.Dialogs
{
    internal class DialogPage : ContentPage
    {
        public DialogPage()
        {
            AutomationId = "PrismDialogModal";
            BackgroundColor = Color.Transparent;
        }

        public View DialogView { get; set; }
    }
}

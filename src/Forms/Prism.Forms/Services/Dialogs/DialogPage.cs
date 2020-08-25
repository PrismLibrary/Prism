using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Prism.Services.Dialogs
{
    internal class DialogPage : ContentPage
    {
        public DialogPage()
        {
            AutomationId = "PrismDialogModal";
            BackgroundColor = Color.Transparent;
            On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.OverFullScreen);
        }

        public View DialogView { get; set; }
    }
}

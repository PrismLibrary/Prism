using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Prism.Services.Dialogs
{
    public interface IDialogContainer
    {
        event EventHandler<IDialogResult> DialogResult;
        View DialogView { get; }
        void RaiseDialogResult(IDialogResult result);
        ICommand Dismiss { get; set; }
    }

    internal static class IDialogContainerExtensions
    {
        public static Task<IDialogResult> RequestCloseAsync(this IDialogContainer container)
        {
            var tcs = new TaskCompletionSource<IDialogResult>();
            void OnDialogResult(object sender, IDialogResult result)
            {
                container.DialogResult -= OnDialogResult;
                tcs.SetResult(result);
            }

            container.DialogResult += OnDialogResult;
            container.Dismiss.Execute(null);
            return tcs.Task;
        }
    }
}

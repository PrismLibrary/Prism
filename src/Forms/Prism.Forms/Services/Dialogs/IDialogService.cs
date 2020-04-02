using System;
using System.Threading.Tasks;

namespace Prism.Services.Dialogs
{
    public interface IDialogService
    {
        void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback);
    }
}
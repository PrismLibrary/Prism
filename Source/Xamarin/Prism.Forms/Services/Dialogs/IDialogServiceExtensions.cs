using System;
using System.Linq;
using System.Reflection;

namespace Prism.Services.Dialogs
{
    public static class IDialogServiceExtensions
    {
        public static void ShowDialog(this IDialogService dialogService, string name) =>
            dialogService.ShowDialog(name, null, null);
        public static void ShowDialog(this IDialogService dialogService, string name, Action<IDialogResult> callback) =>
            dialogService.ShowDialog(name, null, callback);
        public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters) =>
            dialogService.ShowDialog(name, parameters, null);
    }
}
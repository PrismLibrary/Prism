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

        internal static (string Name, bool IsRequired) GetAutoInitializeProperty(this PropertyInfo pi)
        {
            var attr = pi.GetCustomAttribute<DialogParameterAttribute>();
            if (attr is null)
            {
                return (pi.Name, false);
            }

            return (string.IsNullOrEmpty(attr.Name) ? pi.Name : attr.Name, attr.IsRequired);
        }

        internal static bool HasKey(this IDialogParameters parameters, string name, out string key)
        {
            key = parameters.Keys.FirstOrDefault(k => k.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return !string.IsNullOrEmpty(key);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.ShowDialog
{
    public static class ShowDialogRequestExtensions
    {
        public static void RaiseNotification(this ShowDialogRequest<INotification> request, string title, string message, Action callback)
        {
            request.Raise(title, new Notification { Content = message }, (n) => callback());
        }

        public static void RaiseConfirmation(this ShowDialogRequest<IConfirmation> request, string title, string message, Action<bool> callback)
        {
            request.Raise(title, new Confirmation { Content = message }, (c) => callback(c.Confirmed));
        }

        public static void RaiseYesNoCancel(this ShowDialogRequest<IYesNoCancel> request, string title, string message, Action<bool?> callback)
        {
            request.Raise(title, new YesNoCancel { Content = message }, (c) => callback(c.Confirmed));
        }
    }
}

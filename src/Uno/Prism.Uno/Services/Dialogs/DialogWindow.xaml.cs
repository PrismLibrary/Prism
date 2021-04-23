using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

#if HAS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#elif HAS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endif

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Prism's default dialog host.
    /// </summary>
    public partial class DialogWindow : ContentDialog, IDialogWindow
    {
        public DialogWindow()
        {
            this.InitializeComponent();
        }
        public IDialogResult Result { get; set; }

        event RoutedEventHandler IDialogWindow.Loaded
        {
            add => Loaded += value;
            remove => Loaded -= value;
        }

        event TypedEventHandler<ContentDialog, ContentDialogClosingEventArgs> IDialogWindow.Closing
        {
            add => Closing += value;
            remove => Closing -= value;
        }

        event TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> IDialogWindow.Closed
        {
            add => Closed += value;
            remove => Closed -= value;
        }
    }
}

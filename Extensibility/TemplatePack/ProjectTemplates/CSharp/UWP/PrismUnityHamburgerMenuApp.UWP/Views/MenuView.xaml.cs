using $safeprojectname$.PageViewModels;
using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace $safeprojectname$.Views
{
    public sealed partial class MenuView : UserControl, INotifyPropertyChanged
    {
        public MenuView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MenuViewModel ConcreteDataContext
        {
            get
            {
                return DataContext as MenuViewModel;
            }
        }
        
        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConcreteDataContext)));
        }
    }
}

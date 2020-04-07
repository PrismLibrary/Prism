using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml.Controls;

namespace HelloUnoWorld.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class ViewA : UserControl, INavigationAware
    {
        public ViewA()
        {
            InitializeComponent();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
         
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
         
        }
    }
}

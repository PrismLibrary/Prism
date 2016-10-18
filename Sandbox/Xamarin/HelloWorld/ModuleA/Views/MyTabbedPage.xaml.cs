using System;
using Xamarin.Forms;

namespace ModuleA.Views
{
    public partial class MyTabbedPage : TabbedPage, IDisposable
    {
        public MyTabbedPage()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            
        }
    }
}

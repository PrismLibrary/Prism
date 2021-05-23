using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prism.DI.Forms.Tests.Mocks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Issue2415Page : ContentPage
    {
        public Issue2415Page()
        {
            InitializeComponent();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HelloWorld.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            //BUG:  Navigating Modal with animated = false does not prevent animated transition
            //var p1 = new ViewA();
            //var p2 = new ViewB();
            //var p3 = new ViewC();
            //var p4 = new MyTabbedPage();

            //p3.Navigation.PushModalAsync(p4, false);
            //p2.Navigation.PushModalAsync(p3, false);
            //p1.Navigation.PushModalAsync(p2, false);
            //Navigation.PushModalAsync(p1, false);

            //BUG:  TabbedPage tab header are not shown, only first page in TabbedPage children is shown.
            var p1 = new MyNavigationPage();
            var p2 = new ViewA();
            var p3 = new ViewC();
            var p4 = new MyTabbedPage();

            p3.Navigation.PushAsync(p4, false);
            p2.Navigation.PushAsync(p3, false);
            p1.Navigation.PushAsync(p2, false);
            Navigation.PushModalAsync(p1, false);


            //BUG: Results in the MyTabbedPage tab headers showing on ViewC. Hit the back button, pops to ViewB, then ViewA, then to the TabbedPage page as expected.
            //var p1 = new MyNavigationPage();
            //var p2 = new MyTabbedPage();
            //var p3 = new ViewA();
            //var p4 = new ViewB();
            //var p5 = new ViewC();

            //p4.Navigation.PushAsync(p5, false);
            //p3.Navigation.PushAsync(p4, false);
            //p2.Navigation.PushAsync(p3, false);
            //p1.Navigation.PushAsync(p2, false);
            //Navigation.PushModalAsync(p1, false);
        }
    }
}

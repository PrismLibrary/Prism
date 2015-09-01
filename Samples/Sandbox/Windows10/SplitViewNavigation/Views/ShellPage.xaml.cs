using System.Diagnostics;
using SplitViewNavigation.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SplitViewNavigation.Views
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootFrame"></param>
        /// <param name="viewModel"></param>
        public ShellPage(Frame rootFrame, ShellPageViewModel viewModel)
        {
            Debug.WriteLine("ShellPage()");
            this.InitializeComponent();
            this.DataContext = viewModel;
            this.RootSplitView.Content = rootFrame;

            //< Frame.ContentTransitions >
            //    < TransitionCollection >
            //        < NavigationThemeTransition >
            //            < NavigationThemeTransition.DefaultNavigationTransitionInfo >
            //                < EntranceNavigationTransitionInfo />
            //            </ NavigationThemeTransition.DefaultNavigationTransitionInfo >
            //        </ NavigationThemeTransition >
            //    </ TransitionCollection >
            //</ Frame.ContentTransitions >
            //Debug.WriteLine(DisplayInformation.GetForCurrentView().ResolutionScale);
            //Debug.WriteLine(DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel);
            this.ScreenSize.Text = string.Format("{0},{1}",
                Window.Current.Bounds.Width.ToString("0.00"),
                Window.Current.Bounds.Height.ToString("0.00"));

            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, global::Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.ScreenSize.Text = string.Format("{0},{1}",
                Window.Current.Bounds.Width.ToString("0.00"),
                Window.Current.Bounds.Height.ToString("0.00"));
        }
    }
}

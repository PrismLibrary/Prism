using System;
using Uno.UI;
using Windows.UI.Xaml;

namespace HelloUnoWorld.Wasm
{
    public class Program
    {
        private static App _app;

        static int Main(string[] args)
        {
            // Enable x:Name to be mapped to html DOM elements
            FeatureConfiguration.UIElement.AssignDOMXamlName = true;

            Windows.UI.Xaml.Application.Start(_ => _app = new App());

            return 0;
        }
    }
}

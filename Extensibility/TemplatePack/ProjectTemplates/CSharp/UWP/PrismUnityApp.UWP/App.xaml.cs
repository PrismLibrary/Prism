using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;

namespace $safeprojectname$
{
    public sealed partial class App : PrismUnityApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName;
                if (viewName.Contains(".Views."))
                {
                    viewName = viewName.Replace(".Views.", ".PageViewModels.");
                }
                else
                {
                    throw new ArgumentException($"The specified View type {viewName} isn't in the Views namespace.");
                }

                //// TODO: Uncomment this if ViewModels are in a separate project
                ////var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName.Replace("$safeprojectname$", "$safeprojectname$.ViewModel");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelName = $"{viewName}{suffix}, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });

            return base.OnInitializeAsync(args);
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate(nameof(PageTokens.Main), null);
            return Task.FromResult(true);
        }
    }
}
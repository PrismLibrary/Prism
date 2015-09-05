using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.ApplicationSettings;
using HelloWorld.Services;
using HelloWorld.ViewModels;
using HelloWorld.Views;
using Prism.Mvvm;
using Prism.Windows;

namespace HelloWorld
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : PrismApplication
    {
        // Declare any app services that you want to hold on to as singletons
        IDataRepository _dataRepository;
        public App() : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Required override. Generally you do your initial navigation to launch page, or 
        /// to the page approriate based on a search, sharing, or secondary tile launch of the app
        /// </summary>
        /// <param name="args">The launch arguments passed to the application</param>
        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            // Use the logical name for the view to navigate to. The default convention
            // in the NavigationService will be to append "Page" to the name and look 
            // for that page in a .Views child namespace in the project. IF you want another convention
            // for mapping view names to view types, you can override 
            // the MvvmAppBase.GetPageNameToTypeResolver method
            NavigationService.Navigate("Main", null);
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// This is the place you initialize your services and set default factory or default resolver for the view model locator
        /// </summary>
        /// <param name="args">The same launch arguments passed when the app starts.</param>
        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // New up the singleton data repository, and pass it the state service it depends on from the base class
            _dataRepository = new DataRepository(SessionStateService);

            // Register factory methods for the ViewModelLocator for each view model that takes dependencies so that you can pass in the
            // dependent services from the factory method here.
            ViewModelLocationProvider.Register(typeof(MainPage).ToString(), () => new MainPageViewModel(_dataRepository, NavigationService));
            ViewModelLocationProvider.Register(typeof(UserInputPage).ToString(), () => new UserInputPageViewModel(_dataRepository, NavigationService));

            return base.OnInitializeAsync(args);
        }
    }
}

using Prism.Navigation;
using Xamarin.Forms;

namespace Prism
{
    public abstract class Bootstrapper
    {
        protected Application App { get; set; }

        public INavigationService NavigationService { get; set; }

        /// <summary>
        /// Runs the bootstrapper process.
        /// </summary>
        public void Run(Application app)
        {
            App = app;
            ConfigureViewModelLocator();
            Run();
        }

        public abstract void Run();

        protected abstract Page CreateMainPage();

        protected abstract void ConfigureViewModelLocator();

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="Microsoft.Practices.ServiceLocation.ServiceLocator" />.
        /// </summary>
        protected abstract void ConfigureServiceLocator();

        protected abstract void RegisterTypes();
    }
}

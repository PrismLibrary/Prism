using Microsoft.Practices.Unity;
using ModuleA.Views;
using Prism.Modularity;
using Prism.Unity;
using Xamarin.Forms;

namespace ModuleA
{
    public class ModuleAModule : IModule
    {
        readonly IUnityContainer _container;

        public ModuleAModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterTypeForNavigation<MyTabbedPage>();
            _container.RegisterTypeForNavigation<ViewA>();
            _container.RegisterTypeForNavigation<ViewB>();
            _container.RegisterTypeForNavigation<ViewC>();

            _container.RegisterType<IApplicationCommands, ApplicationCommands>(new ContainerControlledLifetimeManager());

            //var masterDetail = PrismApplication.Current.MainPage as MasterDetailPage;
            //if (masterDetail != null)
            //    masterDetail.Master = new MasterNavigation();
        }
    }
}

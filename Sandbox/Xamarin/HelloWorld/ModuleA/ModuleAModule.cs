using ModuleA.Views;
using Prism.Ioc;
using Prism.Modularity;
using Xamarin.Forms;

namespace ModuleA
{
    public class ModuleAModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //var masterDetail = PrismApplication.Current.MainPage as MasterDetailPage;
            //if (masterDetail != null)
            //    masterDetail.Master = new MasterNavigation();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MyTabbedPage>();
            containerRegistry.RegisterForNavigation<ViewA>();
            containerRegistry.RegisterForNavigation<ViewB>();
            containerRegistry.RegisterForNavigation<ViewC>();
            containerRegistry.RegisterForNavigation<MyCarouselPage>();

            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
        }
    }
}

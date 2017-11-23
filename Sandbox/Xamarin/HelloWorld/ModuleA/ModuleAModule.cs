using ModuleA.Views;
using Prism.Ioc;
using Prism.Modularity;
using Xamarin.Forms;

namespace ModuleA
{
    public class ModuleAModule : IModule
    {
        public void Initialize()
        {
            //var masterDetail = PrismApplication.Current.MainPage as MasterDetailPage;
            //if (masterDetail != null)
            //    masterDetail.Master = new MasterNavigation();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterTypeForNavigation<MyTabbedPage>();
            containerRegistry.RegisterTypeForNavigation<ViewA>();
            containerRegistry.RegisterTypeForNavigation<ViewB>();
            containerRegistry.RegisterTypeForNavigation<ViewC>();

            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
        }
    }
}

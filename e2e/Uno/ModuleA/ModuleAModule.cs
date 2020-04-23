using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Ioc;
using Prism.Modularity;

namespace ModuleA
{
    public class ModuleAModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ModulePageA>();

            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
        }
    }
}

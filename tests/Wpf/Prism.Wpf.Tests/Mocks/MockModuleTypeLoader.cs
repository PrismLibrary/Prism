

using System;
using System.Collections.Generic;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Mocks
{
    public class MockModuleTypeLoader : IModuleTypeLoader
    {        
        public List<IModuleInfo> LoadedModules = new List<IModuleInfo>();
        public bool canLoadModuleTypeReturnValue = true;
        public Exception LoadCompletedError;

        public bool CanLoadModuleType(IModuleInfo moduleInfo)
        {
            return canLoadModuleTypeReturnValue;
        }

        public void LoadModuleType(IModuleInfo moduleInfo)
        {
            this.LoadedModules.Add(moduleInfo);
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, this.LoadCompletedError));
        }

        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        public void RaiseLoadModuleProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            this.ModuleDownloadProgressChanged?.Invoke(this, e);
        }

        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        public void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        public void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            this.LoadModuleCompleted?.Invoke(this, e);
        }
    }
}

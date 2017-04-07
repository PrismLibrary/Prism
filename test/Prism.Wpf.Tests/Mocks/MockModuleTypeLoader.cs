

using System;
using System.Collections.Generic;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Mocks
{
    public class MockModuleTypeLoader : IModuleTypeLoader
    {        
        public List<ModuleInfo> LoadedModules = new List<ModuleInfo>();
        public bool canLoadModuleTypeReturnValue = true;
        public Exception LoadCompletedError;

        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            return canLoadModuleTypeReturnValue;
        }

        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            this.LoadedModules.Add(moduleInfo);
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, this.LoadCompletedError));
        }

        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        public void RaiseLoadModuleProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            if (this.ModuleDownloadProgressChanged != null)
            {
                this.ModuleDownloadProgressChanged(this, e);
            }
        }

        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        public void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        public void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            if (this.LoadModuleCompleted != null)
            {                
                this.LoadModuleCompleted(this, e);
            }
        }
    }
}

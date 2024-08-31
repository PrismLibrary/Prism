using System;
using System.Collections.Generic;
using Prism.Modularity;

namespace Prism.Avalonia.Tests.Mocks
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
            LoadedModules.Add(moduleInfo);
            RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, LoadCompletedError));
        }

        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        public void RaiseLoadModuleProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            ModuleDownloadProgressChanged?.Invoke(this, e);
        }

        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        public void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        public void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            LoadModuleCompleted?.Invoke(this, e);
        }
    }
}

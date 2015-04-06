// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Prism.Mef.Modularity;
using Prism.Modularity;

namespace Prism.Mef.Wpf.Tests.Support
{
    [ModuleExport("MefModuleOne", typeof(MefModuleOne))]
    public class MefModuleOne : IModule
    {
        public bool WasInitialized = false;
        public void Initialize()
        {
            WasInitialized = true;
        }
    }
}

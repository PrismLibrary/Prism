// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Prism.Modularity;

namespace Prism.IocContainer.Wpf.Tests.Support.Mocks
{
    public class MockModuleInitializer : IModuleInitializer
    {
        public bool LoadCalled;

        public void Initialize(ModuleInfo moduleInfo)
        {
            LoadCalled = true;
        }
    }
}
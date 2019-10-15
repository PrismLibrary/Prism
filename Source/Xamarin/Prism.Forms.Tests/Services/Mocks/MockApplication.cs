using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Services.Mocks.Dialogs;
using Prism.Forms.Tests.Services.Mocks.Ioc;
using Prism.Ioc;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Services.Mocks
{
    public class MockApplication : Application, IApplicationProvider
    {
        public MockApplication()
        {
            Container = new DialogContainerExtension();
            RegisterTypes(Container);
        }

        public IContainerExtension Container { get; }

        private void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<DialogMock, DialogMockViewModel>();
        }
    }
}

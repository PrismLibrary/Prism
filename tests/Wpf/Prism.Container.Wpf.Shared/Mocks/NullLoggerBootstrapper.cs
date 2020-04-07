using System;
using System.Windows;
using Prism.Logging;

namespace Prism.Container.Wpf.Mocks
{
    internal partial class NullLoggerBootstrapper
    {
        protected override ILoggerFacade CreateLogger()
        {
            return null;
        }

        protected override DependencyObject CreateShell()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeShell()
        {
            throw new NotImplementedException();
        }
    }
}

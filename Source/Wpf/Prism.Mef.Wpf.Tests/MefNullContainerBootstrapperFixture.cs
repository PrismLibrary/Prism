// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Mef;

namespace Prism.Wpf.Mef.Tests
{
    [TestClass]
    public class MefNullContainerBootstrapperFixture : BootstrapperFixtureBase
    {
        [TestMethod]
        public void RunThrowsWhenNullContainerCreated()
        {
            var bootstrapper = new NullContainerBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "CompositionContainer");
        }

        internal class NullContainerBootstrapper : MefBootstrapper
        {
            protected override CompositionContainer CreateContainer()
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
}
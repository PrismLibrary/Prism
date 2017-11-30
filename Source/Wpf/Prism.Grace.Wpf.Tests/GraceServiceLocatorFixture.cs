using CommonServiceLocator;
using Grace.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Prism.Grace.Wpf.Tests
{
    [TestClass]
    public class GraceServiceLocatorFixture
    {
        [TestMethod]
        public void ShouldForwardResolveToInnerContainer()
        {
            object myInstance = new object();

            DependencyInjectionContainer container = new DependencyInjectionContainer();
            container.Configure(c => c.ExportInstance<object>(myInstance));

            IServiceLocator containerAdapter = new GraceServiceLocatorAdapter(container);

            Assert.AreSame(myInstance, containerAdapter.GetInstance(typeof (object)));
        }
    }
}
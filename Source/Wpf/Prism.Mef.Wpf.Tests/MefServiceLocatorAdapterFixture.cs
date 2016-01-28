

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prism.Mef.Wpf.Tests
{
    [TestClass]
    public class MefServiceLocatorAdapterFixture
    {
        [TestMethod]
        public void ShouldForwardResolveToInnerContainer()
        {
            object myInstance = new object();

            CompositionContainer compositionContainer = new CompositionContainer();
            compositionContainer.ComposeExportedValue<object>(myInstance);

            IServiceLocator containerAdapter = new MefServiceLocatorAdapter(compositionContainer);

            Assert.AreSame(myInstance, containerAdapter.GetInstance(typeof(object)));
        }

        [TestMethod]
        public void ShouldForwardResolveToInnerContainerWhenKeyIsUsed()
        {
            object myInstance = new object();

            CompositionContainer compositionContainer = new CompositionContainer();
            compositionContainer.ComposeExportedValue<object>("key", myInstance);

            IServiceLocator containerAdapter = new MefServiceLocatorAdapter(compositionContainer);

            Assert.AreSame(myInstance, containerAdapter.GetInstance(typeof(object), "key"));
        }

        [TestMethod]
        public void ShouldForwardResolveAllToInnerContainer()
        {
            object objectOne = new object();
            object objectTwo = new object();

            CompositionContainer compositionContainer = new CompositionContainer();
            compositionContainer.ComposeExportedValue<object>(objectOne);
            compositionContainer.ComposeExportedValue<object>(objectTwo);

            IServiceLocator containerAdapter = new MefServiceLocatorAdapter(compositionContainer);
            IList<object> returnedList = containerAdapter.GetAllInstances(typeof(object)).ToList();
            Assert.AreSame(returnedList[0], objectOne);
            Assert.AreSame(returnedList[1], objectTwo);
        }

        [TestMethod]
        public void ShouldThrowActivationExceptionWhenMoreThanOneInstanceAvailble()
        {
            const int SEQUENCE_MORE_THAN_ONE_ELEMENT_HRESULT = -2146233079;

            object myInstance = new object();
            object myInstance2 = new object();

            CompositionContainer compositionContainer = new CompositionContainer();
            compositionContainer.ComposeExportedValue<object>(myInstance);
            compositionContainer.ComposeExportedValue<object>(myInstance2);

            IServiceLocator containerAdapter = new MefServiceLocatorAdapter(compositionContainer);
            try
            {
                containerAdapter.GetInstance(typeof(object));
                Assert.Fail("Expected exception not thrown.");
            }
            catch (ActivationException ex)
            {
                Assert.AreEqual(typeof(InvalidOperationException), ex.InnerException.GetType());
                Assert.IsTrue(ex.InnerException.HResult == SEQUENCE_MORE_THAN_ONE_ELEMENT_HRESULT);
            }
        }
    }
}

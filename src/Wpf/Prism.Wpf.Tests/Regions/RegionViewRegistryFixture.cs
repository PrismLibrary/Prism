

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class RegionViewRegistryFixture
    {
        [TestMethod]
        public void CanRegisterContentAndRetrieveIt()
        {
            MockServiceLocator locator = new MockServiceLocator();
            Type calledType = null;
            locator.GetInstance = (type) =>
                                      {
                                          calledType = type;
                                          return new MockContentObject();
                                      };
            var registry = new RegionViewRegistry(locator);

            registry.RegisterViewWithRegion("MyRegion", typeof(MockContentObject));
            var result = registry.GetContents("MyRegion");

            Assert.AreEqual(typeof(MockContentObject), calledType);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.IsInstanceOfType(result.ElementAt(0), typeof(MockContentObject));
        }

        [TestMethod]
        public void ShouldRaiseEventWhenAddingContent()
        {
            var listener = new MySubscriberClass();
            MockServiceLocator locator = new MockServiceLocator();
            locator.GetInstance = (type) => new MockContentObject();
            var registry = new RegionViewRegistry(locator);

            registry.ContentRegistered += listener.OnContentRegistered;

            registry.RegisterViewWithRegion("MyRegion", typeof(MockContentObject));

            Assert.IsNotNull(listener.onViewRegisteredArguments);
            Assert.IsNotNull(listener.onViewRegisteredArguments.GetView);

            var result = listener.onViewRegisteredArguments.GetView();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(MockContentObject));
        }

        [TestMethod]
        public void CanRegisterContentAsDelegateAndRetrieveIt()
        {
            var registry = new RegionViewRegistry(null);
            var content = new MockContentObject();

            registry.RegisterViewWithRegion("MyRegion", () => content);
            var result = registry.GetContents("MyRegion");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreSame(content, result.ElementAt(0));
        }

        [TestMethod]
        public void ShouldNotPreventSubscribersFromBeingGarbageCollected()
        {
            var registry = new RegionViewRegistry(null);
            var subscriber = new MySubscriberClass();
            registry.ContentRegistered += subscriber.OnContentRegistered;

            WeakReference subscriberWeakReference = new WeakReference(subscriber);

            subscriber = null;
            GC.Collect();

            Assert.IsFalse(subscriberWeakReference.IsAlive);
        }

        [TestMethod]
        public void OnRegisterErrorShouldGiveClearException()
        {
            var registry = new RegionViewRegistry(null);
            registry.ContentRegistered += new EventHandler<ViewRegisteredEventArgs>(FailWithInvalidOperationException);

            try
            {
                registry.RegisterViewWithRegion("R1", typeof(object));
                Assert.Fail();
            }
            catch (ViewRegistrationException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Dont do this"));
                Assert.IsTrue(ex.Message.Contains("R1"));
                Assert.AreEqual(ex.InnerException.Message, "Dont do this");
            }
            catch(Exception)
            {
                Assert.Fail("Wrong exception type");
            }

        }


        [TestMethod]
        public void OnRegisterErrorShouldSkipFrameworkExceptions()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof (FrameworkException));
            var registry = new RegionViewRegistry(null);
            registry.ContentRegistered +=new EventHandler<ViewRegisteredEventArgs>(FailWithFrameworkException);

            try
            {
                registry.RegisterViewWithRegion("R1", typeof (object));
                Assert.Fail();
            }
            catch (ViewRegistrationException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Dont do this"));
                Assert.IsTrue(ex.Message.Contains("R1"));
            }
            catch (Exception)
            {
                Assert.Fail("Wrong exception type");
            }
        }

        public void FailWithFrameworkException(object sender, ViewRegisteredEventArgs e)
        {
            try
            {
                FailWithInvalidOperationException(sender, e);
            }
            catch (Exception ex)
            {

                throw new FrameworkException(ex);
            }
        }

        public void FailWithInvalidOperationException(object sender, ViewRegisteredEventArgs e)
        {
            throw new InvalidOperationException("Dont do this");
        }

        public class MockContentObject
        {
        }


        public class MySubscriberClass
        {
            public ViewRegisteredEventArgs onViewRegisteredArguments;
            public void OnContentRegistered(object sender, ViewRegisteredEventArgs e)
            {
                onViewRegisteredArguments = e;
            }
        }

        class FrameworkException : Exception
        {
            public FrameworkException(Exception innerException)
                : base("", innerException)
            {

            }
        }

    }
}

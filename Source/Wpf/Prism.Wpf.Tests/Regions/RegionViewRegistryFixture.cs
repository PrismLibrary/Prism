

using System;
using System.Linq;
using Xunit;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    
    public class RegionViewRegistryFixture
    {
        [Fact]
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

            Assert.Equal(typeof(MockContentObject), calledType);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.IsType<MockContentObject>(result.ElementAt(0));
        }

        [Fact]
        public void ShouldRaiseEventWhenAddingContent()
        {
            var listener = new MySubscriberClass();
            MockServiceLocator locator = new MockServiceLocator();
            locator.GetInstance = (type) => new MockContentObject();
            var registry = new RegionViewRegistry(locator);

            registry.ContentRegistered += listener.OnContentRegistered;

            registry.RegisterViewWithRegion("MyRegion", typeof(MockContentObject));

            Assert.NotNull(listener.onViewRegisteredArguments);
            Assert.NotNull(listener.onViewRegisteredArguments.GetView);

            var result = listener.onViewRegisteredArguments.GetView();
            Assert.NotNull(result);
            Assert.IsType<MockContentObject>(result);
        }

        [Fact]
        public void CanRegisterContentAsDelegateAndRetrieveIt()
        {
            var registry = new RegionViewRegistry(null);
            var content = new MockContentObject();

            registry.RegisterViewWithRegion("MyRegion", () => content);
            var result = registry.GetContents("MyRegion");

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Same(content, result.ElementAt(0));
        }

        [Fact]
        public void ShouldNotPreventSubscribersFromBeingGarbageCollected()
        {
            var registry = new RegionViewRegistry(null);
            var subscriber = new MySubscriberClass();
            registry.ContentRegistered += subscriber.OnContentRegistered;

            WeakReference subscriberWeakReference = new WeakReference(subscriber);

            subscriber = null;
            GC.Collect();

            Assert.False(subscriberWeakReference.IsAlive);
        }

        [Fact]
        public void OnRegisterErrorShouldGiveClearException()
        {
            var registry = new RegionViewRegistry(null);
            registry.ContentRegistered += new EventHandler<ViewRegisteredEventArgs>(FailWithInvalidOperationException);

            try
            {
                registry.RegisterViewWithRegion("R1", typeof(object));
                //Assert.Fail();
            }
            catch (ViewRegistrationException ex)
            {
                Assert.Contains("Dont do this", ex.Message);
                Assert.Contains("R1", ex.Message);
                Assert.Equal("Dont do this", ex.InnerException.Message);
            }
            catch(Exception)
            {
                //Assert.Fail("Wrong exception type");
            }

        }


        [Fact]
        public void OnRegisterErrorShouldSkipFrameworkExceptions()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof (FrameworkException));
            var registry = new RegionViewRegistry(null);
            registry.ContentRegistered +=new EventHandler<ViewRegisteredEventArgs>(FailWithFrameworkException);

            try
            {
                registry.RegisterViewWithRegion("R1", typeof (object));
                //Assert.Fail();
            }
            catch (ViewRegistrationException ex)
            {
                Assert.Contains("Dont do this", ex.Message);
                Assert.Contains("R1", ex.Message);
            }
            catch (Exception)
            {
                //Assert.Fail("Wrong exception type");
            }
        }

        private void FailWithFrameworkException(object sender, ViewRegisteredEventArgs e)
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

        private void FailWithInvalidOperationException(object sender, ViewRegisteredEventArgs e)
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

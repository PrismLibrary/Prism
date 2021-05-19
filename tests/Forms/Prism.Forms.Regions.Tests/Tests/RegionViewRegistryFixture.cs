using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Prism.Ioc;
using Prism.Regions;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Regions.Tests
{
    public class RegionViewRegistryFixture
    {
        [Fact]
        public void CanRegisterContentAndRetrieveIt()
        {
            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(c => c.Resolve(typeof(MockContentObject))).Returns(new MockContentObject());
            var registry = new RegionViewRegistry(containerMock.Object);

            registry.RegisterViewWithRegion("MyRegion", typeof(MockContentObject));
            var result = registry.GetContents("MyRegion");

            //Assert.Equal(typeof(MockContentObject), calledType);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.IsType<MockContentObject>(result.ElementAt(0));
        }

        [Fact]
        public void ShouldRaiseEventWhenAddingContent()
        {
            var listener = new MySubscriberClass();
            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(c => c.Resolve(typeof(MockContentObject))).Returns(new MockContentObject());
            var registry = new RegionViewRegistry(containerMock.Object);

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
        public async Task ShouldNotPreventSubscribersFromBeingGarbageCollected()
        {
            var registry = new RegionViewRegistry(null);
            var subscriber = new MySubscriberClass();
            registry.ContentRegistered += subscriber.OnContentRegistered;

            WeakReference subscriberWeakReference = new WeakReference(subscriber);

            subscriber = null;

            await Task.Delay(50);
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
            catch (Exception)
            {
                //Assert.Fail("Wrong exception type");
            }
        }

        [Fact]
        public void OnRegisterErrorShouldSkipFrameworkExceptions()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(FrameworkException));
            var registry = new RegionViewRegistry(null);
            registry.ContentRegistered += new EventHandler<ViewRegisteredEventArgs>(FailWithFrameworkException);
            var ex = Record.Exception(() => registry.RegisterViewWithRegion("R1", typeof(object)));
            Assert.NotNull(ex);
            Assert.IsType<ViewRegistrationException>(ex);
            Assert.Contains("Dont do this", ex.Message);
            Assert.Contains("R1", ex.Message);
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

        private class MockContentObject : VisualElement
        {
        }

        private class MySubscriberClass
        {
            public ViewRegisteredEventArgs onViewRegisteredArguments;
            public void OnContentRegistered(object sender, ViewRegisteredEventArgs e)
            {
                onViewRegisteredArguments = e;
            }
        }

        private class FrameworkException : Exception
        {
            public FrameworkException(Exception innerException)
                : base("", innerException)
            {

            }
        }

    }
}

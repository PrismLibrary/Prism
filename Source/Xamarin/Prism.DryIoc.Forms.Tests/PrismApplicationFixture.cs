using System.Reflection;
using DryIoc;
using Prism.DryIoc.Forms.Tests.Services;
using Xunit;

namespace Prism.DryIoc.Forms.Tests
{
    public class PrismApplicationFixture
    {
        [Fact]
        public void OnInitialized()
        {
            var app = new PrismApplicationMock();
            Assert.True(app.Initialized);
        }

        [Fact]
        public void DryIoc_ResolveTypeRegisteredWithContainer()
        {
            var app = new PrismApplicationMock();
            var service = app.Container.Resolve<IDryIocServiceMock>();
            Assert.NotNull(service);
            Assert.IsType<DryIocServiceMock>(service);
        }

        [Fact]
        public void DryIoc_ResolveTypeRegisteredWithDependencyService()
        {
            var app = new PrismApplicationMock();
            // TODO(joacar)
            // Since we must call Xamarin.Forms.Init() (and cannot do so from PCL)
            // to perform valid calls to Xamarin.Forms.DependencyService
            // we check that this throws an InvalidOperationException (for reason explained above)
            // This shows that a call to Xamarin.Forms.DependencyService was made and thus should return
            // service instance (if registered)
            Assert.Throws<TargetInvocationException>(() => app.Container.Resolve<IDependencyServiceMock>(IfUnresolved.ReturnDefault));
        }
    }
}

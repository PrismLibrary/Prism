using Prism.Autofac.Forms.Tests.Mocks.Services;

[assembly: Xamarin.Forms.Dependency(typeof(IDependencyServiceMock))]

namespace Prism.Autofac.Forms.Tests.Mocks.Services
{

    public class DependencyServiceMock : IDependencyServiceMock
    {
    }
}
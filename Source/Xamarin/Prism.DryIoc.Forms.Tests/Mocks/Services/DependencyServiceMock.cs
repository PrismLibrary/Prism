using Prism.DryIoc.Forms.Tests.Mocks.Services;

[assembly: Xamarin.Forms.Dependency(typeof(IDependencyServiceMock))]

namespace Prism.DryIoc.Forms.Tests.Mocks.Services
{

    public class DependencyServiceMock : IDependencyServiceMock
    {
    }
}
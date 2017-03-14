using Prism.DI.Forms.Tests.Mocks.Services;

[assembly: Xamarin.Forms.Dependency( typeof( IDependencyServiceMock ) )]

namespace Prism.DI.Forms.Tests.Mocks.Services
{

    public class DependencyServiceMock : IDependencyServiceMock
    {
    }
}
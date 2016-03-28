using Prism.DryIoc.Forms.Tests.Services;
[assembly: Xamarin.Forms.Dependency(typeof(IDependencyServiceMock))]

namespace Prism.DryIoc.Forms.Tests.Services
{

    public class DependencyServiceMock : IDependencyServiceMock
    {
    }
}
namespace Prism.Tests.Mocks.ViewModels;

public sealed class MockViewModelWithDependency
{
    public MockViewModelWithDependency(object dependency) => Dependency = dependency;

    public object Dependency { get; }
}

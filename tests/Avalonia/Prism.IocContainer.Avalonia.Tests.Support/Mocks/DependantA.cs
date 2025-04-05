


namespace Prism.IocContainer.Avalonia.Tests.Support.Mocks
{
    public class DependantA : IDependantA
    {
        public DependantA(IDependantB dependantB)
        {
            MyDependantB = dependantB;
        }

        public IDependantB MyDependantB { get; set; }
    }

    public interface IDependantA
    {
        IDependantB MyDependantB { get; }
    }
}




namespace Prism.IocContainer.Wpf.Tests.Support.Mocks
{
    public class DependantB : IDependantB
    {
        public DependantB(IService service)
        {
            MyService = service;
        }

        public IService MyService { get; set; }
    }

    public interface IDependantB
    {
        IService MyService { get; }
    }
}
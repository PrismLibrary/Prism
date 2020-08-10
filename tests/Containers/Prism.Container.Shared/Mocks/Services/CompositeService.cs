namespace Prism.Ioc.Mocks.Services
{
    public class CompositeService : IServiceA, IServiceB, IServiceC
    {
        public IServiceB ServiceB => this;
    }
}

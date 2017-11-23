namespace Prism.Ioc
{
    public interface IContainerExtension<TContainer>: IContainerProvider<TContainer>, IContainerExtension
    {

    }

    public interface IContainerExtension : IContainerProvider, IContainerRegistry
    {

    }
}

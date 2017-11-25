namespace Prism.Ioc
{
    public static class IContainerExtensionExtensions
    {
        public static TContainer GetContainer<TContainer>(this IContainerRegistry containerRegistry)
        {
            return ((IContainerExtension<TContainer>)containerRegistry).Instance;
        }
    }
}

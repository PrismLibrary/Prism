using Prism.Ioc;

namespace Prism
{
    public interface IPlatformInitializer
    {
        void RegisterTypes(IContainerRegistry containerRegistry);
    }
}

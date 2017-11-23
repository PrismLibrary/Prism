using Prism.Ioc;

namespace Prism
{
    public interface IPlatformInitializer
    {
        void RegisterTypes(IContainer container);
    }

    public interface IPlatformInitializer<T>
    {
        void RegisterTypes(T container);
    }
}

namespace Prism
{
    public interface IPlatformInitializer<T>
    {
        void RegisterTypes(T container);
    }
}

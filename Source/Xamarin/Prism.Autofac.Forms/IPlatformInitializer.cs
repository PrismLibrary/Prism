using Autofac;

namespace Prism.Autofac
{
    public interface IPlatformInitializer : IPlatformInitializer<IContainer>
    {
        void RegisterTypes(ContainerBuilder builder);
    }
}

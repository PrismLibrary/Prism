using Autofac;

namespace Prism.Autofac.Forms
{
    public interface IPlatformInitializer : IPlatformInitializer<IContainer>
    {
        void RegisterTypes(ContainerBuilder builder);
    }
}

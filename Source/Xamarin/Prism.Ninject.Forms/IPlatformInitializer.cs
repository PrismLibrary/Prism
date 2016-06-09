using Ninject;

namespace Prism.Ninject
{
    public interface IPlatformInitializer
    {
        void RegisterTypes(IKernel container);
    }
}

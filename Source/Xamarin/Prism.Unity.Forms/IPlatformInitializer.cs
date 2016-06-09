using Microsoft.Practices.Unity;

namespace Prism.Unity
{
    public interface IPlatformInitializer
    {
        void RegisterTypes(IUnityContainer container);
    }
}

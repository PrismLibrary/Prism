using System.ComponentModel;

namespace Prism.Ioc
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ContainerLocator
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IContainerExtension Current { get; private set; }

        public static IContainerProvider Container => Current;

        public static IContainerExtension SetCurrent(IContainerExtension container) =>
            Current = container;
    }
}

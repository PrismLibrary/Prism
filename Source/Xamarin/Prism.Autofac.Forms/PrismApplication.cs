using Autofac;
using Prism.Ioc;

namespace Prism.Autofac
{
    /// <summary>
    /// Application base class using Autofac
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase
    {
        protected PrismApplication(IPlatformInitializer platformInitializer = null)
            : base(platformInitializer) { }

        /// <summary>
        /// Creates the <see cref="IAutofacContainerExtension"/>
        /// </summary>
        /// <returns></returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new AutofacContainerExtension(new ContainerBuilder());
        }
    }
}

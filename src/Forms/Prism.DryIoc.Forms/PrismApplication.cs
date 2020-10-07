using DryIoc;
using Prism.Ioc;
using Xamarin.Forms.Internals;

[assembly: Xamarin.Forms.XmlnsDefinition("http://prismlibrary.com", "Prism.DryIoc")]
namespace Prism.DryIoc
{
    /// <summary>
    /// Application base class using DryIoc
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase
    {
        /// <summary>
        /// Initializes a new instance of PrismApplication using the default constructor
        /// </summary>
        protected PrismApplication()
            : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplication" /> with a <see cref="IPlatformInitializer" />.
        /// Used when there are specific types that need to be registered on the platform.
        /// </summary>
        /// <param name="platformInitializer">The <see cref="IPlatformInitializer"/>.</param>
        protected PrismApplication(IPlatformInitializer platformInitializer)
            : base(platformInitializer) { }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplication" /> with a <see cref="IPlatformInitializer" />.
        /// Used when there are specific types that need to be registered on the platform.
        /// Also determines whether to set the <see cref="DependencyResolver" /> for resolving Renderers and Platform Effects.
        /// </summary>
        /// <param name="platformInitializer">The <see cref="IPlatformInitializer"/>.</param>
        /// <param name="setFormsDependencyResolver">Should <see cref="PrismApplication" /> set the <see cref="DependencyResolver" />.</param>
        protected PrismApplication(IPlatformInitializer platformInitializer, bool setFormsDependencyResolver)
            : base(platformInitializer, setFormsDependencyResolver) { }

        /// <summary>
        /// Creates the <see cref="IContainerExtension"/> for DryIoc
        /// </summary>
        /// <returns></returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new DryIocContainerExtension(new Container(CreateContainerRules()));
        }

        /// <summary>
        /// Create <see cref="Rules" /> to alter behavior of <see cref="IContainer" />
        /// </summary>
        /// <returns>An instance of <see cref="Rules" /></returns>
        protected virtual Rules CreateContainerRules() => DryIocContainerExtension.DefaultRules;
    }
}

using Prism.Ioc;
using Unity;
using System.Collections.Generic;
#if __ANDROID__
using System;
using Prism.Logging;
using Unity.Resolution;
using Xamarin.Forms.Internals;
#endif

#if !NETSTANDARD1_0
[assembly: Xamarin.Forms.XmlnsDefinition("http://prismlibrary.com", "Prism.Unity")]
#endif
namespace Prism.Unity
{
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

        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension(new UnityContainer());
        }
    }
}

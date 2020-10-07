using System;
using DryIoc;
using Prism.Ioc;

namespace Prism.DryIoc
{
    /// <summary>
    /// Base bootstrapper class that uses <see cref="DryIocContainerExtension"/> as it's container.
    /// </summary>
    public abstract class PrismBootstrapper : PrismBootstrapperBase
    {
        /// <summary>
        /// Create <see cref="Rules" /> to alter behavior of <see cref="IContainer" />
        /// </summary>
        /// <returns>An instance of <see cref="Rules" /></returns>
        protected virtual Rules CreateContainerRules() => DryIocContainerExtension.DefaultRules;

        /// <summary>
        /// Create a new <see cref="DryIocContainerExtension"/> used by Prism.
        /// </summary>
        /// <returns>A new <see cref="DryIocContainerExtension"/>.</returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new DryIocContainerExtension(new Container(CreateContainerRules()));
        }

        /// <summary>
        /// Registers the <see cref="Type"/>s of the Exceptions that are not considered 
        /// root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ContainerException));
        }
    }
}

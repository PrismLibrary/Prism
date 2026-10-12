using Unity;

namespace Prism.Unity
{
    /// <summary>
    /// Base application class that uses <see cref="UnityContainerExtension"/> as it's container.
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase
    {
        /// <summary>
        /// Create a new <see cref="UnityContainerExtension"/> used by Prism.
        /// </summary>
        /// <returns>A new <see cref="UnityContainerExtension"/>.</returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension();
        }

        /// <summary>
        /// Registers <see cref="ResolutionFailedException"/> as a framework exception so
        /// exception unwrapping can report the underlying application error.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ResolutionFailedException));
        }
    }
}

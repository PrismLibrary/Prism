namespace Prism.Ioc
{
    /// <summary>
    /// Provides a way to finalize a container once all registrations have been made
    /// </summary>
    public interface IContainerRequiresFinalization
    {
        /// <summary>
        /// Finalize any container configurations prior to resolving services.
        /// </summary>
        void Finalize();
    }
}

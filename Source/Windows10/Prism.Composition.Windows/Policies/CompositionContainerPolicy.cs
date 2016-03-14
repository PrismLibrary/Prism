namespace Prism.Composition.Windows.Policies
{
    using System;
    using System.Composition.Hosting;

    /// <summary>
    /// Default implementation of the <see cref="ICompositionContainerPolicy"/> interface.
    /// </summary>
    public class CompositionContainerPolicy : ICompositionContainerPolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContainerPolicy"/> class.
        /// </summary>
        /// <param name="compositionHostWithCustomProviders">Instance of the <see cref="CompositionHost"/>
        /// this policy holds with custom providers.
        /// </param>
        /// <param name="compositionHostWithDefaultProviders">Instance of the <see cref="CompositionHost"/>
        /// this policy holds with default providers.
        /// </param>
        public CompositionContainerPolicy(CompositionHost compositionHostWithCustomProviders, CompositionHost compositionHostWithDefaultProviders)
        {
            if (compositionHostWithCustomProviders == null)
            {
                throw new ArgumentNullException("compositionHostWithCustomProviders");
            }

            if (compositionHostWithDefaultProviders == null)
            {
                throw new ArgumentNullException("compositionHostWithDefaultProviders");
            }

            this.CompositionHostWithCustomProviders = compositionHostWithCustomProviders;

            this.CompositionHostWithDefaultProviders = compositionHostWithDefaultProviders;
        }

        /// <summary>
        /// Gets or (sets) the <see cref="CompositionHost"/> instance with custom providers.
        /// </summary>
        public CompositionHost CompositionHostWithCustomProviders { get; private set; }

        /// <summary>
        /// Gets or (sets) the <see cref="CompositionHost"/> instance with default providers.
        /// </summary>
        public CompositionHost CompositionHostWithDefaultProviders { get; private set; }
    }
}

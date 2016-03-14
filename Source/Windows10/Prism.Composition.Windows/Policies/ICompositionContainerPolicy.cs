namespace Prism.Composition.Windows.Policies
{
    using System.Composition.Hosting;
    using Microsoft.Practices.ObjectBuilder2;

    /// <summary>
    /// Represents a builder policy which holds two <see cref="CompositionHost"/> instances.
    /// </summary>
    public interface ICompositionContainerPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Gets the <see cref="CompositionHost"/> instance with custom providers.
        /// </summary>
        CompositionHost CompositionHostWithCustomProviders { get; }

        /// <summary>
        /// Gets the <see cref="CompositionHost"/> instance with default providers.
        /// </summary>
        CompositionHost CompositionHostWithDefaultProviders { get; }
    }
}
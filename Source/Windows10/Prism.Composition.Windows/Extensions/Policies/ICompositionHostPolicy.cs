namespace Prism.Composition.Windows.Extensions.Policies
{
    using System.Composition.Hosting;
    using Microsoft.Practices.ObjectBuilder2;
    
    /// <summary> Interface for composition host policy.</summary>
    public interface ICompositionHostPolicy : IBuilderPolicy
    {
        /// <summary> Gets or sets the composition host.</summary>
        /// <value> The composition host.</value>
        CompositionHost CompositionHost { get; set; }
    }
}
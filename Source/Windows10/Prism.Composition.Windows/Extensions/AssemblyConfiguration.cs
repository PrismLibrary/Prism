namespace Prism.Composition.Windows.Extensions
{
    using System.Composition.Convention;
    using System.Reflection;
    
    /// <summary>An assembly configuration.</summary>
    public class AssemblyConfiguration
    {
        /// <summary>Gets or sets the assembly.</summary>
        /// <value>The assembly.</value>
        public Assembly Assembly { get; set; }
        
        /// <summary>Gets or sets the conventions.</summary>
        /// <value>The conventions.</value>
        public AttributedModelProvider Conventions { get; set; }
    }
}
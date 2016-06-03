namespace Prism.Composition.Windows.Extensions.Policies
{
    using System;
    using System.Composition.Hosting;

    /// <summary>A composition host policy.</summary>
    public class CompositionHostPolicy : ICompositionHostPolicy
    {
        /// <summary>Initializes a new instance of the <see cref="CompositionHostPolicy" /> class.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null. </exception>
        /// <param name="compositionHost">  The composition host. </param>
        public CompositionHostPolicy(CompositionHost compositionHost)
        {
            if (compositionHost == null)
            {
                throw new ArgumentNullException("compositionHost");
            }

            this.CompositionHost = compositionHost;
        }

        /// <summary>Gets or sets the composition host.</summary>
        /// <value>The composition host.</value>
        public CompositionHost CompositionHost { get; set; }
    }
}

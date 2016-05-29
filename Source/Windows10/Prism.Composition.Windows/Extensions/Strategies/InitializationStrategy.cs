namespace Prism.Composition.Windows.Extensions.Strategies
{
    using System.Composition;
    using Microsoft.Practices.ObjectBuilder2;
    using Policies;
    
    /// <summary>An initialization strategy.</summary>
    public class InitializationStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation. </param>
        public override void PostBuildUp(IBuilderContext context)
        {
            var compositionHost = context.Policies.Get<ICompositionHostPolicy>(null).CompositionHost;

            compositionHost.SatisfyImports(context.Existing);
        }
    }
}

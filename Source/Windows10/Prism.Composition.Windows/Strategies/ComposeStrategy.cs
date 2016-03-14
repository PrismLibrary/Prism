namespace Prism.Composition.Windows.Strategies
{
    using System;
    using System.Composition;
    using Microsoft.Practices.ObjectBuilder2;
    using Policies;

    /// <summary>
    /// Represents a strategy which injects MEF dependencies to
    /// the Unity created instance.
    /// </summary>
    public class ComposeStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PostBuildUp(IBuilderContext context)
        {     
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
                   
            var container = context.Policies.Get<ICompositionContainerPolicy>(null).CompositionHostWithCustomProviders;

            container.SatisfyImports(context.Existing);
        }
    }
}
namespace Prism.Composition.Windows.Strategies
{
    using System;
    using System.Collections;
    using Microsoft.Practices.ObjectBuilder2;
    using Policies;

    /// <summary>
    /// Represents a MEF composition strategy which tries to resolve desired
    /// component via MEF. If succeeded, build process is completed.
    /// </summary>
    public class CompositionStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var policy = context.Policies.Get<IBuildKeyMappingPolicy>(context.OriginalBuildKey);

            if (policy != null)
            {
                return;
            }

            var buildKey = context.BuildKey;

            var container = context.Policies.Get<ICompositionContainerPolicy>(null).CompositionHostWithCustomProviders;

            object export = null;

            var exports = (IList)container.GetExports(buildKey.Type, buildKey.Name);

            if (exports.Count > 0)
            {
                export = exports[0];
            }

            if (export != null)
            {
                context.Existing = export;

                context.BuildComplete = true;
            }
        }
    }
}

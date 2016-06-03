namespace Prism.Composition.Windows.Extensions.Strategies
{
    using System;
    using System.Collections;
    using System.Composition.Hosting;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.ObjectBuilder2;
    using Policies;
    
    /// <summary>A type mapping strategy.</summary>
    public class TypeMappingStrategy : BuilderStrategy
    {
        /// <summary>Gets or sets the composition host.</summary>
        /// <value>The composition host.</value>
        private CompositionHost CompositionHost { get; set; }
        
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation. </param>
        public override void PreBuildUp(IBuilderContext context)
        {
            var buildKeyMappingPolicy = context.Policies.Get<IBuildKeyMappingPolicy>(context.OriginalBuildKey);

            if (buildKeyMappingPolicy != null)
            {
                return;
            }

            this.CompositionHost = context.Policies.Get<ICompositionHostPolicy>(null).CompositionHost;

            object result = null;

            if (context.BuildKey.Type.GetInterfaces().Contains(typeof(IEnumerable)))
            {
                result = this.TryGetExports(context.BuildKey);
            }
            else
            {
                result = this.TryGetExport(context.BuildKey);   
            }

            if (result != null)
            {
                context.Existing = result;

                context.BuildComplete = true;
            }
        }

        /// <summary>Try get exports.</summary>
        /// <param name="namedTypeBuildKey">The named type build key. </param>
        /// <returns>An object.</returns>
        private object TryGetExports(NamedTypeBuildKey namedTypeBuildKey)
        {
            Type type = null;

            if (namedTypeBuildKey.Type.GetElementType() != null)
            {
                type = namedTypeBuildKey.Type.GetElementType();
            }
            else
            {
                type = namedTypeBuildKey.Type.GenericTypeArguments[0];
            }

            var results = this.CompositionHost.GetExports(type, namedTypeBuildKey.Name);

            if (results.Count() > 0)
            {
                return results;
            }

            return null;
        }

        /// <summary>Try get export.</summary>
        /// <param name="namedTypeBuildKey">The named type build key. </param>
        /// <returns>An object.</returns>
        private object TryGetExport(NamedTypeBuildKey namedTypeBuildKey)
        {
            var lazyType = typeof(Lazy<>);

            Type[] typeArguments = { namedTypeBuildKey.Type };

            var type = lazyType.MakeGenericType(typeArguments);

            var results = this.CompositionHost.GetExports(type, namedTypeBuildKey.Name);

            if (results.Count() > 0)
            {
                PropertyInfo field = type.GetProperty("Value");

                return field.GetValue(results.First());
            }

            return null;
        }
    }
}

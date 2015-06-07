using System;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace Prism.Unity.Extensions
{
    internal class DependencyServiceStrategy : BuilderStrategy
    {
        private readonly IUnityContainer _container;

        public DependencyServiceStrategy(IUnityContainer container)
        {
            _container = container;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var key = context.OriginalBuildKey;

            if (key.Type.GetTypeInfo().IsInterface && !_container.IsRegistered(key.Type))
            {
                context.Existing = CallToDependencyService(key.Type);
                context.BuildComplete = context.Existing != null;
            }
        }

        /// <summary>
        /// This is horrendous, but Xamarin did not provide a non-generic version of the DependencyService.Get method call.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public object CallToDependencyService(Type targetType)
        {
            MethodInfo method = typeof(DependencyService).GetTypeInfo().GetDeclaredMethod("Get"); 
            MethodInfo genericMethod = method.MakeGenericMethod(targetType);
            return genericMethod.Invoke(null, new object[] { DependencyFetchTarget.GlobalInstance });
        }
    }
}

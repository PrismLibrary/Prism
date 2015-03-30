using System;
using Ninject;
using Ninject.Parameters;

namespace Prism.Ninject
{
    public static class NinjectExtensions
    {
        public static bool IsRegistered<TService>(this IKernel kernel)
        {
            return kernel.IsRegistered(typeof(TService));
        }

        public static bool IsRegistered(this IKernel kernel, Type type)
        {
            return kernel.CanResolve(kernel.CreateRequest(type, _ => true, new IParameter[] { }, false, false));
        }

        public static void RegisterTypeIfMissing<TFrom, TTo>(this IKernel kernel, bool asSingleton)
        {
            kernel.RegisterTypeIfMissing(typeof(TFrom), typeof(TTo), asSingleton);
        }

        public static void RegisterTypeIfMissing(this IKernel kernel, Type from, Type to, bool asSingleton)
        {
            // Don't do anything if there are already bindings registered
            if (kernel.IsRegistered(from))
            {
                return;
            }

            // Register the types
            var binding = kernel.Bind(from).To(to);
            if (asSingleton)
            {
                binding.InSingletonScope();
            }
            else
            {
                binding.InTransientScope();
            }
        }
    }
}
using System;

namespace Prism.SimpleInjector.Navigation
{
    public class SimpleInjectorPageNavigationException : Exception
    {
        public SimpleInjectorPageNavigationException()
        {
        }

        public SimpleInjectorPageNavigationException(string message) : base(message)
        {
        }

        public SimpleInjectorPageNavigationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
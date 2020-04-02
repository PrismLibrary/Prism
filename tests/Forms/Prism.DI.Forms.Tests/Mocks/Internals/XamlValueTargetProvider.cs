using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prism.DI.Forms.Tests.Mocks.Internals
{
    class XamlValueTargetProvider : IProvideValueTarget
    {
        public XamlValueTargetProvider(object targetObject, object targetProperty)
        {
            TargetObject = targetObject;
            TargetProperty = targetProperty;
        }

        public object TargetObject { get; }
        public object TargetProperty { get; }

        public IEnumerable<object> ParentObjects => GetBindableStack();

        private IEnumerable<object> GetBindableStack()
        {
            var stack = new List<object>();
            if(TargetObject is Element element)
            {
                stack.Add(element);
                while(element.Parent != null)
                {
                    element = element.Parent;
                    stack.Add(element);
                }
            }

            return stack;
        }
    }
}
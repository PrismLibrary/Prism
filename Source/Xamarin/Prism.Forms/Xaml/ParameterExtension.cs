using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Xaml.Internals;

namespace Prism.Xaml
{
    public class ParameterExtension : Parameter, IMarkupExtension<Parameter>
    {
        public Parameter ProvideValue(IServiceProvider serviceProvider)
        {
            var target = serviceProvider.GetService<IProvideValueTarget>();
            if (target != null && target.TargetObject is BindableObject bindableTargetObject)
            {
                var self = this;
                bindableTargetObject.BindingContextChanged -= BindingContextChanged;
                bindableTargetObject.BindingContextChanged += BindingContextChanged;

                void BindingContextChanged(object parentObject, EventArgs args)
                {
                    var parent = (BindableObject)parentObject;
                    self.BindingContext = parent?.BindingContext;
                }
            }
        
            return this;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            ProvideValue(serviceProvider);
    }
}

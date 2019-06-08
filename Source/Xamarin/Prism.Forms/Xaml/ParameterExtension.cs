using System;
using Xamarin.Forms.Xaml;

namespace Prism.Xaml
{
    public class ParameterExtension : Parameter, IMarkupExtension<Parameter>
    {
        public Parameter ProvideValue(IServiceProvider serviceProvider) =>
            this;

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            ProvideValue(serviceProvider);
    }
}

using System;
using Xamarin.Forms.Xaml;

namespace Prism.Xaml
{
    /// <summary>
    /// XAML Extension for INavigation and IDialog parameters
    /// </summary>
    public class ParameterExtension : Parameter, IMarkupExtension<Parameter>
    {
        /// <summary>
        /// Method to retrieve value for parameter
        /// </summary>
        /// <param name="serviceProvider">The service that supplies the parameters value</param>
        /// <returns>The instance of the ParameterExtension class</returns>
        public Parameter ProvideValue(IServiceProvider serviceProvider) =>
            this;

        /// <summary>
        /// Method to retrieve value for parameter
        /// </summary>
        /// <param name="serviceProvider">The service that supplies the parameters value</param>
        /// <returns>The return value from <see cref="ProvideValue(IServiceProvider)"/></returns>
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            ProvideValue(serviceProvider);
    }
}

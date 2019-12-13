using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Prism.Mvvm
{
    /// <summary>
    /// This class defines the attached property and related change handler that calls the <see cref="Prism.Mvvm.ViewModelLocationProvider"/>.
    /// </summary>
    public static class ViewModelLocator
    {
        /// <summary>
        /// Instructs Prism whether or not to automatically create an instance of a ViewModel using a convention, and assign the associated View's <see cref="Xamarin.Forms.BindableObject.BindingContext"/> to that instance.
        /// </summary>
        public static readonly BindableProperty AutowireViewModelProperty =
            BindableProperty.CreateAttached("AutowireViewModel", typeof(bool?), typeof(ViewModelLocator), null, propertyChanged: OnAutowireViewModelChanged);

        /// <summary>
        /// Instructs Prism to use a given page as the parent for a Partial View
        /// </summary>
        [Obsolete("This API is Obsolete and will be removed during the 8.0 previews")]
        public static readonly BindableProperty AutowirePartialViewProperty =
            BindableProperty.CreateAttached("AutowirePartialView", typeof(Page), typeof(ViewModelLocator), null, propertyChanged: OnAutowirePartialViewChanged);

        internal static readonly BindableProperty PartialViewsProperty =
            BindableProperty.CreateAttached("PrismPartialViews", typeof(List<BindableObject>), typeof(ViewModelLocator), null);

        /// <summary>
        /// Gets the AutowireViewModel property value.
        /// </summary>
        /// <param name="bindable"></param>
        /// <returns></returns>
        public static bool? GetAutowireViewModel(BindableObject bindable)
        {
            return (bool?)bindable.GetValue(ViewModelLocator.AutowireViewModelProperty);
        }

        /// <summary>
        /// Sets the AutowireViewModel property value.  If <c>true</c>, creates an instance of a ViewModel using a convention, and sets the associated View's <see cref="Xamarin.Forms.BindableObject.BindingContext"/> to that instance.
        /// </summary>
        /// <param name="bindable"></param>
        /// <param name="value"></param>
        public static void SetAutowireViewModel(BindableObject bindable, bool? value)
        {
            bindable.SetValue(ViewModelLocator.AutowireViewModelProperty, value);
        }

        [Obsolete("This API is Obsolete and will be removed during the 8.0 previews")]
        public static Page GetAutowirePartialView(BindableObject bindable)
        {
            return (Page)bindable.GetValue(AutowirePartialViewProperty);
        }

        [Obsolete("This API is Obsolete and will be removed during the 8.0 previews")]
        public static void SetAutowirePartialView(BindableObject bindable, Page page)
        {
            bindable.SetValue(AutowirePartialViewProperty, page);
        }

        [Obsolete("This API is Obsolete and will be removed during the 8.0 previews")]
        internal static List<BindableObject> GetPartialViews(this Page page)
        {
            return (List<BindableObject>)page.GetValue(PartialViewsProperty);
        }

        private static void OnAutowireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            bool? bNewValue = (bool?)newValue;
            if (bNewValue.HasValue && bNewValue.Value)
                ViewModelLocationProvider.AutoWireViewModelChanged(bindable, Bind);
        }

        [Obsolete("This API is Obsolete and will be removed during the 8.0 previews")]
        private static void OnAutowirePartialViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue == newValue)
                return;

            if (oldValue is Page oldPage)
            {
                List<BindableObject> oldPartials = oldPage.GetPartialViews();
                oldPartials.Remove(bindable);
            }

            if (newValue is Page page)
            {
                // Add View to Views Collection for Page.
                List<BindableObject> partialViews = page.GetPartialViews();
                if (partialViews == null)
                {
                    partialViews = new List<BindableObject>();
                    page.SetValue(PartialViewsProperty, partialViews);
                }

                partialViews.Add(bindable);
                // Set Autowire Property
                if (bindable.GetValue(AutowireViewModelProperty) == null)
                {
                    bindable.SetValue(AutowireViewModelProperty, true);
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> of a View
        /// </summary>
        /// <param name="view">The View to set the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> on</param>
        /// <param name="viewModel">The object to use as the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> for the View</param>
        private static void Bind(object view, object viewModel)
        {
            if (view is BindableObject element)
                element.BindingContext = viewModel;
        }
    }
}

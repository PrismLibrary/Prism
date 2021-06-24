using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Behaviors;
using Prism.Properties;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prism.Xaml
{
    /// <summary>
    /// Provides a base implementation of <see cref="IMarkupExtension{T}" /> that locates the parent <see cref="Page" />
    /// </summary>
    /// <typeparam name="T">The generic type the extension will return.</typeparam>
    public abstract class ParentPageAwareExtension<T> : BindableObject, IMarkupExtension<T>
    {
        private IServiceProvider ServiceProvider;

        private Element _targetElement;
        /// <summary>
        /// The target element the XAML Extension is being used on.
        /// </summary>
        protected Element TargetElement
        {
            get
            {
                if (_targetElement == null)
                {
                    Initialize();
                }

                return _targetElement;
            }
            set => _targetElement = value;
        }

        /// <summary>
        /// Sets the Target BindingContext strategy
        /// </summary>
        public TargetBindingContext TargetBindingContext { get; set; }

        private Page _sourcePage;
        /// <summary>
        /// The parent Page of the layout where the XAML Extension is being used.
        /// </summary>
        public Page SourcePage
        {
            protected internal get
            {
                if (_sourcePage == null)
                {
                    Initialize();
                }

                return _sourcePage;
            }
            set => _sourcePage = value;
        }

        /// <summary>
        /// Provides the object that is created from the markup extension.
        /// </summary>
        /// <param name="serviceProvider">The Service Provider</param>
        /// <returns>The object created by the markup extension.</returns>
        public T ProvideValue(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            return ProvideValue();
        }

        /// <summary>
        /// Provides the object that is created from the markup extension.
        /// </summary>
        /// <returns>The object created by the markup extension.</returns>
        protected abstract T ProvideValue();

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            ProvideValue(serviceProvider);

        private void Initialize()
        {
            var valueTargetProvider = ServiceProvider.GetService<IProvideValueTarget>();

            if (valueTargetProvider == null)
                throw new ArgumentException(Resources.ServiceProviderDidNotHaveIProvideValueTarget);

            _targetElement = valueTargetProvider.TargetObject as Element;

            //this is handling the scenario of the extension being used within the EventToCommandBehavior
            if (_targetElement is null && valueTargetProvider.TargetObject is BehaviorBase<BindableObject> behavior)
                _targetElement = behavior.AssociatedObject as Element;

            if (_targetElement is null)
                throw new Exception($"{valueTargetProvider.TargetObject} is not supported");

            var parentPage = (Page)GetBindableStack().FirstOrDefault(p => p.GetType()
                                                                           .IsSubclassOf(typeof(Page)));

            if (_sourcePage is null && parentPage != null)
            {
                SourcePage = parentPage;

                if (parentPage.Parent is FlyoutPage flyout
                    && flyout.Flyout == parentPage)
                {
                    SourcePage = flyout;
                }
            }

            if (BindingContext == null || !IsSet(BindingContextProperty))
            {
                BindingContext = TargetBindingContext switch
                {
                    TargetBindingContext.Page => SourcePage.BindingContext,
                    _ => TargetElement.BindingContext
                };
            }
        }

        /// <summary>
        /// Callback when the TargetElement is changed
        /// </summary>
        protected virtual void OnTargetElementChanged()
        {
        }

        private IEnumerable<Element> GetBindableStack()
        {
            var stack = new List<Element>();
            if (TargetElement is Element element)
            {
                stack.Add(element);
                while (element.Parent != null)
                {
                    element = element.Parent;
                    stack.Add(element);
                }
            }

            return stack;
        }
    }
}

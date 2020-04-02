using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Properties;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prism.Xaml
{
    public abstract class ParentPageAwareExtension<T> : BindableObject, IMarkupExtension<T>
    {
        private IServiceProvider ServiceProvider;

        private Element _targetElement;
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

        private Page _sourcePage;
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

        public T ProvideValue(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            return ProvideValue();
        }

        protected abstract T ProvideValue();

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            ProvideValue(serviceProvider);

        private void Initialize()
        {
            var valueTargetProvider = ServiceProvider.GetService<IProvideValueTarget>();

            if (valueTargetProvider == null)
                throw new ArgumentException(Resources.ServiceProviderDidNotHaveIProvideValueTarget);

            _targetElement = valueTargetProvider.TargetObject as Element;

            if (_targetElement is null)
                throw new ArgumentNullException(nameof(TargetElement));

            var parentPage = (Page)GetBindableStack().FirstOrDefault(p => p.GetType()
                                                                           .IsSubclassOf(typeof(Page)));

            if (_sourcePage is null && parentPage != null)
            {
                SourcePage = parentPage;

                if (parentPage.Parent is MasterDetailPage mdp
                    && mdp.Master == parentPage)
                {
                    SourcePage = mdp;
                }
            }

            if(BindingContext == null)
            {
                BindingContext = SourcePage.BindingContext;
            }
        }

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

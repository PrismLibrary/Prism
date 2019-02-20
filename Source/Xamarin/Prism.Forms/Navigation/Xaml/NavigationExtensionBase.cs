using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prism.Navigation.Xaml
{
    public abstract class NavigationExtensionBase : BindableObject, IMarkupExtension<ICommand>, ICommand
    {
        public static readonly BindableProperty AnimatedProperty =
            BindableProperty.Create(nameof(Animated), typeof(bool), typeof(NavigationExtensionBase), true);

        public static readonly BindableProperty UseModalNavigationProperty =
            BindableProperty.Create(nameof(UseModalNavigation), typeof(bool?), typeof(NavigationExtensionBase), null);

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

        protected internal bool IsNavigating;

        private Page _sourcePage;
        public Page SourcePage
        {
            protected internal get
            {
                if(_sourcePage == null)
                {
                    Initialize();
                }

                return _sourcePage;
            }
            set => _sourcePage = value;
        }

        public bool Animated
        {
            get => (bool)GetValue(AnimatedProperty);
            set => SetValue(AnimatedProperty, value);
        }

        public bool? UseModalNavigation
        {
            get => (bool?)GetValue(UseModalNavigationProperty);
            set => SetValue(UseModalNavigationProperty, value);
        }

        public bool CanExecute(object parameter) => !IsNavigating;

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            var parameters = parameter.ToNavigationParameters(TargetElement);

            IsNavigating = true;
            try
            {
                RaiseCanExecuteChanged();

                var navigationService = Navigation.GetNavigationService(SourcePage);
                await HandleNavigation(parameters, navigationService);
            }
            catch(Exception ex)
            {
                Log(ex, parameters);
            }
            finally
            {
                IsNavigating = false;
                RaiseCanExecuteChanged();
            }
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }

        public ICommand ProvideValue(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            return this;
        }

        protected virtual void Log(Exception ex, INavigationParameters parameters)
        {
            Xamarin.Forms.Internals.Log.Warning("Warning", $"{GetType().Name} threw an exception");
            Xamarin.Forms.Internals.Log.Warning("Exception", ex.ToString());
        }

        protected abstract Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService);

        protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private void Initialize()
        {
            var valueTargetProvider = ServiceProvider.GetService<IProvideValueTarget>();

            if (valueTargetProvider == null)
                throw new ArgumentException("The ServiceProvider did not provide a 'IProvideValueTarget'");

            TargetElement = valueTargetProvider.TargetObject as Element;

            if (_targetElement is null)
                throw new ArgumentNullException(nameof(TargetElement));

            Navigation.SetRaiseCanExecuteChangedInternal(TargetElement, RaiseCanExecuteChanged);

            var parentPage = (Page)GetBindableStack().FirstOrDefault(p => p.GetType()
                                                                       .GetTypeInfo()
                                                                       .IsSubclassOf(typeof(Page)));

            if (_sourcePage is null && parentPage != null)
            {
                SourcePage = parentPage;

                if(parentPage.Parent is MasterDetailPage mdp 
                    && mdp.Master == parentPage)
                {
                    SourcePage = mdp;
                }
            }
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

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
    public abstract class NavigationExtensionBase : IMarkupExtension, ICommand
    {
        protected BindableObject Bindable;
        protected IEnumerable<BindableObject> BindableTree;
        protected bool IsNavigating;

        public Page SourcePage { protected get; set; }

        public bool CanExecute(object parameter)
        {
            var canNavigate = true;
            foreach (var bindableObject in BindableTree)
            {
                canNavigate = Navigation.GetCanNavigate(bindableObject);
                if (!canNavigate) break;
            }

            return canNavigate && !IsNavigating;
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            var parameters = parameter.ToNavigationParameters(Bindable);

            IsNavigating = true;
            RaiseCanExecuteChanged();

            var navigationService = Navigation.GetNavigationService(SourcePage);
            await HandleNavigation(parameters, navigationService);

            IsNavigating = false;
            RaiseCanExecuteChanged();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            var valueTargetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var rootObjectProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
            Initialize(valueTargetProvider, rootObjectProvider);
            return this;
        }

        protected abstract Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService);

        protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private void Initialize(IProvideValueTarget valueTargetProvider, IRootObjectProvider rootObjectProvider)
        {
            // if XamlCompilation is active, IRootObjectProvider is not available, but SimpleValueTargetProvider is available
            // if XamlCompilation is inactive, IRootObjectProvider is available, but SimpleValueTargetProvider is not available
            object rootObject;
            //object bindable;

            var propertyInfo = valueTargetProvider.GetType().GetTypeInfo().DeclaredProperties.FirstOrDefault(dp => dp.Name.Contains("ParentObjects"));
            if (propertyInfo == null) throw new ArgumentNullException("ParentObjects");
            var parentObjects = (propertyInfo.GetValue(valueTargetProvider) as IEnumerable<object>).ToList();
            BindableTree = parentObjects.Cast<BindableObject>();

            if (rootObjectProvider == null && valueTargetProvider == null)
                throw new ArgumentException("serviceProvider does not provide an IRootObjectProvider or SimpleValueTargetProvider");
            if (rootObjectProvider == null)
            {
                var parentObject = parentObjects.FirstOrDefault(pO => pO.GetType().GetTypeInfo().IsSubclassOf(typeof(Page)));

                Bindable = (BindableObject) parentObjects.FirstOrDefault();
                rootObject = parentObject ?? throw new ArgumentNullException("parentObject");
            }
            else
            {
                rootObject = rootObjectProvider.RootObject;
                Bindable = (BindableObject) valueTargetProvider.TargetObject;
            }

            Navigation.SetRaiseCanExecuteChangedInternal(Bindable, RaiseCanExecuteChanged);

            if (rootObject is Page providedPage)
                SourcePage = SourcePage ?? providedPage; // allow the user's defined page to take precedence
        }
    }
}
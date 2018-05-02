using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Prism.Common;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prism.Navigation.Xaml
{
    public abstract class Navigation : IMarkupExtension, ICommand
    {
        protected internal static readonly BindableProperty RaiseCanExecuteChangedInternalProperty =
            BindableProperty.CreateAttached("RaiseCanExecuteChangedInternal",
                typeof(Action),
                typeof(Navigation),
                default(Action));

        public static readonly BindableProperty CanNavigateProperty =
            BindableProperty.CreateAttached("CanNavigate",
                typeof(bool),
                typeof(Navigation),
                true,
                propertyChanged: OnCanNavigatePropertyChanged);

        protected internal static readonly BindableProperty NavigationServiceProperty =
            BindableProperty.CreateAttached("NavigationService",
                typeof(INavigationService),
                typeof(Navigation),
                default(INavigationService));


        protected BindableObject Bindable;
        protected IEnumerable<BindableObject> BindableTree;
        protected bool IsNavigating;

        public bool AllowDoubleTap { get; set; } = false;

        public Page Page { protected get; set; }

        public bool CanExecute(object parameter)
        {
            var canNavigate = true;
            foreach (var bindableObject in BindableTree)
            {
                canNavigate = GetCanNavigate(bindableObject);
                if (!canNavigate) break;
            }

            return canNavigate && (AllowDoubleTap || !IsNavigating);
        }

        public event EventHandler CanExecuteChanged;
        public abstract void Execute(object parameter);

        public object ProvideValue(System.IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            var valueTargetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var rootObjectProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
            Initialize(valueTargetProvider, rootObjectProvider);
            return this;
        }

        public static bool GetCanNavigate(BindableObject view) => (bool) view.GetValue(CanNavigateProperty);

        public static void SetCanNavigate(BindableObject view, bool value) => view.SetValue(CanNavigateProperty, value);

        protected internal static Action GetRaiseCanExecuteChangedInternal(BindableObject view) => (Action) view.GetValue(RaiseCanExecuteChangedInternalProperty);

        protected internal static void SetRaiseCanExecuteChangedInternal(BindableObject view, Action value) => view.SetValue(RaiseCanExecuteChangedInternalProperty, value);

        protected static INavigationService GetNavigationService(Page page)
        {
            var navigationService = (INavigationService) page.GetValue(NavigationServiceProperty);
            if (navigationService == null) page.SetValue(NavigationServiceProperty, navigationService = CreateNavigationService(page));

            return navigationService;
        }

        protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private static INavigationService CreateNavigationService(Page view)
        {
            var context = (PrismApplicationBase) Application.Current;
            var navigationService = context.Container.Resolve<INavigationService>("PageNavigationService");
            if (navigationService is IPageAware pageAware) pageAware.Page = view;
            return navigationService;
        }

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

            SetRaiseCanExecuteChangedInternal(Bindable, RaiseCanExecuteChanged);

            if (rootObject is Page page)
                Page = Page ?? page; // allow the user's defined page to take precedence
        }

        private static void OnCanNavigatePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var action = GetRaiseCanExecuteChangedInternal(bindable);
            action?.Invoke();
        }
    }
}
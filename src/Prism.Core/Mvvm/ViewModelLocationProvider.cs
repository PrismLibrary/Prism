using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

#nullable enable
namespace Prism.Mvvm
{
    /// <summary>
    /// The ViewModelLocationProvider class locates the view model for the view that has the AutoWireViewModelChanged attached property set to true.
    /// The view model will be located and injected into the view's DataContext. To locate the view model, two strategies are used: First the ViewModelLocationProvider
    /// will look to see if there is a view model factory registered for that view, if not it will try to infer the view model using a convention based approach.
    /// This class also provides methods for registering the view model factories,
    /// and also to override the default view model factory and the default view type to view model type resolver.
    /// </summary>

    // Documentation on using the MVVM pattern is at http://go.microsoft.com/fwlink/?LinkID=288814&clcid=0x409

    public static class ViewModelLocationProvider
    {
        /// <summary>
        /// Resets the ViewModelLocationProvider for Unit Testing Purposes.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Reset()
        {
            _factories = new Dictionary<string, Func<object>>();
            _typeFactories = new Dictionary<string, Type>();
            _defaultViewModelFactory = type => Activator.CreateInstance(type);
            _defaultViewModelFactoryWithViewParameter = null;
            _defaultViewTypeToViewModelTypeResolver = DefaultViewTypeToViewModel;
        }

        /// <summary>
        /// A dictionary that contains all the registered factories for the views.
        /// </summary>
        static Dictionary<string, Func<object>> _factories = new Dictionary<string, Func<object>>();

        /// <summary>
        /// A dictionary that contains all the registered ViewModel types for the views.
        /// </summary>
        static Dictionary<string, Type> _typeFactories = new Dictionary<string, Type>();

        /// <summary>
        /// The default view model factory which provides the ViewModel type as a parameter.
        /// </summary>
        static Func<Type, object> _defaultViewModelFactory = type => Activator.CreateInstance(type);

        /// <summary>
        /// ViewModelFactory that provides the View instance and ViewModel type as parameters.
        /// </summary>
        static Func<object, Type, object>? _defaultViewModelFactoryWithViewParameter;

        /// <summary>
        /// Default view type to view model type resolver, assumes the view model is in same assembly as the view type, but in the "ViewModels" namespace.
        /// </summary>
        static Func<Type, Type?> _defaultViewTypeToViewModelTypeResolver = DefaultViewTypeToViewModel;

        private static Type? DefaultViewTypeToViewModel(Type viewType)
        {
            var viewName = viewType.FullName;
            viewName = viewName?.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var suffix = viewName != null && viewName.EndsWith("View") ? "Model" : "ViewModel";
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
            return Type.GetType(viewModelName);
        }

        static Func<object, Type?> _defaultViewToViewModelTypeResolver = view => null;

        /// <summary>
        /// Sets the default view model factory.
        /// </summary>
        /// <param name="viewModelFactory">The view model factory which provides the ViewModel type as a parameter.</param>
        public static void SetDefaultViewModelFactory(Func<Type, object> viewModelFactory)
        {
            _defaultViewModelFactory = viewModelFactory;
        }

        /// <summary>
        /// Sets the default view model factory.
        /// </summary>
        /// <param name="viewModelFactory">The view model factory that provides the View instance and ViewModel type as parameters.</param>
        public static void SetDefaultViewModelFactory(Func<object, Type, object> viewModelFactory)
        {
            _defaultViewModelFactoryWithViewParameter = viewModelFactory;
        }

        /// <summary>
        /// Sets the default view type to view model type resolver.
        /// </summary>
        /// <param name="viewTypeToViewModelTypeResolver">The view type to view model type resolver.</param>
        public static void SetDefaultViewTypeToViewModelTypeResolver(Func<Type, Type?> viewTypeToViewModelTypeResolver)
        {
            _defaultViewTypeToViewModelTypeResolver = viewTypeToViewModelTypeResolver;
        }

        /// <summary>
        /// Sets the default ViewModel Type Resolver given the View instance. This can be used to evaluate the View for
        /// custom attributes or Attached Properties to determine the ViewModel Type.
        /// </summary> 
        public static void SetDefaultViewToViewModelTypeResolver(Func<object, Type?> viewToViewModelTypeResolver) =>
            _defaultViewToViewModelTypeResolver = viewToViewModelTypeResolver;

        /// <summary>
        /// Automatically looks up the viewmodel that corresponds to the current view, using two strategies:
        /// It first looks to see if there is a mapping registered for that view, if not it will fallback to the convention based approach.
        /// </summary>
        /// <param name="view">The dependency object, typically a view.</param>
        /// <param name="setDataContextCallback">The call back to use to create the binding between the View and ViewModel</param>
        public static void AutoWireViewModelChanged(object view, Action<object, object> setDataContextCallback)
        {
            // Try mappings first
            object? viewModel = GetViewModelForView(view);

            // try to use ViewModel type
            if (viewModel == null)
            {
                // check type mappings
                var viewModelType = GetViewModelTypeForView(view.GetType());

                // check platform View to ViewModel resolver
                if (viewModelType == null)
                    viewModelType = _defaultViewToViewModelTypeResolver(view);

                // fallback to convention based
                if (viewModelType == null)
                    viewModelType = _defaultViewTypeToViewModelTypeResolver(view.GetType());

                if (viewModelType == null)
                    return;

                viewModel = _defaultViewModelFactoryWithViewParameter != null ? _defaultViewModelFactoryWithViewParameter(view, viewModelType) : _defaultViewModelFactory(viewModelType);
            }

            setDataContextCallback(view, viewModel);
        }

        /// <summary>
        /// Gets the view model for the specified view.
        /// </summary>
        /// <param name="view">The view that the view model wants.</param>
        /// <returns>The ViewModel that corresponds to the view passed as a parameter.</returns>
        private static object? GetViewModelForView(object view)
        {
            var viewKey = view.GetType().ToString();

            // Mapping of view models base on view type (or instance) goes here
            return _factories.ContainsKey(viewKey) ? _factories[viewKey]() : null;
        }

        /// <summary>
        /// Gets the ViewModel type for the specified view.
        /// </summary>
        /// <param name="view">The View that the ViewModel wants.</param>
        /// <returns>The ViewModel type that corresponds to the View.</returns>
        private static Type? GetViewModelTypeForView(Type view)
        {
            var viewKey = view.ToString();

            return _typeFactories.ContainsKey(viewKey) ? _typeFactories[viewKey] : null;
        }

        /// <summary>
        /// Registers the ViewModel factory for the specified view type.
        /// </summary>
        /// <typeparam name="T">The View</typeparam>
        /// <param name="factory">The ViewModel factory.</param>
        public static void Register<T>(Func<object> factory)
        {
            Register(typeof(T).ToString(), factory);
        }

        /// <summary>
        /// Registers the ViewModel factory for the specified view type name.
        /// </summary>
        /// <param name="viewTypeName">The name of the view type.</param>
        /// <param name="factory">The ViewModel factory.</param>
        public static void Register(string viewTypeName, Func<object> factory)
        {
            _factories[viewTypeName] = factory;
        }

        /// <summary>
        /// Registers a ViewModel type for the specified view type.
        /// </summary>
        /// <typeparam name="T">The View</typeparam>
        /// <typeparam name="VM">The ViewModel</typeparam>
        public static void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] VM>()
        {
            var viewType = typeof(T);
            var viewModelType = typeof(VM);

            Register(viewType.ToString(), viewModelType);
        }

        /// <summary>
        /// Registers a ViewModel type for the specified view.
        /// </summary>
        /// <param name="viewTypeName">The View type name</param>
        /// <param name="viewModelType">The ViewModel type</param>
        public static void Register(string viewTypeName, Type viewModelType)
        {
            _typeFactories[viewTypeName] = viewModelType;
        }
    }
}

using System.Globalization;
using System.Reflection;

namespace Prism.Mvvm;

/// <summary>
/// The ViewModelLocationProvider2 class locates the view model for the view that has the AutoWireViewModelChanged attached property set to true.
/// The view model will be located and injected into the view's DataContext. To locate the view model, two strategies are used: First the ViewModelLocationProvider2
/// will look to see if there is a view model factory registered for that view, if not it will try to infer the view model using a convention based approach.
/// This class also provides methods for registering the view model factories,
/// and also to override the default view model factory and the default view type to view model type resolver.
/// </summary>
public static class ViewModelLocationProvider2
{
    ///// <summary>
    ///// A dictionary that contains all the registered factories for the views.
    ///// </summary>
    //static Dictionary<string, Func<object>> _factories = new Dictionary<string, Func<object>>();

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
    static Func<object, Type, object> _defaultViewModelFactoryWithViewParameter;

    /// <summary>
    /// Default view type to view model type resolver, assumes the view model is in same assembly as the view type, but in the "ViewModels" namespace.
    /// </summary>
    static Func<Type, Type> _defaultViewTypeToViewModelTypeResolver =
        viewType =>
        {
            var viewName = viewType.FullName;
            viewName = viewName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
            return Type.GetType(viewModelName);
        };

    static Func<object, Type> _defaultViewToViewModelTypeResolver = view => null;

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
    public static void SetDefaultViewTypeToViewModelTypeResolver(Func<Type, Type> viewTypeToViewModelTypeResolver)
    {
        _defaultViewTypeToViewModelTypeResolver = viewTypeToViewModelTypeResolver;
    }

    public static void SetDefaultViewToViewModelTypeResolver(Func<object, Type> viewToViewModelTypeResolver) =>
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
        object viewModel = null;

        var viewModelType = _defaultViewToViewModelTypeResolver(view);

        //check type mappings
        if (viewModelType == null)
            viewModelType = GetViewModelTypeForView(view.GetType());

        // fallback to convention based
        if (viewModelType == null)
            viewModelType = _defaultViewTypeToViewModelTypeResolver(view.GetType());

        if (viewModelType == null)
            return;

        viewModel = _defaultViewModelFactoryWithViewParameter != null ? _defaultViewModelFactoryWithViewParameter(view, viewModelType) : _defaultViewModelFactory(viewModelType);


        setDataContextCallback(view, viewModel);
    }

    /// <summary>
    /// Gets the ViewModel type for the specified view.
    /// </summary>
    /// <param name="view">The View that the ViewModel wants.</param>
    /// <returns>The ViewModel type that corresponds to the View.</returns>
    private static Type GetViewModelTypeForView(Type view)
    {
        var viewKey = view.ToString();

        if (_typeFactories.ContainsKey(viewKey))
            return _typeFactories[viewKey];

        return null;
    }

    /// <summary>
    /// Registers a ViewModel type for the specified view type.
    /// </summary>
    /// <typeparam name="T">The View</typeparam>
    /// <typeparam name="VM">The ViewModel</typeparam>
    public static void Register<T, VM>()
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
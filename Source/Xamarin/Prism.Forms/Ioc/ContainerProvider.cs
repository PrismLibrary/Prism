namespace Prism.Ioc
{
    /// <summary>
    /// Provides Types and Services registered with the Container
    /// </summary>
    /// <typeparam name="T">The type to Resolve</typeparam>
    /// <example>
    /// We can use this to build better types such as ValueConverters with full dependency injection
    /// <code>
    /// public class MyValueConverter : IValueConverter
    /// {
    ///     private ILoggerFacade _logger { get; }
    ///     public MyValueConverter(ILoggerFacade logger)
    ///     {
    ///         _logger = logger;
    ///     }
    /// 
    ///     public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    ///     {
    ///         _logger.Log($"Converting {value.GetType().Name} to {targetType.Name}", Category.Debug, Priority.None);
    ///         // do stuff
    ///     }
    /// 
    ///     public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    ///     {
    ///         _logger.Log($"Converting back from {value.GetType().Name} to {targetType.Name}", Category.Debug, Priority.None);
    ///         return null;
    ///     }
    /// }
    /// </code>
    /// We can then simply use our ValueConveter or other class directly in XAML
    /// <![CDATA[
    /// <ContentPage xmlns:prism="clr-namespace:Prism.Ioc;assembly=Prism.Forms">
    ///     <ContentPage.Resources>
    ///         <ResourceDictionary>
    ///             <prism:ContainerProvider x:TypeArguments="MyValueConverter" x:Key="myValueConverter" />
    ///         <ResourceDictionary>
    ///     <ContentPage.Resources>
    ///     <Label Text="{Binding SomeProp,Converter={StaticResource myValueConverter}}" />
    /// </ContentPage>
    /// ]]>
    /// </example>
    public class ContainerProvider<T>
    {
        /// <summary>
        /// The Name used to register the type with the Container
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Resolves the specified type from the Application's Container
        /// </summary>
        /// <param name="containerProvider"></param>
        public static implicit operator T(ContainerProvider<T> containerProvider)
        {
            var container = PrismApplicationBase.Current.Container;
            if (container == null) return default(T);
            if (string.IsNullOrWhiteSpace(containerProvider.Name))
            {
                return container.Resolve<T>();
            }

            return container.Resolve<T>(containerProvider.Name);
        }
    }
}

using System;
using System.Windows.Markup;

namespace Prism.Ioc
{
    /// <summary>
    /// Provides Types and Services registered with the Container
    /// </summary>
    /// <example>
    /// <para>
    /// Usage as markup extension:
    /// <![CDATA[
    ///   <TextBlock
    ///     Text="{Binding
    ///       Path=Foo,
    ///       Converter={prism:ContainerProvider {x:Type local:MyConverter}}}" />
    /// ]]>
    /// </para>
    /// <para>
    /// Usage as XML element:
    /// <![CDATA[
    ///   <Window>
    ///     <Window.DataContext>
    ///       <prism:ContainerProvider Type="{x:Type local:MyViewModel}" />
    ///     </Window.DataContext>
    ///   </Window>
    /// ]]>
    /// </para>
    /// </example>
    public class ContainerProviderExtension : MarkupExtension
    {
        public ContainerProviderExtension()
        {
        }

        public ContainerProviderExtension(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// The type to Resolve
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The Name used to register the type with the Container
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Provide resolved object from <see cref="ContainerLocator"/>
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return string.IsNullOrEmpty(Name)
                ? ContainerLocator.Container?.Resolve(Type)
                : ContainerLocator.Container?.Resolve(Type, Name);
        }
    }
}

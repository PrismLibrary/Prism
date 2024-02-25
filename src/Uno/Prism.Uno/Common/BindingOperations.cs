namespace Prism;

internal static class BindingOperations
{
    public static BindingExpression GetBinding(FrameworkElement instance, DependencyProperty property) =>
        instance.GetBindingExpression(property);
}

namespace Prism.Dialogs;

internal class DialogContainerView : ContentView
{
    public static readonly BindableProperty IsPageContentProperty =
        BindableProperty.Create(nameof(IsPageContent), typeof(bool), typeof(DialogContainerView), false);

    public static readonly BindableProperty IsPopupContentProperty =
        BindableProperty.Create(nameof(IsPopupContent), typeof(bool), typeof(DialogContainerView), false);

    public bool IsPageContent
    {
        get => (bool)GetValue(IsPageContentProperty);
        set => SetValue(IsPageContentProperty, value);
    }

    public bool IsPopupContent
    {
        get => (bool)GetValue(IsPopupContentProperty);
        set => SetValue(IsPopupContentProperty, value);
    }
}

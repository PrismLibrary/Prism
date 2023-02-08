namespace Prism.Services.Xaml;

public static class DialogLayout
{
    /// <summary>
    /// Gets the key for specifying or retrieving popup overlay style from Application Resources.
    /// </summary>
    public const string PopupOverlayStyle = "PrismDialogMaskStyle";

    public static readonly BindableProperty RelativeWidthRequestProperty =
        BindableProperty.CreateAttached("RelativeWidthRequest", typeof(double?), typeof(DialogLayout), null);

    public static double? GetRelativeWidthRequest(BindableObject bindable) =>
        (double?)bindable.GetValue(RelativeWidthRequestProperty);

    public static void SetRelativeWidthRequest(BindableObject bindable, double? value) =>
        bindable.SetValue(RelativeWidthRequestProperty, value);

    public static readonly BindableProperty RelativeHeightRequestProperty =
        BindableProperty.CreateAttached("RelativeHeightRequest", typeof(double?), typeof(DialogLayout), null);

    public static double? GetRelativeHeightRequest(BindableObject bindable) =>
        (double?)bindable.GetValue(RelativeHeightRequestProperty);

    public static void SetRelativeHeightRequest(BindableObject bindable, double? value) =>
        bindable.SetValue(RelativeHeightRequestProperty, value);

    public static readonly BindableProperty LayoutBoundsProperty =
        BindableProperty.CreateAttached("LayoutBounds", typeof(Rect), typeof(DialogLayout), new Rect(0.5, 0.5, -1, -1));

    public static Rect GetLayoutBounds(BindableObject bindable) =>
        (Rect)bindable.GetValue(LayoutBoundsProperty);

    public static void SetLayoutBounds(BindableObject bindable, Rect value) =>
        bindable.SetValue(LayoutBoundsProperty, value);

    public static readonly BindableProperty MaskStyleProperty =
        BindableProperty.CreateAttached("MaskStyle", typeof(Style), typeof(DialogLayout), null);

    public static Style GetMaskStyle(BindableObject bindable) =>
        (Style)bindable.GetValue(MaskStyleProperty);

    public static void SetMaskStyle(BindableObject bindable, Style value) =>
        bindable.SetValue(MaskStyleProperty, value);

    public static readonly BindableProperty MaskProperty =
        BindableProperty.CreateAttached("Mask", typeof(View), typeof(DialogLayout), null);

    public static View GetMask(BindableObject bindable) =>
        (View)bindable.GetValue(MaskProperty);

    public static void SetMask(BindableObject bindable, View value) =>
        bindable.SetValue(MaskProperty, value);

    public static readonly BindableProperty UseMaskProperty =
        BindableProperty.CreateAttached("UseMask", typeof(bool?), typeof(DialogLayout), null);

    public static bool? GetUseMask(BindableObject bindable)
    {
        var value = bindable.GetValue(UseMaskProperty);
        if (value is bool boolean)
            return boolean;

        return true;
    }

    public static void SetUseMask(BindableObject bindable, bool? value) =>
        bindable.SetValue(UseMaskProperty, value);

    public static readonly BindableProperty CloseOnBackgroundTappedProperty =
        BindableProperty.CreateAttached("CloseOnBackgroundTapped", typeof(bool?), typeof(DialogLayout), null);

    public static bool? GetCloseOnBackgroundTapped(BindableObject bindable) =>
        (bool?)bindable.GetValue(CloseOnBackgroundTappedProperty);

    public static void SetCloseOnBackgroundTapped(BindableObject bindable, bool? value) =>
        bindable.SetValue(CloseOnBackgroundTappedProperty, value);
}

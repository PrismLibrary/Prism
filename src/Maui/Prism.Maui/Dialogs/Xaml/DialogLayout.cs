namespace Prism.Dialogs.Xaml;

/// <summary>
/// Provides attached properties for customizing the layout and behavior of dialogs in .NET MAUI.
/// </summary>
public static class DialogLayout
{
    /// <summary>
    /// Gets the key for specifying or retrieving popup overlay style from Application Resources.
    /// </summary>
    public const string PopupOverlayStyle = "PrismDialogMaskStyle";

    /// <summary>
    /// Attached property that specifies the desired relative width of the dialog.
    /// </summary>
    public static readonly BindableProperty RelativeWidthRequestProperty =
        BindableProperty.CreateAttached("RelativeWidthRequest", typeof(double?), typeof(DialogLayout), null);

    /// <summary>
    /// Gets the value of the RelativeWidthRequest attached property from the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to get the property value from.</param>
    /// <returns>The value of the RelativeWidthRequest property, or null if not set.</returns>
    public static double? GetRelativeWidthRequest(BindableObject bindable) =>
        (double?)bindable.GetValue(RelativeWidthRequestProperty);

    /// <summary>
    /// Sets the value of the RelativeWidthRequest attached property on the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to set the property value on.</param>
    /// <param name="value">The desired relative width of the dialog.</param>
    public static void SetRelativeWidthRequest(BindableObject bindable, double? value) =>
        bindable.SetValue(RelativeWidthRequestProperty, value);

    /// <summary>
    /// Attached property that specifies the desired relative height of the dialog.
    /// </summary>
    public static readonly BindableProperty RelativeHeightRequestProperty =
        BindableProperty.CreateAttached("RelativeHeightRequest", typeof(double?), typeof(DialogLayout), null);

    /// <summary>
    /// Gets the value of the RelativeHeightRequest attached property from the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to get the property value from.</param>
    /// <returns>The value of the RelativeHeightRequest property, or null if not set.</returns>
    public static double? GetRelativeHeightRequest(BindableObject bindable) =>
        (double?)bindable.GetValue(RelativeHeightRequestProperty);

    /// <summary>
    /// Sets the value of the RelativeHeightRequest attached property on the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to set the property value on.</param>
    /// <param name="value">The desired relative height of the dialog.</param>
    public static void SetRelativeHeightRequest(BindableObject bindable, double? value) =>
        bindable.SetValue(RelativeHeightRequestProperty, value);

    /// <summary>
    /// Attached property that specifies the layout bounds of the dialog within its parent.
    /// </summary>
    public static readonly BindableProperty LayoutBoundsProperty =
        BindableProperty.CreateAttached("LayoutBounds", typeof(Rect), typeof(DialogLayout), new Rect(0.5, 0.5, -1, -1));

    /// <summary>
    /// Gets the value of the LayoutBounds attached property from the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to get the property value from.</param>
    /// <returns>The layout bounds of the dialog.</returns>
    public static Rect GetLayoutBounds(BindableObject bindable) =>
        (Rect)bindable.GetValue(LayoutBoundsProperty);

    /// <summary>
    /// Sets the value of the LayoutBounds attached property on the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to set the property value on.</param>
    /// <param name="value">The desired layout bounds for the dialog.</param>
    public static void SetLayoutBounds(BindableObject bindable, Rect value) =>
        bindable.SetValue(LayoutBoundsProperty, value);

    /// <summary>
    /// Attached property that specifies the style to apply to the dialog mask (background overlay).
    /// </summary>
    public static readonly BindableProperty MaskStyleProperty =
        BindableProperty.CreateAttached("MaskStyle", typeof(Style), typeof(DialogLayout), null);

    /// <summary>
    /// Gets the style applied to the dialog mask (background overlay) from the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to get the mask style from.</param>
    /// <returns>The style applied to the dialog mask, or null if not set.</returns>
    public static Style GetMaskStyle(BindableObject bindable) =>
        (Style)bindable.GetValue(MaskStyleProperty);

    /// <summary>
    /// Sets the style to be applied to the dialog mask (background overlay) on the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to set the mask style on.</param>
    /// <param name="value">The style to apply to the dialog mask.</param>
    public static void SetMaskStyle(BindableObject bindable, Style value) =>
        bindable.SetValue(MaskStyleProperty, value);

    /// <summary>
    /// Attached property that specifies the View to be used as the dialog mask (background overlay).
    /// </summary>
    public static readonly BindableProperty MaskProperty =
        BindableProperty.CreateAttached("Mask", typeof(View), typeof(DialogLayout), null);

    /// <summary>
    /// Gets the View used as the dialog mask (background overlay) from the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to get the mask view from.</param>
    /// <returns>The View used as the dialog mask, or null if not set.</returns>
    public static View GetMask(BindableObject bindable) =>
        (View)bindable.GetValue(MaskProperty);

    /// <summary>
    /// Sets the View to be used as the dialog mask (background overlay) on the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to set the mask view on.</param>
    /// <param name="value">The View to use as the dialog mask.</param>
    public static void SetMask(BindableObject bindable, View value) =>
        bindable.SetValue(MaskProperty, value);

    /// <summary>
    /// Attached property that specifies whether a dialog should use a mask (background overlay).
    /// </summary>
    public static readonly BindableProperty UseMaskProperty =
        BindableProperty.CreateAttached("UseMask", typeof(bool?), typeof(DialogLayout), null);

    /// <summary>
    /// Gets whether a mask is used for the dialog from the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to get the mask usage from.</param>
    /// <returns>True if a mask is used for the dialog, false if not, or null if not explicitly set.</returns>
    public static bool? GetUseMask(BindableObject bindable)
    {
        // Default to using a mask if not explicitly set
        return bindable.GetValue(UseMaskProperty) is bool boolean ? boolean : true;
    }

    /// <summary>
    /// Sets whether a mask should be used for the dialog on the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to set the mask usage on.</param>
    /// <param name="value">True to use a mask for the dialog, false to not use a mask, or null to use the default behavior.</param>
    public static void SetUseMask(BindableObject bindable, bool? value) =>
        bindable.SetValue(UseMaskProperty, value);

    /// <summary>
    /// Attached property that specifies whether the dialog should close when the background is tapped.
    /// </summary>
    public static readonly BindableProperty CloseOnBackgroundTappedProperty =
        BindableProperty.CreateAttached("CloseOnBackgroundTapped", typeof(bool?), typeof(DialogLayout), null);

    /// <summary>
    /// Gets whether the dialog closes when the background is tapped from the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to get the close-on-background-tap behavior from.</param>
    /// <returns>True if the dialog closes on background tap, false if not, or null if not explicitly set.</returns>
    public static bool? GetCloseOnBackgroundTapped(BindableObject bindable) =>
        (bool?)bindable.GetValue(CloseOnBackgroundTappedProperty);

    /// <summary>
    /// Sets whether the dialog should close when the background is tapped on the specified BindableObject.
    /// </summary>
    /// <param name="bindable">The BindableObject to set the close-on-background-tap behavior on.</param>
    /// <param name="value">True to close the dialog on background tap, false to not close on background tap, or null to use the default behavior.</param>
    public static void SetCloseOnBackgroundTapped(BindableObject bindable, bool? value) =>
        bindable.SetValue(CloseOnBackgroundTappedProperty, value);

}

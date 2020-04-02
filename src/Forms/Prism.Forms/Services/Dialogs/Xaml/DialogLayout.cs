using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Prism.Services.Dialogs.Xaml
{
    public static class DialogLayout
    {
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
            BindableProperty.CreateAttached("LayoutBounds", typeof(Rectangle), typeof(DialogLayout), new Rectangle(0.5, 0.5, -1, -1));

        public static Rectangle GetLayoutBounds(BindableObject bindable) =>
            (Rectangle)bindable.GetValue(LayoutBoundsProperty);

        public static void SetLayoutBounds(BindableObject bindable, Rectangle value) =>
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

        public static bool? GetUseMask(BindableObject bindable) =>
            (bool?)bindable.GetValue(UseMaskProperty);

        public static void SetUseMask(BindableObject bindable, bool? value) =>
            bindable.SetValue(UseMaskProperty, value);

        public static readonly BindableProperty CloseOnBackgroundTappedProperty =
            BindableProperty.CreateAttached("CloseOnBackgroundTapped", typeof(bool?), typeof(DialogLayout), null);

        public static bool? GetCloseOnBackgroundTapped(BindableObject bindable) =>
            (bool?)bindable.GetValue(CloseOnBackgroundTappedProperty);

        public static void SetCloseOnBackgroundTapped(BindableObject bindable, bool? value) =>
            bindable.SetValue(CloseOnBackgroundTappedProperty, value);
    }
}

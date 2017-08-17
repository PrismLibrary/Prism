using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Prism.Mvvm
{
    public static class ChildViewRegistry
    {
        public static readonly BindableProperty ChildViewsProperty =
            BindableProperty.CreateAttached("ChildViews", typeof(IList<Element>), typeof(ChildViewRegistry), new List<Element>());

        public static IList<Element> GetChildViews(BindableObject bindable) =>
            (IList<Element>)bindable.GetValue(ChildViewsProperty);

        public static void AddChildView(BindableObject bindable, Element element) =>
            GetChildViews(bindable).Add(element);
    }
}

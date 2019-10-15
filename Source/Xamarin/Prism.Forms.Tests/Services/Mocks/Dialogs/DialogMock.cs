using System;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Services.Mocks.Dialogs
{
    public class DialogMock : StackLayout
    {
        public const string Message = "This is a Dialog";
        public static DialogMock Current { get; private set; }
        public DialogMockViewModel ViewModel => BindingContext as DialogMockViewModel;

        internal static Action<View> ConstructorCallback { get; set; }

        public DialogMock()
        {
            Current = this;
            Children.Add(new Label { Text = Message });
            var title = new Label();
            title.SetBinding(Label.TextProperty, "Title");
            Children.Add(title);
            ConstructorCallback?.Invoke(this);
        }
    }
}

using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using Prism.Dialogs;
using Prism.Dialogs.Xaml;
using Prism.Maui.Tests.Mocks;
using Prism.Xaml;
using Xunit;

namespace Prism.Maui.Tests;

public class TypedBindingFixture
{
    public TypedBindingFixture() => DispatcherProvider.SetCurrent(TestDispatcher.Provider);

    [Theory]
    [InlineData(TargetBindingContext.Element)]
    [InlineData(TargetBindingContext.Page)]
    public void TargetBindingContextTracksChangesAndReplacement(TargetBindingContext source)
    {
        var original = new ContentPage { BindingContext = new object() };
        var extension = new TestExtension { TargetBindingContext = source };
        extension.SetSource(original);
        Assert.Same(original.BindingContext, extension.BindingContext);

        original.BindingContext = new object();
        Assert.Same(original.BindingContext, extension.BindingContext);

        var replacement = new ContentPage { BindingContext = new object() };
        extension.SetSource(replacement);
        Assert.Same(replacement.BindingContext, extension.BindingContext);

        original.BindingContext = new object();
        Assert.Same(replacement.BindingContext, extension.BindingContext);
        replacement.BindingContext = new object();
        Assert.Same(replacement.BindingContext, extension.BindingContext);
    }

    [Fact]
    public void RelativeDialogSizeTracksContainerSize()
    {
        var dialog = new ContentView();
        DialogLayout.SetUseMask(dialog, false);
        DialogLayout.SetRelativeWidthRequest(dialog, 0.5);
        DialogLayout.SetRelativeHeightRequest(dialog, 0.75);
        var container = new TestDialogContainer();
        container.Layout(new Rect(0, 0, 400, 800));

        var overlay = Assert.IsType<Grid>(container.CreateLayout(dialog));
        var popupArea = Assert.IsType<AbsoluteLayout>(overlay.Children.Single());
        var popup = Assert.IsAssignableFrom<View>(popupArea.Children.Single());
        Assert.Equal(200, popup.WidthRequest);
        Assert.Equal(600, popup.HeightRequest);

        container.Layout(new Rect(0, 0, 600, 400));
        Assert.Equal(300, popup.WidthRequest);
        Assert.Equal(300, popup.HeightRequest);
    }

    private sealed class TestExtension : TargetAwareExtensionBase<object>
    {
        public void SetSource(ContentPage page)
        {
            if (TargetBindingContext == TargetBindingContext.Element)
                TargetElement = page;
            else
                Page = page;
        }

        protected override object ProvideValue(IServiceProvider serviceProvider) => BindingContext;
    }

    private sealed class TestDialogContainer : DialogContainerPage
    {
        public View CreateLayout(View dialog) =>
            GetContentLayout(new ContentPage(), dialog, false, new Command(() => { }), new DialogParameters());
    }
}
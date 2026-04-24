#nullable enable
using Microsoft.Xaml.Interactivity;
using Prism.Dialogs;
using Prism.Interactivity;
using Xunit;

namespace Prism.Uno.WinUI.Tests;

public class DialogAndInteractivityFixture
{
    [Fact]
    public void KnownDialogParametersContainsUnoDialogPlacementKey()
    {
        Assert.Equal("dialogPlacement", KnownDialogParameters.DialogPlacement);
    }

    [Fact]
    public void DialogServiceExposesShowDialogMethod()
    {
        var method = typeof(DialogService).GetMethod(nameof(DialogService.ShowDialog));

        Assert.NotNull(method);
        Assert.Equal(typeof(void), method!.ReturnType);
        Assert.Equal(3, method.GetParameters().Length);
    }

    [Fact]
    public void InvokeCommandActionImplementsIAction()
    {
        Assert.Contains(typeof(IAction), typeof(InvokeCommandAction).GetInterfaces());
    }

    [Fact]
    public void DialogServiceExposesExpectedInternalConfigurationMethods()
    {
        Assert.NotNull(typeof(DialogService).GetMethod("CreateDialogWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        Assert.NotNull(typeof(DialogService).GetMethod("ConfigureDialogWindowContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        Assert.NotNull(typeof(DialogService).GetMethod("ConfigureDialogWindowEvents", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
    }

    [Fact]
    public void DialogWindowImplementsIDialogWindowContract()
    {
        Assert.Contains(typeof(IDialogWindow), typeof(DialogWindow).GetInterfaces());

        var showAsync = typeof(IDialogWindow).GetMethod("ShowAsync");
        var hide = typeof(IDialogWindow).GetMethod("Hide");
        Assert.NotNull(showAsync);
        Assert.NotNull(hide);
    }

    [Fact]
    public void CreateDialogWindowUsesContainerResolveForNamedWindow()
    {
        var container = new Moq.Mock<Prism.Ioc.IContainerProvider>();
        container.Setup(x => x.Resolve(typeof(IDialogWindow), "MainWindow")).Returns(new TestDialogWindow());
        var service = new DialogService(container.Object);
        var method = typeof(DialogService).GetMethod("CreateDialogWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var result = method!.Invoke(service, new object?[] { "MainWindow" });

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IDialogWindow>(result);
    }

    [Fact]
    public void ConfigureDialogWindowContentThrowsForNonFrameworkElement()
    {
        var container = new Moq.Mock<Prism.Ioc.IContainerProvider>();
        container.Setup(x => x.Resolve(typeof(object), "SampleDialog")).Returns(new object());
        var service = new DialogService(container.Object);
        var method = typeof(DialogService).GetMethod("ConfigureDialogWindowContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var ex = Assert.Throws<System.Reflection.TargetInvocationException>(() =>
            method!.Invoke(service, new object[] { "SampleDialog", new TestDialogWindow(), new DialogParameters() }));

        Assert.IsType<NullReferenceException>(ex.InnerException);
    }

    private sealed class TestDialogWindow : IDialogWindow
    {
        public object? DataContext { get; set; }
        public Style Style { get; set; } = null!;
        public event RoutedEventHandler? Loaded;
        public event Windows.Foundation.TypedEventHandler<ContentDialog, ContentDialogClosingEventArgs>? Closing;
        public event Windows.Foundation.TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs>? Closed;
        public IDialogResult? Result { get; set; }
        public object? Content { get; set; }

        public Windows.Foundation.IAsyncOperation<ContentDialogResult> ShowAsync(ContentDialogPlacement placement) => throw new NotImplementedException();

        public void Hide()
        {
        }
    }
}

using System.Windows.Controls;
using System.Windows.Threading;
using Moq;
using Prism.Dialogs;
using Prism.Ioc;
using Prism.Wpf.Tests.Mocks;
using Xunit;

namespace Prism.Wpf.Tests.Dialogs;

public class DialogCallerCloseFixture
{
    [StaFact]
    public void ModalWorkStartsWhileDisplayedAndBackgroundCloseHonorsVeto()
    {
        var model = new DialogCallerModel { AllowClose = false };
        var owner = new Window { Width = 1, Height = 1, Opacity = 0, ShowInTaskbar = false };
        owner.Show();
        var window = new DialogWindow { Width = 1, Height = 1, Opacity = 0, ShowInTaskbar = false,
            Owner = owner };
        var container = new Mock<IContainerExtension>();
        var registry = new Mock<IDialogViewRegistry>();
        container.Setup(c => c.Resolve(typeof(IDialogWindow))).Returns(window);
        registry.Setup(r => r.CreateView(container.Object, "Busy"))
            .Returns(new UserControl { DataContext = model });
        var service = new DialogService(container.Object, registry.Object);
        var timedOut = false;
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
        timer.Tick += (_, _) => { timedOut = true; model.AllowClose = true; window.Close(); };
        var opened = false;
        var callbacks = 0;
        Exception error = null;
        DialogCloseListener saved = default;
        Task work = null;
        timer.Start();
        try
        {
            service.ShowDialog("Busy", new DialogParameters(), new DialogCallback()
                .OnOpenedAsync(async listener =>
                {
                    saved = listener;
                    opened = true;
                    Assert.True(window.IsVisible);
                    await listener.InvokeAsync(new DialogResult(ButtonResult.Cancel));
                    Assert.Equal(0, model.ClosedCount);
                    model.AllowClose = true;
                    work = Task.Run(async () => await listener.InvokeAsync(new DialogResult(ButtonResult.OK)));
                    await work;
                })
                .OnClose(result => { callbacks++; Assert.Equal(ButtonResult.OK, result.Result); })
                .OnError(ex => error = ex));
        }
        finally { timer.Stop(); owner.Close(); }
        Assert.False(timedOut);
        Assert.True(opened);
        Assert.Null(error);
        Assert.Equal(1, callbacks);
        Assert.Equal(1, model.ClosedCount);
        // The worker may still be completing its posted close; after closure all
        // copies have released the window and another request completes immediately.
        Assert.True(saved.InvokeAsync().IsCompleted);
        registry.Verify(r => r.CreateView(container.Object, "Busy"), Times.Once);
        Assert.Null(window.DataContext);
        Assert.Null(window.Content);
    }
}

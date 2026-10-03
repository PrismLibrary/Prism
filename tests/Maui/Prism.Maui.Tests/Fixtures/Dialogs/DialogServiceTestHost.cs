using System.Windows.Input;
using Prism.Dialogs;

#nullable enable
namespace Prism.Maui.Tests.Fixtures.Dialogs;

internal sealed class DialogServiceTestHost : ContentPage, IDialogContainer
{
    public Task PushGate { get; set; } = Task.CompletedTask;
    public Task PopGate { get; set; } = Task.CompletedTask;
    public Action? AfterPush { get; set; }
    public TaskCompletionSource PopEntered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public int PopCount { get; private set; }
    public View DialogView { get; private set; } = null!;
    public ICommand Dismiss { get; private set; } = null!;

    public async Task ConfigureLayout(Page page, View view, bool hide, ICommand dismiss, IDialogParameters parameters)
    {
        DialogView = view;
        Dismiss = dismiss;
        Content = view;
        await PushGate;
        await page.Navigation.PushModalAsync(this, false);
        IDialogContainer.DialogStack.Add(this);
        AfterPush?.Invoke();
    }

    public async Task DoPop(Page page)
    {
        PopCount++;
        PopEntered.TrySetResult();
        await PopGate;
        if (page.Navigation.ModalStack.LastOrDefault() != this)
            throw new InvalidOperationException("Another modal covers this dialog.");
        await page.Navigation.PopModalAsync(false);
        if (page.Navigation.ModalStack.Contains(this))
            throw new InvalidOperationException("PrismWindow canceled the native pop.");
        IDialogContainer.DialogStack.Remove(this);
    }
}

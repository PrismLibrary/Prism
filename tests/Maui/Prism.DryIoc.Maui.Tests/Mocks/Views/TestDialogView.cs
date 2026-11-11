#nullable enable

using Prism.Dialogs;

namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

/// <summary>Border-based dialog; auto-closes on <see cref="VisualElement.Loaded"/> after <see cref="IDialogContainer.DialogView"/> is wired.</summary>
public class TestDialogView : Border
{
    public TestDialogView()
    {
        Content = new Label { Text = "TestDialog" };
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        Loaded -= OnLoaded;
        if (BindingContext is IDialogAware aware)
            aware.RequestClose.Invoke(ButtonResult.OK);
    }
}

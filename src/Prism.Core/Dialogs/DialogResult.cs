using System;

#nullable enable
namespace Prism.Dialogs;

#if NET6_0_OR_GREATER
public record DialogResult : IDialogResult
{
    public Exception? Exception { get; init; }

    public IDialogParameters Parameters { get; init; }
}
#else
public class DialogResult : IDialogResult
{
    public Exception? Exception { get; set; }

    public IDialogParameters Parameters { get; set; }
}
#endif

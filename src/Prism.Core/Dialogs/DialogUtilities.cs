using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Prism.Dialogs;

/// <summary>
/// Provides utilities for the Dialog Service to be able to reuse
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class DialogUtilities
{
    /// <summary>
    /// Initializes <see cref="IDialogAware.RequestClose"/>
    /// </summary>
    /// <param name="dialogAware"></param>
    /// <param name="callback"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void InitializeListener(IDialogAware dialogAware, Func<IDialogResult, Task> callback)
    {
        var listener = new DialogCloseListener(callback);
        SetListener(dialogAware, listener);
    }

    /// <summary>
    /// Initializes <see cref="IDialogAware.RequestClose"/>
    /// </summary>
    /// <param name="dialogAware"></param>
    /// <param name="callback"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void InitializeListener(IDialogAware dialogAware, Action<IDialogResult> callback)
    {
        var listener = new DialogCloseListener(callback);
        SetListener(dialogAware, listener);
    }

    private static void SetListener(IDialogAware dialogAware, DialogCloseListener listener)
    {
        var type = dialogAware.GetType();
        var propInfo = type.GetProperty(nameof(IDialogAware.RequestClose));

        if(propInfo is not null && propInfo.PropertyType == typeof(DialogCloseListener) && propInfo.SetMethod is not null)
        {
            propInfo.SetValue(dialogAware, listener);
            return;
        }

        var fields = type.GetRuntimeFields().Where(x => x.FieldType == typeof(DialogCloseListener));
        var field = fields.FirstOrDefault(x => x.Name == $"<{nameof(IDialogAware.RequestClose)}>k__BackingField");
        if (field is not null)
        {
            field.SetValue(dialogAware, listener);
        }
        else if (fields.Any())
        {
            field = fields.First();
            field.SetValue(dialogAware, listener);
        }
        else
        {
            throw new DialogException(DialogException.UnableToSetTheDialogCloseListener);
        }
    }
}

namespace Prism.Services;

/// <summary>
/// Represents a button displayed in <see cref="IPageDialogService.DisplayActionSheetAsync(string, IActionSheetButton[])"/>
/// </summary>
public class ActionSheetButton : ActionSheetButtonBase
{
    /// <summary>
    /// Represents a button in an action sheet dialog.
    /// </summary>
    protected internal ActionSheetButton()
    {

    }

    /// <summary>
    /// Gets or sets the action to perform when the button is pressed.
    /// </summary>
    /// <value>The action.</value>
    public Action Action { get; protected set; }

    /// <summary>
    /// Gets or sets the asynchronous callback to execute when the button is pressed.
    /// </summary>
    public Func<Task> AsyncCallback { get; protected set; }

    /// <summary>
    /// Executes the action to take when the button is pressed.
    /// </summary>
    protected override async Task OnButtonPressed()
    {
        Action?.Invoke();
        if (AsyncCallback != null)
            await AsyncCallback();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as a "cancel button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateCancelButton(string text) => CreateCancelButton(text, default);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as a "cancel button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="action">The action to execute when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateCancelButton(string text, Action action) =>
        CreateButtonInternal(text, action, null, isCancel: true);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as a "cancel button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="asyncCallback">The action to execute when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateCancelButton(string text, Func<Task> asyncCallback) =>
        CreateButtonInternal(text, null, asyncCallback, isCancel: true);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as a "cancel button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="action">The action to execute when the button is pressed.</param>
    /// <param name="parameter">The parameter to pass to the action when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateCancelButton<T>(string text, Action<T> action, T parameter) =>
        CreateButtonInternal(text, action, null, parameter, isCancel: true);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as a "cancel button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="asyncCallback">The action to execute when the button is pressed.</param>
    /// <param name="parameter">The parameter to pass to the action when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateCancelButton<T>(string text, Func<T, Task> asyncCallback, T parameter) =>
        CreateButtonInternal(text, null, asyncCallback, parameter, isCancel: true);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as a "destroy button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateDestroyButton(string text) =>
        CreateDestroyButton(text, default);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as a "destroy button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="action">The action to execute when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateDestroyButton(string text, Action action) =>
        CreateButtonInternal(text, action, null, isDestroy: true);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as a "destroy button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="asyncCallback">The action to execute when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateDestroyButton(string text, Func<Task> asyncCallback) =>
        CreateButtonInternal(text, null, asyncCallback, isDestroy: true);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as a "destroy button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="action">The action to execute when the button is pressed.</param>
    /// <param name="parameter">The parameter to pass to the action when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateDestroyButton<T>(string text, Action<T> action, T parameter) =>
        CreateButtonInternal(text, action, null, parameter, isDestroy: true);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as a "destroy button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="asyncCallback">The action to execute when the button is pressed.</param>
    /// <param name="parameter">The parameter to pass to the action when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateDestroyButton<T>(string text, Func<T, Task> asyncCallback, T parameter) =>
        CreateButtonInternal(text, null, asyncCallback, parameter, isDestroy: true);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as an "other button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="action">The action to execute when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateButton(string text, Action action) =>
        CreateButtonInternal(text, action, null);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as an "other button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="asyncCallback">The action to execute when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateButton(string text, Func<Task> asyncCallback) =>
        CreateButtonInternal(text, null, asyncCallback);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as an "other button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="action">The action to execute when the button is pressed.</param>
    /// <param name="parameter">The parameter to pass to the action when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateButton<T>(string text, Action<T> action, T parameter) =>
        CreateButtonInternal(text, action, null, parameter);

    /// <summary>
    /// Creates a new instance of <see cref="ActionSheetButton"/> that displays as an "other button".
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="asyncCallback">The action to execute when the button is pressed.</param>
    /// <param name="parameter">The parameter to pass to the action when the button is pressed.</param>
    /// <returns>An instance of <see cref="ActionSheetButton"/>.</returns>
    public static IActionSheetButton CreateButton<T>(string text, Func<T, Task> asyncCallback, T parameter) =>
        CreateButtonInternal(text, null, asyncCallback, parameter);

    private static IActionSheetButton CreateButtonInternal(string text, Action action, Func<Task> asyncCallback, bool isCancel = false, bool isDestroy = false) =>
        new ActionSheetButton()
        {
            Text = text,
            Action = action,
            AsyncCallback = asyncCallback,
            IsCancel = isCancel,
            IsDestroy = isDestroy,
        };

    private static IActionSheetButton CreateButtonInternal<T>(string text, Action<T> action, Func<T, Task> asyncCallback, T parameter, bool isCancel = false, bool isDestroy = false) =>
        new ActionSheetButton<T>()
        {
            Text = text,
            Action = action,
            AsyncCallback = asyncCallback,
            Parameter = parameter,
            IsCancel = isCancel,
            IsDestroy = isDestroy,
        };
}

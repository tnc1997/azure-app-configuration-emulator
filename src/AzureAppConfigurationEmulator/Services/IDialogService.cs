using Microsoft.AspNetCore.Components;

namespace AzureAppConfigurationEmulator.Services;

/// <summary>
/// The reference to the dialog that is shown.
/// </summary>
public interface IDialogReference
{
    /// <summary>
    /// Gets a task that completes with the <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/dialog">element</see> that is rendered.
    /// </summary>
    public Task<ElementReference> Element { get; }

    /// <summary>
    /// Gets the parameters to pass to the component.
    /// </summary>
    public IDictionary<string, object?>? Parameters { get; }

    /// <summary>
    /// Gets a task that completes with the result that is returned when the dialog is closed.
    /// </summary>
    public Task<IDialogResult?> Result { get; }

    /// <summary>
    /// Gets the type of the component that represents the dialog.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Tries to set the <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/dialog">element</see> that is rendered.
    /// </summary>
    /// <param name="element">The <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/dialog">element</see> that is rendered.</param>
    /// <returns>True if the element was set; otherwise, false.</returns>
    public bool TrySetElement(ElementReference element);

    /// <summary>
    /// Tries to set the result that is returned when the dialog is closed.
    /// </summary>
    /// <param name="result">The result that is returned when the dialog is closed.</param>
    /// <returns>True if the result was set; otherwise, false.</returns>
    public bool TrySetResult(IDialogResult? result = null);
}

/// <summary>
/// The result that is returned when the dialog is closed.
/// </summary>
public interface IDialogResult
{
    /// <summary>
    /// Gets the data for the result.
    /// </summary>
    public object? Data { get; }
}

/// <summary>
/// The service that shows and closes dialogs.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Invoked when a dialog is closed with the refernece to the dialog and the result that is returned when the dialog is closed.
    /// </summary>
    public event Func<IDialogReference, IDialogResult?, Task>? OnDialogClosed;

    /// <summary>
    /// Invoked when a dialog is shown with the reference to the dialog.
    /// </summary>
    public event Func<IDialogReference, Task>? OnDialogShown;

    /// <summary>
    /// Closes the dialog that is referred to by the <see cref="reference"/>.
    /// </summary>
    /// <param name="reference">The reference to the dialog that is shown.</param>
    /// <param name="result">The result that is returned when the dialog is closed.</param>
    /// <returns>A task that completes when the dialog was closed.</returns>
    public Task Close(IDialogReference reference, IDialogResult? result = null);

    /// <summary>
    /// Shows the dialog that is represented by the <see cref="TComponent"/>.
    /// </summary>
    /// <param name="parameters">The parameters to pass to the <see cref="TComponent"/>.</param>
    /// <typeparam name="TComponent">The type of the component that represents the dialog.</typeparam>
    /// <returns>A task that completes with the reference to the dialog that was shown.</returns>
    public Task<IDialogReference> Show<TComponent>(IDictionary<string, object?>? parameters = null);
}

using Microsoft.JSInterop;

namespace AzureAppConfigurationEmulator.Services;

public class DialogService(IJSRuntime js) : IAsyncDisposable, IDialogService
{
    private Lazy<ValueTask<IJSObjectReference>> Module { get; } = new(() => js.InvokeAsync<IJSObjectReference>("import", "./scripts/dialog.js"));

    public async ValueTask DisposeAsync()
    {
        if (Module.IsValueCreated)
        {
            var module = await Module.Value;

            await module.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }

    public async Task CloseAsync(string id)
    {
        var module = await Module.Value;

        await module.InvokeVoidAsync("close", id);
    }

    public async Task CloseAsync<TResult>(string id, TResult result)
    {
        var module = await Module.Value;

        await module.InvokeVoidAsync("close", id, result);
    }

    public async Task ShowAsync(string id)
    {
        var module = await Module.Value;

        await module.InvokeVoidAsync("show", id);
    }
}

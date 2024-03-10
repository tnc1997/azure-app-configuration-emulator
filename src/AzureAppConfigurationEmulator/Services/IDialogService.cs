namespace AzureAppConfigurationEmulator.Services;

public interface IDialogService
{
    public Task CloseAsync(string id);
    public Task CloseAsync<TResult>(string id, TResult result);
    
    public Task ShowAsync(string id);
}

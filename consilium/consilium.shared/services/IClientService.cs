namespace Consilium.Shared.Services;

public interface IClientService {
    Task<HttpResponseMessage> GetAsync(string url);
    Task<HttpResponseMessage> PostAsync(string url, object? content);
    Task<HttpResponseMessage> PatchAsync(string url, object? content);
    Task<HttpResponseMessage> DeleteAsync(string url);
    void UpdateHeaders(string email, string token);
}
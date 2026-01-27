using System.Net.Http.Json;

namespace Consilium.Shared.Services;

public class ClientService : IClientService {
    private HttpClient client;
    public ClientService(IHttpClientFactory factory) {
        client = factory.CreateClient("ApiClient");
    }

    public void UpdateHeaders(string email, string token) {
        client.DefaultRequestHeaders.Clear();
        try {
            client.DefaultRequestHeaders.Add("Email-Auth_Email", email);
            client.DefaultRequestHeaders.Add("Email-Auth_ApiKey", token);
        } catch {
            client.DefaultRequestHeaders.Add("Email-Auth_Email", "");
            client.DefaultRequestHeaders.Add("Email-Auth_ApiKey", "");
        }
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url) {
        return await client.DeleteAsync(url);
    }

    public async Task<HttpResponseMessage> GetAsync(string url) {
        return await client.GetAsync(url);
    }

    public async Task<HttpResponseMessage> PatchAsync(string url, object? content) {
        return await client.PatchAsJsonAsync(url, content);
    }

    public async Task<HttpResponseMessage> PostAsync(string url, object? content) {
        return await client.PostAsJsonAsync(url, content);
    }
}
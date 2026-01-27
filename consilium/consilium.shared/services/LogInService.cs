namespace Consilium.Shared.Services;

public class LogInService : ILogInService {
    private readonly IClientService client;
    private readonly IPersistenceService service;

    public LogInService(IClientService client, IPersistenceService service) {
        this.client = client;
        this.service = service;
    }

    public async Task<bool> DeleteAccount() {
        var response = await client.DeleteAsync($"/account/delete");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> GlobalLogOut() {
        var response = await client.GetAsync($"/account/signout/global");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> IsValidLogIn() {
        var response = await client.GetAsync($"/account/valid");
        return response.IsSuccessStatusCode;
    }

    public async Task<string> LogIn(string email) {
        var response = await client.PostAsync($"/account?email={email}", null);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<bool> LogOut() {
        var response = await client.GetAsync($"/account/signout");
        service.DeleteToken();
        return response.IsSuccessStatusCode;
    }
    public async Task<bool> CheckAuthStatus() {

        try {
            var response = await client.GetAsync("/account/valid");
            return response.IsSuccessStatusCode;
        } catch {
            return false;
        }

    }
}
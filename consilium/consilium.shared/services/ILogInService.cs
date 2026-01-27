namespace Consilium.Shared.Services;
public interface ILogInService {
    Task<string> LogIn(string email);
    Task<bool> IsValidLogIn();
    Task<bool> LogOut();
    Task<bool> GlobalLogOut();
    Task<bool> DeleteAccount();
    Task<bool> CheckAuthStatus();
}